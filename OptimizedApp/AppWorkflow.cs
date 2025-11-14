using MainApp;

namespace OptimizedApp;

public static class AppWorkflow
{
    public static async Task RunWorkflow(string[] imagePaths, string outputDir)
    {
        Console.WriteLine("Starting pipeline...");

        Console.WriteLine("Available operations:");
        Console.WriteLine("1 = Rename");
        Console.WriteLine("2 = Grayscale");
        Console.WriteLine("3 = Resize");
        Console.WriteLine("4 = Color Map");
        Console.WriteLine("5 = Rotate");
        Console.WriteLine("6 = Crop");
        Console.Write("Choose operations (e.g., 1 7 5): ");
        var opCodes = Console.ReadLine()?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];

        Console.Write("Confirm selection? (y/n): ");
        if (!Console.ReadLine().Trim().ToLower().StartsWith("y"))
            return;

        var operations = new List<IOperation>();
        foreach (var code in opCodes)
        {
            IOperation? op = code switch
            {
                "1" => new RenameOperation(),
                "2" => new GrayscaleOperation(),
                "3" => new ResizeOperation(),
                "4" => new ColorMapOperation(),
                "5" => new RotateOperation(),
                "6" => new CropOperation(),
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
            Console.WriteLine("No valid operations selected.");
            return;
        }

        Console.Write("How many images to process in parallel? (Default 2): ");
        var input = Console.ReadLine()?.Trim();
        int degree = int.TryParse(input, out var val) && val > 0 ? val : 2;

        var pipeline = new PipelineController(operations);
        pipeline.ConfigureAll();
        await pipeline.RunAsync(imagePaths, outputDir, degree);

        Console.WriteLine($"Pipeline completed. Output at: {outputDir}");
    }
}