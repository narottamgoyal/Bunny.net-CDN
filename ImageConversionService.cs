using SkiaSharp;
using System;
using System.IO;

/// <summary>
/// Service for converting images to WebP format and generating thumbnails.
/// </summary>
public class ImageConversionService
{
    private readonly int _thumbnailQuality;
    private readonly int _imageQuality;
    private readonly int _thumbnailMaxDimension;

    public ImageConversionService(int thumbnailQuality = 60, int imageQuality = 75, int thumbnailMaxDimension = 120)
    {
        _thumbnailQuality = Math.Clamp(thumbnailQuality, 0, 100);
        _imageQuality = Math.Clamp(imageQuality, 0, 100);
        _thumbnailMaxDimension = thumbnailMaxDimension > 0 ? thumbnailMaxDimension : 120;
    }

    // ==========================
    // Public API
    // ==========================

    /// <summary>
    /// Converts the input image to a WebP thumbnail.
    /// </summary>
    /// <param name="filePath">Path to the image file.</param>
    /// <returns>A <see cref="MemoryStream"/> containing the WebP-encoded thumbnail. The caller is responsible for disposing the stream.</returns>
    /// <remarks>
    /// This method allocates unmanaged memory for the image internally.
    /// The returned <see cref="MemoryStream"/> **must be disposed** after use to avoid memory pressure.
    /// Recommended usage:
    /// <code>
    /// using var thumbnailStream = imageService.ConvertThumbnail("image.jpg");
    /// // Use thumbnailStream here
    /// </code>
    /// </remarks>
    public MemoryStream ConvertThumbnail(byte[] bytes) => ConvertInternal(bytes: bytes, quality: _thumbnailQuality, isThumbnail: true);
    public MemoryStream ConvertThumbnail(string filePath) => ConvertInternal(filePath: filePath, quality: _thumbnailQuality, isThumbnail: true);
    public MemoryStream ConvertThumbnail(Stream stream) => ConvertInternal(stream: stream, quality: _thumbnailQuality, isThumbnail: true);

    /// <summary>
    /// Converts the input image to a WebP full-size image.
    /// </summary>
    /// <param name="filePath">Path to the image file.</param>
    /// <returns>A <see cref="MemoryStream"/> containing the WebP-encoded image. The caller is responsible for disposing the stream.</returns>
    /// <remarks>
    /// Ensure proper disposal of the returned stream to prevent memory leaks:
    /// <code>
    /// using var imageStream = imageService.ConvertImage("image.jpg");
    /// // Use imageStream here
    /// </code>
    /// </remarks>
    public MemoryStream ConvertImage(byte[] bytes) => ConvertInternal(bytes: bytes, quality: _imageQuality, isThumbnail: false);
    public MemoryStream ConvertImage(string filePath) => ConvertInternal(filePath: filePath, quality: _imageQuality, isThumbnail: false);
    public MemoryStream ConvertImage(Stream stream) => ConvertInternal(stream: stream, quality: _imageQuality, isThumbnail: false);

    // ==========================
    // Core Logic
    // ==========================
    private MemoryStream ConvertInternal(string? filePath = null, byte[]? bytes = null, Stream? stream = null, int quality = 75, bool isThumbnail = false)
    {
        // 1. Decode the original image (Wrapped in using to fix Memory Leak)
        using SKBitmap originalBitmap = DecodeImage(filePath, bytes, stream);

        // 2. Determine which bitmap to encode (Original or Resized)
        SKBitmap bitmapToEncode = originalBitmap;
        SKBitmap? resizedBitmap = null;

        if (isThumbnail)
        {
            // Only resize if the original is larger than the target
            if (originalBitmap.Width > _thumbnailMaxDimension || originalBitmap.Height > _thumbnailMaxDimension)
            {
                resizedBitmap = ResizeThumbnail(originalBitmap, _thumbnailMaxDimension);
                if (resizedBitmap == null)
                {
                    // TODO: log Failed to resize bitmap
                    bitmapToEncode = originalBitmap;
                }
                else
                {
                    bitmapToEncode = resizedBitmap;
                }
            }
        }

        try
        {
            // 3. Encode to WebP
            using var image = SKImage.FromBitmap(bitmapToEncode);
            using var data = image.Encode(SKEncodedImageFormat.Webp, quality);

            var memStream = new MemoryStream();
            data.SaveTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }
        finally
        {
            // 4. Cleanup: If we created a temporary resized bitmap, dispose it.
            // Note: originalBitmap is disposed automatically by the 'using' statement at step 1.
            resizedBitmap?.Dispose();
        }
    }

    private SKBitmap DecodeImage(string? filePath, byte[]? bytes, Stream? stream)
    {
        if (filePath != null) return SKBitmap.Decode(filePath);
        if (bytes != null) return SKBitmap.Decode(bytes);
        if (stream != null)
        {
            // Rewind stream if possible to ensure we read from start
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
            return SKBitmap.Decode(stream);
        }
        throw new ArgumentException("No valid input provided to decode image.");
    }

    private SKBitmap ResizeThumbnail(SKBitmap bitmap, int maxDimension)
    {
        float scale = Math.Min((float)maxDimension / bitmap.Width, (float)maxDimension / bitmap.Height);

        int newWidth = (int)(bitmap.Width * scale);
        int newHeight = (int)(bitmap.Height * scale);

        var info = new SKImageInfo(newWidth, newHeight);

        // Use Medium or High quality filter for smoother thumbnails
        // SKSamplingOptions.Default is often "NearestNeighbor" which looks pixelated
        var sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Nearest);

        return bitmap.Resize(info, sampling);
    }
}