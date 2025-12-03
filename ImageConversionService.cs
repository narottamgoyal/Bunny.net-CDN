using SkiaSharp;
using System;
using System.IO;

public class ImageConversionService
{
    private readonly int _thumbnailQuality;
    private readonly int _imageQuality;

    public ImageConversionService(int thumbnailQuality = 60, int imageQuality = 75)
    {
        if (thumbnailQuality < 0 || thumbnailQuality > 100)
            throw new ArgumentOutOfRangeException(nameof(thumbnailQuality));
        if (imageQuality < 0 || imageQuality > 100)
            throw new ArgumentOutOfRangeException(nameof(imageQuality));

        _thumbnailQuality = thumbnailQuality;
        _imageQuality = imageQuality;
    }

    // ==========================
    // High-level Entry Methods
    // ==========================

    // Thumbnail (small quality)
    public MemoryStream ConvertThumbnail(string filePath) => ConvertFromFile(filePath, _thumbnailQuality);
    public MemoryStream ConvertThumbnail(byte[] bytes) => ConvertFromBytes(bytes, _thumbnailQuality);
    public MemoryStream ConvertThumbnail(Stream stream) => ConvertFromStream(stream, _thumbnailQuality);
    public MemoryStream ConvertThumbnailFromBase64(string base64) => ConvertFromBase64(base64, _thumbnailQuality);

    // Full image (higher quality)
    public MemoryStream ConvertImage(string filePath) => ConvertFromFile(filePath, _imageQuality);
    public MemoryStream ConvertImage(byte[] bytes) => ConvertFromBytes(bytes, _imageQuality);
    public MemoryStream ConvertImage(Stream stream) => ConvertFromStream(stream, _imageQuality);
    public MemoryStream ConvertImageFromBase64(string base64) => ConvertFromBase64(base64, _imageQuality);

    // ==========================
    // Internal Conversion Methods
    // ==========================
    private MemoryStream ConvertFromFile(string filePath, int quality)
    {
        using var bitmap = SKBitmap.Decode(filePath);
        if (bitmap == null)
            throw new ArgumentException("Unable to decode image from file.", nameof(filePath));

        return ConvertToWebpStream(bitmap, quality);
    }

    private MemoryStream ConvertFromBytes(byte[] bytes, int quality)
    {
        using var bitmap = SKBitmap.Decode(bytes);
        if (bitmap == null)
            throw new ArgumentException("Unable to decode image from bytes.");

        return ConvertToWebpStream(bitmap, quality);
    }

    private MemoryStream ConvertFromStream(Stream stream, int quality)
    {
        using var bitmap = SKBitmap.Decode(stream);
        if (bitmap == null)
            throw new ArgumentException("Unable to decode image from stream.");

        return ConvertToWebpStream(bitmap, quality);
    }

    private MemoryStream ConvertFromBase64(string base64, int quality)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        return ConvertFromBytes(bytes, quality);
    }

    // ==========================
    // Core WebP Conversion
    // ==========================
    private MemoryStream ConvertToWebpStream(SKBitmap bitmap, int quality)
    {
        using var image = SKImage.FromBitmap(bitmap);
        var memStream = new MemoryStream();
        using var data = image.Encode(SKEncodedImageFormat.Webp, quality);
        data.SaveTo(memStream);
        memStream.Seek(0, SeekOrigin.Begin);
        return memStream;
    }
}
