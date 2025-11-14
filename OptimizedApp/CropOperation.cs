// ------------------------------------------------------------------
// CropOperation (Optimized Version)
//
// What it does:
// This class cuts out a specific rectangular area from an image —
// like cropping a photo to focus on one part.
//
// How it works (in simple terms):
// - The program asks the user for the crop settings:
//     - X and Y: where to start cutting (top-left corner of the crop box).
//     - Width and Height: how big the crop area should be.
// - It then creates a new image using just that region.
// - The rest of the image (outside the selected box) is removed.
//
// What happens behind the scenes:
// - The original image is loaded in memory.
// - A rectangular section is defined using the user's numbers.
// - That section is copied (cloned) to create a new cropped image.
// - The original image is replaced with the cropped one.
//
// Why this version is optimized:
// - Uses fast built-in functions (`Clone(...)`) that work directly in memory.
// - Avoids pixel-by-pixel copying for speed and accuracy.
// - Cleanly integrates into an image processing pipeline.
//
// Example:
// - Original image: 1000×800 pixels.
// - User enters: X=100, Y=50, Width=400, Height=300.
// - Result: A new 400×300 image taken from position (100, 50).
//
// Notes:
// - No checks are done here to prevent invalid crop sizes (e.g. too large).
// - The cropped area must fit inside the original image, or it may throw an error.
// - Useful for trimming borders, zooming in on objects, or making thumbnails.
//
// ------------------------------------------------------------------

using MainApp;
using System.Drawing;

namespace OptimizedApp;

public class CropOperation : IOperation
{
    private int _x, _y, _width, _height;

    public void Configure()
    {
        Console.Write("Enter crop X: "); _x = int.Parse(Console.ReadLine()!);
        Console.Write("Enter crop Y: "); _y = int.Parse(Console.ReadLine()!);
        Console.Write("Enter crop width: "); _width = int.Parse(Console.ReadLine()!);
        Console.Write("Enter crop height: "); _height = int.Parse(Console.ReadLine()!);
    }

    public async Task<ByteArrayImageContext> ApplyAsync(ByteArrayImageContext context)
    {
        using var original = context.ToBitmap();
        Rectangle cropRect = new Rectangle(_x, _y, _width, _height);

        using var cropped = original.Clone(cropRect, original.PixelFormat);

        context.ReplaceWith(cropped);
        return await Task.FromResult(context);
    }
}
