// ------------------------------------------------------------------
// IUnoptimizedOperation Interface
//
// What it does:
// This interface defines the structure for all image operations
// used in the unoptimized version of the app.
//
// How it works:
// - Any class that performs an image operation (like resize, rotate, grayscale)
//   must implement this interface.
// - It requires a single method:
//     Apply(BitmapImageContext context)
//       Takes an image, applies the operation, and returns the updated image.
//
// Why it's "Unoptimized":
// - It uses `BitmapImageContext`, which relies on .NET's high-level `Bitmap` class.
// - These operations typically use slower methods like `GetPixel`, `SetPixel`, and `Graphics.DrawImage`.
// - Suitable for clarity and educational purposes, but not efficient for large-scale processing.
//
// Example Implementations:
// - GrayscaleOperation: changes image to black & white.
// - ResizeOperation: resizes the image to new dimensions.
// - RotateOperation: rotates the image by a given angle.
//
// ------------------------------------------------------------------

namespace UnoptimizedApp;

using MainApp;

public interface IUnoptimizedOperation
{
    BitmapImageContext Apply(BitmapImageContext context);
}
