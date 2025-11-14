// Why it's "Unoptimized":
// - It uses a simple method called `GetPixel` to read the color of each pixel,
//   and `SetPixel` to write a new color.
// - This method is very easy to understand but *slow* for big images,
//   because it processes one pixel at a time through high-level calls.

using System.Drawing;
using MainApp;

namespace UnoptimizedApp;

public class ColorMapOperation : IUnoptimizedOperation
{
    private int _targetIndex = 1;
    private int _replaceIndex = 2;

    public ColorMapOperation()
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

    public BitmapImageContext Apply(BitmapImageContext context)
    {
        Bitmap original = context.Bitmap;
        Bitmap result = new(original.Width, original.Height);

        for (int y = 0; y < original.Height; y++)
        {
            for (int x = 0; x < original.Width; x++)
            {
                var pixel = original.GetPixel(x, y);
                if (IsMatch(pixel, _targetIndex))
                {
                    result.SetPixel(x, y, GetReplaceColor(_replaceIndex));
                }
                else
                {
                    result.SetPixel(x, y, pixel);
                }
            }
        }

        context.Bitmap = result;
        return context;
    }

    private static bool IsMatch(Color pixel, int index)
    {
        return index switch
        {
            1 => pixel.R > 150 && pixel.G < 100 && pixel.B < 100,
            2 => pixel.G > 150 && pixel.R < 100 && pixel.B < 100,
            3 => pixel.B > 150 && pixel.R < 100 && pixel.G < 100,
            _ => false
        };
    }

    private static Color GetReplaceColor(int index)
    {
        return index switch
        {
            1 => Color.Red,
            2 => Color.Green,
            3 => Color.Blue,
            _ => Color.Black
        };
    }
}
