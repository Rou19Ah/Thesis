// ------------------------------------------------------------------
// RotateOperation (Optimized Version)
//
// What it does:
// This class rotates an image by any angle the user provides,
// such as 45°, 90°, 180°, etc.
//
// How it works (in simple terms):
// - The program asks the user to enter a rotation angle in degrees.
// - It then creates a new image (same size as the original).
// - Using graphics tools, it draws the original image rotated
//   by that angle onto the new image.
//
// What happens behind the scenes:
// - A "graphics context" is used to apply the rotation smoothly.
// - It first moves the center of the image to the origin (for clean rotation).
// - Then it rotates the drawing canvas.
// - After rotation, it draws the original image into place.
// - The result is a clean, high-quality rotated image.
//
// Why this version is optimized:
// - It uses hardware-accelerated drawing with `Graphics` and `InterpolationMode`.
// - The image is processed in memory and reused efficiently.
// - Instead of pixel-by-pixel rotation, it transforms the whole image as one block.
//
// Limitations:
// - The rotated image might get cut off if it goes outside the bounds.
//   (This version keeps the original image size instead of resizing the canvas.)
//
// Example:
// - Rotate a photo by 45 degrees → it spins the image visually like turning a paper.
// - If the angle is 180, it flips the image upside down.
//
// ------------------------------------------------------------------


using MainApp;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OptimizedApp;

public class RotateOperation : IOperation
{
    private float _angle;

    public void Configure()
    {
        Console.Write("Enter rotation angle (in degrees): ");
        _angle = float.Parse(Console.ReadLine()!);
    }

    public async Task<ByteArrayImageContext> ApplyAsync(ByteArrayImageContext context)
    {
        using var original = context.ToBitmap();
        using var rotated = new Bitmap(original.Width, original.Height);

        rotated.SetResolution(original.HorizontalResolution, original.VerticalResolution);

        using (Graphics g = Graphics.FromImage(rotated))
        {
            g.Clear(Color.Black);
            g.TranslateTransform(original.Width / 2f, original.Height / 2f);
            g.RotateTransform(_angle);
            g.TranslateTransform(-original.Width / 2f, -original.Height / 2f);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(original, new Point(0, 0));
        }

        context.ReplaceWith(rotated);
        return await Task.FromResult(context);
    }
}
