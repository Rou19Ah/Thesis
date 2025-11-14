// Why this is Unoptimized:
// - It uses `GetPixel` and `SetPixel` for every single pixel in the image.
// - These methods are very slow because they perform a lot of internal checks
//   and memory access for each pixel one at a time.
// - For large images, this can make the operation take several minutes.
// - Optimized versions use raw memory access (like byte arrays or pointers)
//   to process the entire image much faster in a single loop.

using System.Drawing;
using MainApp;

namespace UnoptimizedApp;

public class GrayscaleOperation : IUnoptimizedOperation
{
    public BitmapImageContext Apply(BitmapImageContext context)
    {
        Bitmap original = context.Bitmap;
        Bitmap result = new(original.Width, original.Height);

        for (int y = 0; y < original.Height; y++)
        {
            for (int x = 0; x < original.Width; x++)
            {
                var pixel = original.GetPixel(x, y);
                int gray = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                result.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
            }
        }

        context.Bitmap = result;
        //context.ReplaceBitmap(result);
        return context;
    }
}