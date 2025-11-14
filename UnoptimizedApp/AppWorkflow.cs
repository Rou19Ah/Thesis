using MainApp;

namespace UnoptimizedApp;

public static class AppWorkflow
{
    public static void Run(string[] imagePaths, string outputDir)
    {
        Console.WriteLine("Starting image processing...");

        Console.WriteLine("Available operations:");
        Console.WriteLine("1 = Rename");
        Console.WriteLine("2 = Grayscale");
        Console.WriteLine("3 = Resize");
        Console.WriteLine("4 = Color Map");
        Console.WriteLine("5 = Rotate");
        Console.WriteLine("6 = Crop");
        Console.Write("Choose operations (e.g., 1 4): ");
        var opCodes = Console.ReadLine()?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];

        Console.Write("Confirm selection? (y/n): ");
        if (!Console.ReadLine()?.Trim().ToLower().StartsWith("y") ?? true)
            return;

        var operations = new List<IUnoptimizedOperation>();

        foreach (var code in opCodes)
        {
            IUnoptimizedOperation? op = code switch
            {
                "1" => new RenameOperation(),
                "2" => new GrayscaleOperation(),
                "3" => new ResizeOperation(),
                "4" => new ColorMapOperation(),
                "5" => new RotateOperation(),
                "6" => new CropOperation(),
                //"3" => new ResizeOperation(),
                _ => null
            };

            if (op != null)
            {
                operations.Add(op);
            }
            else
            {
                Console.WriteLine($"Unknown operation code: {code}");
            }
        }

        if (operations.Count == 0)
        {
            Console.WriteLine("No valid operations selected. Aborting.");
            return;
        }

        foreach (var path in imagePaths)
        {
            var context = new BitmapImageContext(path);

            foreach (var op in operations)
            {
                context = op.Apply(context);
            }

            string outputPath = Path.Combine(outputDir, context.FileName);
            context.Bitmap.Save(outputPath);
        }

        Console.WriteLine($"Processing completed. Output saved to: {outputDir}");
    }
}
