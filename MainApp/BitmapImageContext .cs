// ------------------------------------------------------------------
// BitmapImageContext (Basic Image Holder)
//
// What it does:
// This class holds a loaded image (as a `Bitmap`) and allows operations
// to modify or replace that image. It’s used in the unoptimized version
// of the app where image editing is done with high-level tools.
//
// How it works (in simple terms):
// - When you give it an image path, it loads that image into memory
//   using the standard .NET `Bitmap` class.
// - Other parts of the program (like grayscale, crop, etc.) can
//   read or change the image using this object.
// - If an operation produces a new version of the image,
//   it can call `ReplaceBitmap(...)` to update the stored image.
//
// Key property:
// ● `Bitmap`
//   - The main image stored in memory.
//   - All image operations read or modify this directly.
//
// Key method:
// ● `ReplaceBitmap(Bitmap newBitmap)`
//   - Replaces the current image with a new one.
//   - Used when an operation like resize or rotate generates a new image.
//
// Example:
// - Load "cat.jpg" → store it as a `Bitmap` inside this context.
// - Apply grayscale → update the stored image.
// - Later save the result to disk using this updated `Bitmap`.
//
// Why this is called "unoptimized":
// - The `Bitmap` class is easy to use but slower for large image processing.
// - Works fine for learning and small projects, but can become slow
//   compared to memory-optimized versions like `ByteArrayImageContext`.
//
// ------------------------------------------------------------------


using System.Drawing;

namespace MainApp;

public class BitmapImageContext : BaseImageContext
{
    public Bitmap Bitmap { get; set; }

    public BitmapImageContext(string path) : base(Path.GetFileName(path))
    {
        Bitmap = new Bitmap(path);
    }

    public void ReplaceBitmap(Bitmap newBitmap)
    {
        Bitmap = newBitmap;
    }
}
