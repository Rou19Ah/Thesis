// ------------------------------------------------------------------
// RenameOperation (Optimized Version)
//
// What it does:
// This class automatically renames each image during processing,
// using a naming pattern chosen by the user.
//
// How it works (in simple terms):
// - Before the image processing starts, the program asks the user
//   how they want to rename the images.
// - There are three options:
//     1. Keep the original filename and add a number (e.g. "photo_1").
//     2. Use a generic name like "IMG_1", "IMG_2", etc.
//     3. Let the user type a custom name (e.g. "Vacation_1").
// - For each image, it builds a new name by inserting the current count number.
// - The extension (.jpg, .png, etc.) is kept the same.
// - The result is saved into the file metadata before saving.
//
// Why this is useful:
// - Makes it easy to organize large batches of files.
// - Ensures no two files have the same name.
// - Gives the user control over naming format.
//
// Example:
// - Original file: "sunset.png"
// - If user chooses option 2, the renamed file becomes: "IMG_1.png"
// - If user types "Holiday" in option 3, the output will be: "Holiday_1.png"
//
// Why it’s optimized:
// - The renaming logic is handled once per image, with minimal overhead.
// - It avoids any slow file system operations — only updates in memory.
//
// ------------------------------------------------------------------


using MainApp;

namespace OptimizedApp;

public class RenameOperation : IOperation
{
    private string template = "{originalName}_{index}";
    private string customText = "Image";
    private int counter = 1;

    public void Configure()
    {
        Console.WriteLine("Choose a rename template:");
        Console.WriteLine("  1. originalName_index");
        Console.WriteLine("  2. IMG_index");
        Console.WriteLine("  3. customText_index");

        Console.Write("Your choice (1–3): ");
        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                template = "{originalName}_{index}";
                break;

            case "2":
                template = "IMG_{index}";
                break;

            case "3":
                Console.Write("Enter custom base name: ");
                customText = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(customText))
                {
                    customText = "Image";
                }
                template = customText + "_{index}";
                break;

            default:
                Console.WriteLine("Invalid choice. Defaulting to originalName_index.");
                template = "{originalName}_{index}";
                break;
        }
    }

    public async Task<ByteArrayImageContext> ApplyAsync(ByteArrayImageContext context)
    {
        string originalName = Path.GetFileNameWithoutExtension(context.FileName);
        string extension = Path.GetExtension(context.FileName);

        string newName = template
            .Replace("{originalName}", originalName)
            .Replace("{index}", counter.ToString());

        counter++;

        context.FileName = newName + extension;
        return await Task.FromResult(context);
    }
}
