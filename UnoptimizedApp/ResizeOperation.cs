// Why this is Unoptimized:
// - It uses the high-level `Graphics.DrawImage()` method,
//   which is simple but slower compared to direct memory manipulation.
// - This approach creates a new Bitmap and resizes pixel data indirectly,
//   relying on the .NET graphics engine, which adds some overhead.
// - It also uses the `Bitmap` class from `System.Drawing`, which is not memory-efficient
//   for large batches or real-time processing.
// - Optimized versions operate on raw image bytes (e.g., byte arrays or spans),
//   avoiding the overhead of graphics contexts and improving performance.

using System.Drawing;
using MainApp;

namespace UnoptimizedApp;

public class ResizeOperation : IUnoptimizedOperation
{
    private readonly int newWidth;
    private readonly int newHeight;

    public ResizeOperation()
    {
        Console.Write("Enter new width: ");
        newWidth = int.TryParse(Console.ReadLine(), out int w) ? w : 100;

        Console.Write("Enter new height: ");
        newHeight = int.TryParse(Console.ReadLine(), out int h) ? h : 100;
    }

    public BitmapImageContext Apply(BitmapImageContext context)
    {
        Bitmap resized = new(newWidth, newHeight);
        using Graphics g = Graphics.FromImage(resized);
        g.DrawImage(context.Bitmap, 0, 0, newWidth, newHeight);

        context.Bitmap = resized;
        return context;
    }
}
