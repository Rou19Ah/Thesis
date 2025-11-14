// -------------------------------------------------------------
// ColorMapOperation (Optimized Version)
//
// What it does:
// This class lets you find a specific color in an image (red, green, or blue)
// and change it into another color (also red, green, or blue).
//
// How it works (in simple terms):
// - The program reads the image pixel-by-pixel.
// - For each pixel, it checks if the color matches your selected target color.
//   For example, it might look for “mostly red” pixels.
// - If the pixel matches, it changes that pixel to your chosen replacement color.
//
// What makes it fast:
// - Instead of using slow methods like GetPixel/SetPixel, it reads the raw
//   image memory directly using “unsafe” pointer access.
// - This is like opening up the image as a big block of memory and quickly
//   walking through it to modify the color data directly.
// - It uses a method called LockBits to freeze the image in memory
//   so it can safely work on the raw bytes.
// - Each pixel is made up of 3 bytes: Blue, Green, Red (BGR format).
//
// Example:
//   You choose to replace RED with BLUE.
//   It finds pixels that are very red (red high, green/blue low)
//   and changes them to blue (red = 0, green = 0, blue = 255).
//
// This method is optimized for speed and is used in the optimized image pipeline.
//
// -------------------------------------------------------------

using MainApp;
using System.Drawing;
using System.Drawing.Imaging;

namespace OptimizedApp;

public class ColorMapOperation : IOperation
{
    private int _targetIndex = 1;
    private int _replaceIndex = 2;

    public void Configure()
    {
        Console.WriteLine("ColorMapOperation: Choose a color to detect and what to replace it with.");

        Console.WriteLine("Colors:\n1 = Red\n2 = Green\n3 = Blue");

        Console.Write("Enter number of color to detect (1-3): ");
        _targetIndex = ParseColorIndex(Console.ReadLine());

        Console.Write("Enter number of color to replace with (1-3): ");
        _replaceIndex = ParseColorIndex(Console.ReadLine());
    }

    private static int ParseColorIndex(string? input)
    {
        return input switch
        {
            "1" => 1,
            "2" => 2,
            "3" => 3,
            _ => 1    
        };
    }

    public unsafe Task<ByteArrayImageContext> ApplyAsync(ByteArrayImageContext context)
    {
        using var bmp = context.ToBitmap();

        Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
        BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

        try
        {
            byte* ptr = (byte*)data.Scan0;
            int stride = data.Stride;

            for (int y = 0; y < data.Height; y++)
            {
                byte* row = ptr + y * stride;
                for (int x = 0; x < data.Width; x++)
                {
                    byte* px = row + x * 3;

                    byte b = px[0], g = px[1], r = px[2];

                    if (IsMatch(r, g, b, _targetIndex))
                    {
                        ApplyColor(px, _replaceIndex);
                    }
                }
            }
        }
        finally
        {
            bmp.UnlockBits(data);
        }

        context.ReplaceWith(bmp);
        return Task.FromResult(context);
    }

    private static bool IsMatch(byte r, byte g, byte b, int index)
    {
        return index switch
        {
            1 => r > 150 && g < 100 && b < 100, 
            2 => g > 150 && r < 100 && b < 100, 
            3 => b > 150 && r < 100 && g < 100, 
            _ => false
        };
    }

    private unsafe void ApplyColor(byte* px, int index)
    {
        px[0] = px[1] = px[2] = 0;

        switch (index)
        {
            case 1: px[2] = 255; break;
            case 2: px[1] = 255; break;
            case 3: px[0] = 255; break;
        }
    }
}
