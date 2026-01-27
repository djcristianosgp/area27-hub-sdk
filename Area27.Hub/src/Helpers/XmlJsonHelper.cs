using System;
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace Area27.Hub.Helpers;

/// <summary>
/// Provides utilities for converting between XML and JSON formats.
/// </summary>
public static class XmlJsonHelper
{
    /// <summary>
    /// Converts XML string to JSON string.
    /// </summary>
    /// <param name="xml">XML content as string.</param>
    /// <param name="indented">If true, formats the output JSON with indentation.</param>
    /// <returns>JSON representation of the XML content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when xml is null or empty.</exception>
    /// <exception cref="XmlException">Thrown when XML is malformed.</exception>
    public static string XmlToJson(string xml, bool indented = true)
    {
        if (string.IsNullOrWhiteSpace(xml))
            throw new ArgumentNullException(nameof(xml), "XML content cannot be null or empty.");

        try
        {
            var doc = XDocument.Parse(xml);
            var jsonObject = XmlToJsonObject(doc.Root!);

            var options = new JsonSerializerOptions
            {
                WriteIndented = indented,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(jsonObject, options);
        }
        catch (XmlException ex)
        {
            throw new XmlException("Failed to parse XML content.", ex);
        }
    }

    /// <summary>
    /// Converts XML file to JSON string.
    /// </summary>
    /// <param name="xmlFilePath">Path to the XML file.</param>
    /// <param name="indented">If true, formats the output JSON with indentation.</param>
    /// <returns>JSON representation of the XML file content.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the XML file does not exist.</exception>
    /// <exception cref="XmlException">Thrown when XML is malformed.</exception>
    public static string XmlFileToJson(string xmlFilePath, bool indented = true)
    {
        if (!File.Exists(xmlFilePath))
            throw new FileNotFoundException($"XML file not found: {xmlFilePath}");

        var xmlContent = File.ReadAllText(xmlFilePath);
        return XmlToJson(xmlContent, indented);
    }

    /// <summary>
    /// Converts XML file to JSON file.
    /// </summary>
    /// <param name="xmlFilePath">Path to the XML file.</param>
    /// <param name="jsonFilePath">Path where the JSON file will be saved.</param>
    /// <param name="indented">If true, formats the output JSON with indentation.</param>
    /// <exception cref="FileNotFoundException">Thrown when the XML file does not exist.</exception>
    /// <exception cref="XmlException">Thrown when XML is malformed.</exception>
    public static void XmlFileToJsonFile(string xmlFilePath, string jsonFilePath, bool indented = true)
    {
        var json = XmlFileToJson(xmlFilePath, indented);
        File.WriteAllText(jsonFilePath, json);
    }

    /// <summary>
    /// Converts JSON string to XML string.
    /// </summary>
    /// <param name="json">JSON content as string.</param>
    /// <param name="rootElementName">Name of the root XML element. Default is "root".</param>
    /// <returns>XML representation of the JSON content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when json is null or empty.</exception>
    /// <exception cref="JsonException">Thrown when JSON is malformed.</exception>
    public static string JsonToXml(string json, string rootElementName = "root")
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentNullException(nameof(json), "JSON content cannot be null or empty.");

        try
        {
            var jsonElement = JsonDocument.Parse(json).RootElement;
            var xmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var rootElement = new XElement(rootElementName);
            
            JsonToXmlElement(jsonElement, rootElement);
            xmlDoc.Add(rootElement);

            return xmlDoc.ToString();
        }
        catch (JsonException ex)
        {
            throw new JsonException("Failed to parse JSON content.", ex);
        }
    }

    /// <summary>
    /// Converts JSON file to XML string.
    /// </summary>
    /// <param name="jsonFilePath">Path to the JSON file.</param>
    /// <param name="rootElementName">Name of the root XML element. Default is "root".</param>
    /// <returns>XML representation of the JSON file content.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the JSON file does not exist.</exception>
    /// <exception cref="JsonException">Thrown when JSON is malformed.</exception>
    public static string JsonFileToXml(string jsonFilePath, string rootElementName = "root")
    {
        if (!File.Exists(jsonFilePath))
            throw new FileNotFoundException($"JSON file not found: {jsonFilePath}");

        var jsonContent = File.ReadAllText(jsonFilePath);
        return JsonToXml(jsonContent, rootElementName);
    }

    /// <summary>
    /// Converts JSON file to XML file.
    /// </summary>
    /// <param name="jsonFilePath">Path to the JSON file.</param>
    /// <param name="xmlFilePath">Path where the XML file will be saved.</param>
    /// <param name="rootElementName">Name of the root XML element. Default is "root".</param>
    /// <exception cref="FileNotFoundException">Thrown when the JSON file does not exist.</exception>
    /// <exception cref="JsonException">Thrown when JSON is malformed.</exception>
    public static void JsonFileToXmlFile(string jsonFilePath, string xmlFilePath, string rootElementName = "root")
    {
        var xml = JsonFileToXml(jsonFilePath, rootElementName);
        File.WriteAllText(xmlFilePath, xml);
    }

    private static object? XmlToJsonObject(XElement element)
    {
        if (!element.HasElements && !element.HasAttributes)
        {
            return element.Value;
        }

        var result = new Dictionary<string, object?>();

        // Add attributes
        foreach (var attribute in element.Attributes())
        {
            result[$"@{attribute.Name.LocalName}"] = attribute.Value;
        }

        // Add child elements
        var childGroups = element.Elements().GroupBy(e => e.Name.LocalName);
        foreach (var group in childGroups)
        {
            var items = group.Select(XmlToJsonObject).ToList();
            result[group.Key] = items.Count == 1 ? items[0] : items;
        }

        // If element has both text and children, add text as special property
        if (element.HasElements && !string.IsNullOrWhiteSpace(element.Value))
        {
            var textOnly = element.Nodes().OfType<XText>().Select(t => t.Value).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(textOnly))
            {
                result["#text"] = textOnly.Trim();
            }
        }

        return result;
    }

    private static void JsonToXmlElement(JsonElement element, XElement parent)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    if (property.Name.StartsWith("@"))
                    {
                        // Handle attributes
                        parent.Add(new XAttribute(property.Name.Substring(1), property.Value.ToString()));
                    }
                    else if (property.Name == "#text")
                    {
                        // Handle text content
                        parent.Value = property.Value.ToString();
                    }
                    else
                    {
                        var childElement = new XElement(property.Name);
                        JsonToXmlElement(property.Value, childElement);
                        parent.Add(childElement);
                    }
                }
                break;

            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    var childElement = new XElement("item");
                    JsonToXmlElement(item, childElement);
                    parent.Add(childElement);
                }
                break;

            case JsonValueKind.String:
                parent.Value = element.GetString() ?? string.Empty;
                break;

            case JsonValueKind.Number:
                parent.Value = element.ToString();
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                parent.Value = element.GetBoolean().ToString().ToLower();
                break;

            case JsonValueKind.Null:
                parent.Value = string.Empty;
                break;
        }
    }
}
