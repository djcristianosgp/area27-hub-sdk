using System;
using System.IO;
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Area27.Hub.Helpers;

/// <summary>
/// Provides utilities for converting JSON data to PDF documents.
/// </summary>
public static class PdfHelper
{
    static PdfHelper()
    {
        // Configure QuestPDF license for community use
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Converts JSON string to a formatted PDF document.
    /// </summary>
    /// <param name="json">JSON content as string.</param>
    /// <param name="outputPath">Path where the PDF file will be saved.</param>
    /// <param name="title">Title for the PDF document. Default is "JSON Data".</param>
    /// <exception cref="ArgumentNullException">Thrown when json or outputPath is null or empty.</exception>
    /// <exception cref="JsonException">Thrown when JSON is malformed.</exception>
    public static void JsonToPdf(string json, string outputPath, string title = "JSON Data")
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentNullException(nameof(json), "JSON content cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentNullException(nameof(outputPath), "Output path cannot be null or empty.");

        try
        {
            var jsonDocument = JsonDocument.Parse(json);
            var prettyJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            CreatePdfDocument(prettyJson, outputPath, title);
        }
        catch (JsonException ex)
        {
            throw new JsonException("Failed to parse JSON content.", ex);
        }
    }

    /// <summary>
    /// Converts JSON file to a formatted PDF document.
    /// </summary>
    /// <param name="jsonFilePath">Path to the JSON file.</param>
    /// <param name="pdfOutputPath">Path where the PDF file will be saved.</param>
    /// <param name="title">Title for the PDF document. If null, uses the JSON filename.</param>
    /// <exception cref="FileNotFoundException">Thrown when the JSON file does not exist.</exception>
    /// <exception cref="JsonException">Thrown when JSON is malformed.</exception>
    public static void JsonFileToPdf(string jsonFilePath, string pdfOutputPath, string? title = null)
    {
        if (!File.Exists(jsonFilePath))
            throw new FileNotFoundException($"JSON file not found: {jsonFilePath}");

        var json = File.ReadAllText(jsonFilePath);
        var documentTitle = title ?? Path.GetFileNameWithoutExtension(jsonFilePath);
        
        JsonToPdf(json, pdfOutputPath, documentTitle);
    }

    /// <summary>
    /// Converts JSON object to a formatted PDF document with custom table layout.
    /// </summary>
    /// <param name="json">JSON content as string.</param>
    /// <param name="outputPath">Path where the PDF file will be saved.</param>
    /// <param name="title">Title for the PDF document.</param>
    /// <param name="includeMetadata">If true, includes generation date and other metadata.</param>
    /// <exception cref="ArgumentNullException">Thrown when json or outputPath is null or empty.</exception>
    /// <exception cref="JsonException">Thrown when JSON is malformed.</exception>
    public static void JsonToPdfTable(string json, string outputPath, string title = "JSON Data", bool includeMetadata = true)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentNullException(nameof(json), "JSON content cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentNullException(nameof(outputPath), "Output path cannot be null or empty.");

        try
        {
            var jsonDocument = JsonDocument.Parse(json);
            CreatePdfTableDocument(jsonDocument.RootElement, outputPath, title, includeMetadata);
        }
        catch (JsonException ex)
        {
            throw new JsonException("Failed to parse JSON content.", ex);
        }
    }

    private static void CreatePdfDocument(string content, string outputPath, string title)
    {
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Courier));

                page.Header()
                    .Text(title)
                    .FontSize(18)
                    .SemiBold()
                    .FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Text(content)
                    .FontSize(9)
                    .FontFamily(Fonts.Courier);

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        })
        .GeneratePdf(outputPath);
    }

    private static void CreatePdfTableDocument(JsonElement rootElement, string outputPath, string title, bool includeMetadata)
    {
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .Text(title)
                            .FontSize(18)
                            .SemiBold()
                            .FontColor(Colors.Blue.Medium);

                        if (includeMetadata)
                        {
                            column.Item()
                                .PaddingTop(5)
                                .Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Medium);
                        }
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        RenderJsonElement(column, rootElement, 0);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        })
        .GeneratePdf(outputPath);
    }

    private static void RenderJsonElement(ColumnDescriptor column, JsonElement element, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);

        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    column.Item()
                        .PaddingLeft(indentLevel * 10)
                        .Row(row =>
                        {
                            row.RelativeItem(2)
                                .Text($"{property.Name}:")
                                .FontSize(10)
                                .SemiBold()
                                .FontColor(Colors.Blue.Darken2);

                            if (property.Value.ValueKind != JsonValueKind.Object && 
                                property.Value.ValueKind != JsonValueKind.Array)
                            {
                                row.RelativeItem(3)
                                    .Text(GetValueAsString(property.Value))
                                    .FontSize(10);
                            }
                        });

                    if (property.Value.ValueKind == JsonValueKind.Object || 
                        property.Value.ValueKind == JsonValueKind.Array)
                    {
                        RenderJsonElement(column, property.Value, indentLevel + 1);
                    }
                }
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    column.Item()
                        .PaddingLeft(indentLevel * 10)
                        .Text($"[{index}]")
                        .FontSize(10)
                        .SemiBold()
                        .FontColor(Colors.Green.Darken2);

                    RenderJsonElement(column, item, indentLevel + 1);
                    index++;
                }
                break;

            default:
                column.Item()
                    .PaddingLeft(indentLevel * 10)
                    .Text(GetValueAsString(element))
                    .FontSize(10);
                break;
        }
    }

    private static string GetValueAsString(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number => element.ToString(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "null",
            _ => element.ToString()
        };
    }
}
