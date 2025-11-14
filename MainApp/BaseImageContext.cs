// ------------------------------------------------------------------
// BaseImageContext (Shared Image Info)
//
// What it does:
// This is a base (abstract) class that stores common information
// shared by different types of image context classes (like file name).
//
// Why it's useful:
// - Your app uses multiple kinds of image contexts:
//     • `BitmapImageContext` (for unoptimized, high-level image work)
//     • `ByteArrayImageContext` (for optimized, low-level memory access)
// - Both of those need to store at least the name of the image file.
// - Instead of repeating that code in every class, this base class
//   provides a shared property and constructor.
//
// Key property:
// ● `FileName`
//   - The name of the image file (without the full path).
//   - Used when saving the edited image back to disk.
//
// How it works:
// - This class is marked as `abstract`, which means it can’t be used directly.
// - Other classes (like `BitmapImageContext`) inherit from it
//   and automatically gain the `FileName` property.
//
// Example:
// - If you're editing "cat.png", the `FileName` would be "cat.png".
// - The program uses this when writing output like "cat_gray.png".
//
// Why it's clean and scalable:
// Avoids repeating shared logic across multiple image context classes.
// Keeps the code organized and easier to extend later.
// Supports both optimized and unoptimized pipelines with a common structure.
//
// ------------------------------------------------------------------


namespace MainApp;

public abstract class BaseImageContext 
{   
    public string FileName { get; set; }

    protected BaseImageContext(string fileName)
    {
        FileName = fileName;
    }
}