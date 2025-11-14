// ------------------------------------------------------------------
// PipelineController (Optimized Async Image Processor)
//
// What it does:
// This class manages a list of image editing operations (like rotate, crop, etc.),
// and applies them to many images — possibly in parallel — in a safe and memory-aware way.
//
// How it works (in simple terms):
// - You give it a list of image files and a list of editing steps (operations).
// - It processes the images one by one or in parallel (with a limit).
// - After each image is edited, it is saved to the output folder.
// - It also watches the app’s memory usage and forces cleanup (GC) if needed.
//
// Key features:
// Can run multiple images at once — but limits how many to avoid memory overload.
// Applies every operation to each image, in the order you set.
// Saves the final edited image with a new name.
// Checks memory after each image — if usage gets too high, it clears memory using GC (Garbage Collector).
//
// What happens behind the scenes:
// - A semaphore is used to *limit how many images are processed at the same time*.
//   For example, if the limit is 2, only 2 images run in parallel.
// - For each image:
//   - A memory-efficient image context (`ByteArrayImageContext`) is created.
//   - All configured operations are applied one after another.
//   - The image is converted back to a `Bitmap` and saved.
// - After each image finishes, it checks how much memory the app is using.
//   - If memory goes above 1GB, it forces garbage collection to clean up unused memory.
//
// Why this version is optimized:
// - Prevents crashes or memory bloat when working with large images.
// - Parallelism gives faster performance, but memory checks prevent overload.
// - Uses smart cleanup only when necessary, so the system isn’t stressed.
//
// Example:
// - 10 images need cropping and grayscale.
// - It processes 2 images at once.
// - When memory use goes over 1GB, it pauses briefly to clear memory before continuing.
//
// ------------------------------------------------------------------


using System.Diagnostics;

namespace MainApp;
public class PipelineController(List<IOperation> operations)
{
    private readonly List<IOperation> _operations = operations;

    public void ConfigureAll()
    {
        foreach (var op in _operations)
            op.Configure();
    }

    public async Task RunAsync(string[] imagePaths, string outputDir, int maxDegreeOfParallelism = 2)
    {
        long memoryLimitBytes = 1_000_000_000; // 1GB
        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

        static long GetProcessMemoryBytes()
        {
            using var proc = Process.GetCurrentProcess();
            return proc.WorkingSet64;
        }

        var tasks = imagePaths.Select(async path =>
        {
            await semaphore.WaitAsync();
            try
            {
                Console.WriteLine($"\nStarting image: {Path.GetFileName(path)}");

                var context = new ByteArrayImageContext(path);

                foreach (var op in _operations)
                {
                    await op.ApplyAsync(context);
                }

                using var finalBitmap = context.ToBitmap();
                string outputPath = Path.Combine(outputDir, context.FileName);
                finalBitmap.Save(outputPath);

                //funny how this works if i simply use dispose method for bitmap or unoptimized app
                //it will avoid memory leak but here it makes memory keep getting reserved more after app is done
                //context.Dispose();

                Console.WriteLine($"Finished image: {Path.GetFileName(path)}");


            }
            finally
            {
                // will help to reduce pressure on GC from spamming unneccassary but
                // as well when passsing threshold it will run GC only saving memory
                // from being reserverd for nothing
                if (GetProcessMemoryBytes() > memoryLimitBytes)
                {
                    Console.WriteLine("Memory threshold exceeded. Triggering GC.");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}
