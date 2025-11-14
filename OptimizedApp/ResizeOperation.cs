// ------------------------------------------------------------------
// ResizeOperation (Optimized Version)
//
// What it does:
// This class changes the size of an image based on what the user types in.
// For example, you can shrink or enlarge an image to any width and height.
//
// How it works (in simple terms):
// - The program first asks the user to enter a new width and height (in pixels).
// - It then resizes the image to fit those dimensions.
// - The resized image replaces the original one in memory.
//
// What happens behind the scenes:
// - The original image is loaded from memory.
// - A new image is created with the specified size.
// - The system automatically stretches or shrinks the content to fit.
// - The result is stored back into the image context for saving or more editing.
//
// Why this version is optimized:
// - It resizes the image using built-in system tools, which are much faster
//   than adjusting each pixel manually.
// - It uses the `Bitmap` constructor that handles scaling efficiently.
//
// Example:
// - Original image: 800x600 → User enters 400x300 → Image is scaled down (shrunk).
// - Original image: 100x100 → User enters 300x300 → Image is enlarged (upscaled).
//
// Notes:
// - It does not keep the original aspect ratio unless the user inputs matching proportions.
// - The resized image might look stretched or squished if width and height are uneven.
//
// ------------------------------------------------------------------

using MainApp;
using System.Drawing;

namespace OptimizedApp;

public class ResizeOperation : IOperation
{
    private int _newWidth;
    private int _newHeight;

    public void Configure()
    {
        Console.Write("Enter new width: ");
        _newWidth = int.Parse(Console.ReadLine()!);

        Console.Write("Enter new height: ");
        _newHeight = int.Parse(Console.ReadLine()!);
    }

    public async Task<ByteArrayImageContext> ApplyAsync(ByteArrayImageContext context)
    {
        using var original = context.ToBitmap();
        using var resized = new Bitmap(original, new Size(_newWidth, _newHeight));

        context.ReplaceWith(resized);
        return await Task.FromResult(context);
    }
}
