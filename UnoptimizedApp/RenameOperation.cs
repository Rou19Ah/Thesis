// Why this is Unoptimized:
// - The logic itself is fast and not performance-heavy, but it operates
//   inside the unoptimized pipeline, which uses slower image handling (Bitmap).
// - It uses basic string replacement for renaming, which is fine for small batches,
//   but lacks features like batch-safe uniqueness, collision detection, or async processing.
// - In optimized systems, file naming might be integrated with metadata,
//   hashing, or parallel-safe strategies for large-scale image sets.

using MainApp;

namespace UnoptimizedApp;

public class RenameOperation : IUnoptimizedOperation
{
    private string template = "{original}_{index}";
    private int counter = 1;

    public RenameOperation()
    {
        Console.WriteLine("Choose rename template:");
        Console.WriteLine("1. original_index");
        Console.WriteLine("2. IMG_index");
        Console.WriteLine("3. customText_index");

        Console.Write("Your choice (1–3): ");
        string? choice = Console.ReadLine();

        if (choice == "2")
            template = "IMG_{index}";
        else if (choice == "3")
        {
            Console.Write("Enter base name: ");
            string? baseName = Console.ReadLine();
            template = (baseName ?? "image") + "_{index}";
        }
    }

    public BitmapImageContext Apply(BitmapImageContext context)
    {
        string original = Path.GetFileNameWithoutExtension(context.FileName);
        string extension = Path.GetExtension(context.FileName);

        string newName = template
            .Replace("{original}", original)
            .Replace("{index}", counter.ToString());

        context.FileName = newName + extension;
        counter++;
        return context;
    }
}
