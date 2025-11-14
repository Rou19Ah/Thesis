// Why it's "Unoptimized":
// - It uses high-level `Bitmap` functions for cropping, which are easy to use,
//   but slower than raw memory access methods.
// - Best for simple workflows or learning purposes.

using System.Drawing;
using MainApp;

namespace UnoptimizedApp;

public class CropOperation : IUnoptimizedOperation
{
    private readonly Rectangle cropArea;

    public CropOperation()
    {
        Console.WriteLine("Enter crop area (x, y, width, height):");
        Console.Write("x: ");
        int x = int.TryParse(Console.ReadLine(), out int xv) ? xv : 0;
        Console.Write("y: ");
        int y = int.TryParse(Console.ReadLine(), out int yv) ? yv : 0;
        Console.Write("width: ");
        int w = int.TryParse(Console.ReadLine(), out int wv) ? wv : 100;
        Console.Write("height: ");
        int h = int.TryParse(Console.ReadLine(), out int hv) ? hv : 100;

        cropArea = new Rectangle(x, y, w, h);
    }

    public BitmapImageContext Apply(BitmapImageContext context)
    {
        Bitmap cropped = context.Bitmap.Clone(cropArea, context.Bitmap.PixelFormat);
        context.Bitmap = cropped;
        return context;
    }
}
