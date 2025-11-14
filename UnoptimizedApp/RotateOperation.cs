// Why this is Unoptimized:
// - It uses the high-level `Graphics` object to apply rotation, which is easier to write
//   but slower and more memory-intensive compared to low-level pixel manipulation.
// - The rotated image is drawn onto a new square canvas, which may waste space
//   and increase memory usage unnecessarily.
// - It doesn’t use any optimizations like pointer access or raw byte processing.
// - Optimized versions operate directly on pixel buffers in memory (e.g., using spans or unsafe code),
//   allowing faster execution and better control over memory layout and precision.
// - This version is suitable for small or occasional image rotations, but it becomes inefficient
//   with large image batches or high-frequency usage.

using System.Drawing;
using MainApp;

namespace UnoptimizedApp;

public class RotateOperation : IUnoptimizedOperation
{
    private readonly float _angle;

    public RotateOperation()
    {
        Console.Write("Enter rotation angle (e.g., 45, 90): ");
        _angle = float.TryParse(Console.ReadLine(), out float a) ? a : 0f;
    }

    public BitmapImageContext Apply(BitmapImageContext context)
    {
        Bitmap original = context.Bitmap;

        int w = original.Width;
        int h = original.Height;

        int size = (int)Math.Ceiling(Math.Sqrt(w * w + h * h));
        Bitmap rotated = new(size, size);

        using Graphics g = Graphics.FromImage(rotated);
        {
            g.TranslateTransform(size / 2f, size / 2f);
            g.RotateTransform(_angle);
            g.DrawImage(original, -w / 2f, -h / 2f);
        }

        context.Bitmap = rotated;
        return context;
    }
}
