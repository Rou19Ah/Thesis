// ------------------------------------------------------------------
// ByteArrayImageContext (High-Performance Image Buffer)
//
// What it does:
// This class stores image data in a raw byte array format (in memory),
// allowing fast and efficient processing of images (like filters or transformations).
//
// Why use it:
// - Working with regular Bitmap images (via GetPixel/SetPixel) is very slow.
// - This class gives direct access to the image data in memory as RGB bytes.
// - It’s ideal for performance-critical image operations like grayscale, rotate, resize, etc.
//
// How it works (in simple terms):
// ● When you load an image from disk:
//   - The image is read using `Bitmap`, but converted into a flat memory block (byte array).
//   - Each pixel is represented by 3 bytes (R, G, B).
//   - This memory is managed by a shared memory pool to reduce allocations.
//
// ● When you want to modify or save the image:
//   - You operate directly on the memory (`PixelMemory`) — super fast!
//   - You can convert it back to a `Bitmap` using `ToBitmap()` when needed.
//   - If an operation like cropping or resizing is applied, `ReplaceWith(...)` updates the internal memory.
//
// Important methods:
// ● `PixelMemory`
//   - This gives you direct access to the byte data of the image.
//   - It's safe and efficient to read/write RGB values directly.
//
// ● `ToBitmap()`
//   - Converts the raw memory back into a standard .NET `Bitmap`,
//     so you can save or display the image.
//
// ● `ReplaceWith(Bitmap bmp)`
//   - Used when an operation creates a new `Bitmap` (like crop, rotate).
//   - Replaces the current memory with the new image content.
//
// ● `Dispose()`
//   - Releases the memory back to the system (via memory pool).
//   - You should use this class with `using` or call `.Dispose()` manually to avoid memory leaks.
//
// Optimization benefits:
// Loads and processes images up to 100x faster than classic `Bitmap.GetPixel()` loops.
// Uses memory pools to avoid repeatedly allocating and freeing large memory chunks.
// Keeps everything in memory until final saving — perfect for pipeline processing.
//
// Example usage:
// - Load image → process as raw bytes → convert to bitmap → save.
// - No drawing, no UI, just raw memory processing = high performance.
//
// ------------------------------------------------------------------


using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MainApp;

public class ByteArrayImageContext : BaseImageContext, IDisposable
{
    private IMemoryOwner<byte> _memoryOwner;

    public Memory<byte> PixelMemory => _memoryOwner.Memory[..PixelCount];
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int PixelCount { get; private set; }

    public ByteArrayImageContext(string path) : base(Path.GetFileName(path))
    {
        using var bmp = new Bitmap(path);
        Width = bmp.Width;
        Height = bmp.Height;

        PixelCount = Width * Height * 3;
        _memoryOwner = MemoryPool<byte>.Shared.Rent(PixelCount);

        LoadPixelsFromBitmap(bmp);
    }

    private void LoadPixelsFromBitmap(Bitmap bmp)
    {
        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

        /// replacement for Getpixel method which is used for bitmap
        /// which in case memory usage might be lower but in case performance it takes 2 second 
        /// operation to 5 minute
        var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

        try
        {
            int stride = data.Stride;
            var span = _memoryOwner.Memory.Span;

            byte[] temp = new byte[stride * Height];
            Marshal.Copy(data.Scan0, temp, 0, temp.Length);

            for (int y = 0; y < Height; y++)
            {
                int srcOffset = y * stride;
                int destOffset = y * Width * 3;
                temp.AsSpan(srcOffset, Width * 3).CopyTo(span.Slice(destOffset, Width * 3));
            }
        }
        finally
        {
            bmp.UnlockBits(data);
        }
    }

    public Bitmap ToBitmap()
    {
        var bmp = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
        var rect = new Rectangle(0, 0, Width, Height);
        var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

        try
        {
            int stride = data.Stride;
            var span = PixelMemory.Span;

            byte[] temp = new byte[stride * Height];

            for (int y = 0; y < Height; y++)
            {
                int srcOffset = y * Width * 3;
                int destOffset = y * stride;

                span.Slice(srcOffset, Width * 3).CopyTo(temp.AsSpan(destOffset, Width * 3));
            }

            Marshal.Copy(temp, 0, data.Scan0, temp.Length);
        }
        finally
        {
            bmp.UnlockBits(data);
        }

        return bmp;
    }

    // method will be used for resize, crop and rotate methods
    public void ReplaceWith(Bitmap bmp)
    {
        Width = bmp.Width;
        Height = bmp.Height;
        PixelCount = Width * Height * 3;

        _memoryOwner = MemoryPool<byte>.Shared.Rent(PixelCount);

        var span = _memoryOwner.Memory.Span;

        // Lock the bitmap for direct memory access (safe version)
        BitmapData data = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb
        );

        try
        {
            IntPtr scan0 = data.Scan0;
            int stride = data.Stride;
            byte[] rawData = new byte[stride * bmp.Height];
            Marshal.Copy(scan0, rawData, 0, rawData.Length);

            int index = 0;
            for (int y = 0; y < bmp.Height; y++)
            {
                int rowStart = y * stride;
                for (int x = 0; x < bmp.Width; x++)
                {
                    int offset = rowStart + x * 3;

                    byte b = rawData[offset];
                    byte g = rawData[offset + 1];
                    byte r = rawData[offset + 2];

                    span[index++] = r;
                    span[index++] = g;
                    span[index++] = b;
                }
            }
        }
        finally
        {
            bmp.UnlockBits(data);
        }
    }


    // only could be used with using keyword to avoid overwhelming memory had to call GC collect.
    public void Dispose() => _memoryOwner?.Dispose();
}
