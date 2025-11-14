// ------------------------------------------------------------------
// GrayscaleOperation (Optimized Version)
//
// What it does:
// This class turns a colored image into black and white (grayscale).
//
// How it works (in simple terms):
// - Every image is made up of tiny dots called pixels.
// - Each pixel has 3 color values: red, green, and blue (RGB).
// - To make an image grayscale, we take those three values
//   and blend them into one "gray" value that looks like brightness.
//
// What happens behind the scenes:
// - The program looks at every pixel in the image, one by one.
// - It reads the red, green, and blue values.
// - It calculates the average brightness using a standard formula:
//     Gray = 0.3 × Red + 0.59 × Green + 0.11 × Blue
//   (This gives more weight to green, since human eyes see green more clearly.)
// - Then it sets all 3 color values (R, G, B) to the same gray level.
// - The image still has color channels, but every pixel now looks gray.
//
// Why this version is optimized:
// - It works directly on the raw image memory (`PixelMemory`) using fast math.
// - No slow drawing functions or bitmap conversions are used.
// - It processes all pixels efficiently in a single loop.
//
// Example:
// - A blue sky becomes light gray.
// - A red apple becomes medium gray.
// - A black shape stays black, and white stays white.
//
// Notes:
// - No extra setup is needed — it just runs directly when selected.
// - Works great in high-performance pipelines.
//
// ------------------------------------------------------------------


using MainApp;

namespace OptimizedApp;

public class GrayscaleOperation : IOperation
{
    public void Configure()
    {
        Console.WriteLine("GrayscaleOperation: No configuration needed.");
    }

    public async Task<ByteArrayImageContext> ApplyAsync(ByteArrayImageContext context)
    {

        for (int i = 0; i < context.PixelMemory.Span.Length; i += 3)
        {
            byte r = context.PixelMemory.Span[i];
            byte g = context.PixelMemory.Span[i + 1];
            byte b = context.PixelMemory.Span[i + 2];

            byte gray = (byte)(0.3 * r + 0.59 * g + 0.11 * b);

            context.PixelMemory.Span[i] = gray;
            context.PixelMemory.Span[i + 1] = gray;
            context.PixelMemory.Span[i + 2] = gray;
        }

        return await Task.FromResult(context);
    }
}
