using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Area27.Hub.Helpers;

/// <summary>
/// Provides image processing utilities for compression, format conversion, and size management.
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// Formats supported for image conversion.
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>PNG format</summary>
        Png,
        /// <summary>JPEG format</summary>
        Jpg,
        /// <summary>Bitmap format</summary>
        Bmp
    }

    /// <summary>
    /// Processes an image: converts format and compresses if necessary to meet size requirements.
    /// </summary>
    /// <param name="imageInput">Image as base64 string or file path.</param>
    /// <param name="maxSizeMb">Maximum allowed size in megabytes.</param>
    /// <param name="outputFormat">Desired output format.</param>
    /// <returns>Processed image as base64 string.</returns>
    /// <exception cref="ArgumentException">Thrown when input is invalid or image cannot be processed.</exception>
    public static string ProcessImage(string imageInput, decimal maxSizeMb, ImageFormat outputFormat = ImageFormat.Jpg)
    {
        if (string.IsNullOrWhiteSpace(imageInput))
            throw new ArgumentException("Image input cannot be null or empty.", nameof(imageInput));

        if (maxSizeMb <= 0)
            throw new ArgumentException("Maximum size must be greater than zero.", nameof(maxSizeMb));

        byte[] imageBytes = LoadImageBytes(imageInput);
        
        using var image = Image.Load(imageBytes);
        
        var maxSizeBytes = (long)(maxSizeMb * 1024 * 1024);
        var currentSizeBytes = imageBytes.Length;

        // If already smaller than max, just convert format
        if (currentSizeBytes <= maxSizeBytes)
        {
            return ConvertImageFormat(image, outputFormat, 90);
        }

        // Compress until size requirement is met
        return CompressImage(image, outputFormat, maxSizeBytes);
    }

    /// <summary>
    /// Converts an image to a specified format.
    /// </summary>
    /// <param name="imageInput">Image as base64 string or file path.</param>
    /// <param name="outputFormat">Desired output format.</param>
    /// <returns>Converted image as base64 string.</returns>
    public static string ConvertImageFormat(string imageInput, ImageFormat outputFormat)
    {
        if (string.IsNullOrWhiteSpace(imageInput))
            throw new ArgumentException("Image input cannot be null or empty.", nameof(imageInput));

        byte[] imageBytes = LoadImageBytes(imageInput);
        using var image = Image.Load(imageBytes);
        
        return ConvertImageFormat(image, outputFormat, 90);
    }

    /// <summary>
    /// Gets the size of an image in megabytes.
    /// </summary>
    /// <param name="imageInput">Image as base64 string or file path.</param>
    /// <returns>Size in megabytes.</returns>
    public static decimal GetImageSizeMb(string imageInput)
    {
        if (string.IsNullOrWhiteSpace(imageInput))
            throw new ArgumentException("Image input cannot be null or empty.", nameof(imageInput));

        byte[] imageBytes = LoadImageBytes(imageInput);
        return imageBytes.Length / (1024m * 1024m);
    }

    // ============ Private Methods ============

    private static byte[] LoadImageBytes(string imageInput)
    {
        // Try to interpret as base64
        if (IsBase64(imageInput))
        {
            try
            {
                return Convert.FromBase64String(imageInput);
            }
            catch
            {
                // Fall through to file path attempt
            }
        }

        // Try to load from file path
        if (File.Exists(imageInput))
        {
            return File.ReadAllBytes(imageInput);
        }

        throw new ArgumentException("Input must be a valid base64 string or an existing file path.", nameof(imageInput));
    }

    private static bool IsBase64(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length % 4 != 0)
            return false;

        try
        {
            Convert.FromBase64String(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string ConvertImageFormat(Image image, ImageFormat format, int jpegQuality = 90)
    {
        using var outputStream = new MemoryStream();

        switch (format)
        {
            case ImageFormat.Png:
                image.SaveAsPng(outputStream);
                break;
            case ImageFormat.Jpg:
                var jpegEncoder = new JpegEncoder { Quality = jpegQuality };
                image.SaveAsJpeg(outputStream, jpegEncoder);
                break;
            case ImageFormat.Bmp:
                image.SaveAsBmp(outputStream);
                break;
            default:
                throw new ArgumentException($"Unsupported format: {format}", nameof(format));
        }

        outputStream.Position = 0;
        byte[] bytes = outputStream.ToArray();
        return Convert.ToBase64String(bytes);
    }

    private static string CompressImage(Image image, ImageFormat outputFormat, long maxSizeBytes)
    {
        int quality = 90;
        byte[] compressed;

        // Binary search for best quality that fits the size requirement
        while (quality > 10)
        {
            using var stream = new MemoryStream();

            switch (outputFormat)
            {
                case ImageFormat.Jpg:
                    var jpegEncoder = new JpegEncoder { Quality = quality };
                    image.SaveAsJpeg(stream, jpegEncoder);
                    break;
                case ImageFormat.Png:
                    image.SaveAsPng(stream);
                    break;
                case ImageFormat.Bmp:
                    image.SaveAsBmp(stream);
                    break;
                default:
                    throw new ArgumentException($"Unsupported format: {outputFormat}", nameof(outputFormat));
            }

            compressed = stream.ToArray();

            if (compressed.Length <= maxSizeBytes)
            {
                return Convert.ToBase64String(compressed);
            }

            // Reduce quality and try again
            quality -= 5;

            // Reduce dimensions if quality alone isn't enough
            if (quality <= 10 && compressed.Length > maxSizeBytes)
            {
                var scale = Math.Sqrt((double)maxSizeBytes / compressed.Length);
                var newWidth = (int)(image.Width * scale);
                var newHeight = (int)(image.Height * scale);

                if (newWidth > 0 && newHeight > 0)
                {
                    image.Mutate(x => x.Resize(newWidth, newHeight));
                    quality = 90;
                }
            }
        }

        using (var stream = new MemoryStream())
        {
            switch (outputFormat)
            {
                case ImageFormat.Jpg:
                    var jpegEncoder = new JpegEncoder { Quality = 10 };
                    image.SaveAsJpeg(stream, jpegEncoder);
                    break;
                case ImageFormat.Png:
                    image.SaveAsPng(stream);
                    break;
                case ImageFormat.Bmp:
                    image.SaveAsBmp(stream);
                    break;
            }

            compressed = stream.ToArray();
        }

        return Convert.ToBase64String(compressed);
    }
}
