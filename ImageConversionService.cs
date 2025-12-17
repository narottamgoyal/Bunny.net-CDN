using SkiaSharp;
using System;
using System.IO;

public class ImageConversionService
{
    private readonly int _thumbnailQuality;
    private readonly int _imageQuality;
    private readonly int _thumbnailMaxDimension;

    public ImageConversionService(int thumbnailQuality = 60, int imageQuality = 75, int thumbnailMaxDimension = 120)
    {
        if (thumbnailQuality < 0 || thumbnailQuality > 100)
            throw new ArgumentOutOfRangeException(nameof(thumbnailQuality));
        if (imageQuality < 0 || imageQuality > 100)
            throw new ArgumentOutOfRangeException(nameof(imageQuality));
        if (thumbnailMaxDimension <= 0)
            throw new ArgumentOutOfRangeException(nameof(thumbnailMaxDimension));

        _thumbnailQuality = thumbnailQuality;
        _imageQuality = imageQuality;
        _thumbnailMaxDimension = thumbnailMaxDimension;
    }

    // ==========================
    // High-level Entry Methods
    // ==========================

    // Thumbnail (smaller, optimized for mobile)
    public MemoryStream ConvertThumbnail(string filePath) => ConvertInternal(filePath: filePath, quality: _thumbnailQuality, resizeThumbnail: true);
    public MemoryStream ConvertThumbnail(byte[] bytes) => ConvertInternal(bytes: bytes, quality: _thumbnailQuality, resizeThumbnail: true);
    public MemoryStream ConvertThumbnail(Stream stream) => ConvertInternal(stream: stream, quality: _thumbnailQuality, resizeThumbnail: true);
    public MemoryStream ConvertThumbnailFromBase64(string base64) => ConvertInternal(base64: base64, quality: _thumbnailQuality, resizeThumbnail: true);

    // Full image (higher quality)
    public MemoryStream ConvertImage(string filePath) => ConvertInternal(filePath: filePath, quality: _imageQuality, resizeThumbnail: false);
    public MemoryStream ConvertImage(byte[] bytes) => ConvertInternal(bytes: bytes, quality: _imageQuality, resizeThumbnail: false);
    public MemoryStream ConvertImage(Stream stream) => ConvertInternal(stream: stream, quality: _imageQuality, resizeThumbnail: false);
    public MemoryStream ConvertImageFromBase64(string base64) => ConvertInternal(base64: base64, quality: _imageQuality, resizeThumbnail: false);

    // ==========================
    // Centralized Decode + Convert
    // ==========================
    private MemoryStream ConvertInternal(string? filePath = null, byte[]? bytes = null, Stream? stream = null, string? base64 = null, int quality = 75, bool resizeThumbnail = false)
    {
        SKBitmap bitmap = DecodeImage(filePath, bytes, stream, base64);

        SKBitmap processedBitmap = bitmap;

        if (resizeThumbnail)
        {
            processedBitmap = ResizeThumbnail(bitmap, _thumbnailMaxDimension);
        }

        using var image = SKImage.FromBitmap(processedBitmap);
        var memStream = new MemoryStream();
        using var data = image.Encode(SKEncodedImageFormat.Webp, quality);
        data.SaveTo(memStream);
        memStream.Seek(0, SeekOrigin.Begin);

        if (resizeThumbnail && processedBitmap != bitmap)
            processedBitmap.Dispose();

        return memStream;
    }

    // ==========================
    // Decode Helper
    // ==========================
    private SKBitmap DecodeImage(string? filePath = null, byte[]? bytes = null, Stream? stream = null, string? base64 = null)
    {
        SKBitmap bitmap = null;

        if (filePath != null)
            bitmap = SKBitmap.Decode(filePath);
        else if (bytes != null)
            bitmap = SKBitmap.Decode(bytes);
        else if (stream != null)
            bitmap = SKBitmap.Decode(stream);
        else if (base64 != null)
        {
            var decodedBytes = Convert.FromBase64String(base64);
            bitmap = SKBitmap.Decode(decodedBytes);
        }

        if (bitmap == null)
            throw new ArgumentException("Unable to decode image from provided input.");

        return bitmap;
    }

    // ==========================
    // Thumbnail Resizing
    // ==========================
    private SKBitmap ResizeThumbnail(SKBitmap bitmap, int maxDimension)
    {
        float scale = Math.Min((float)maxDimension / bitmap.Width, (float)maxDimension / bitmap.Height);
        int width = (int)(bitmap.Width * scale);
        int height = (int)(bitmap.Height * scale);

        return bitmap.Resize(new SKImageInfo(width, height), SKSamplingOptions.Default);
    }
}
