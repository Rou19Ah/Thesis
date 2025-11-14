// ------------------------------------------------------------------
// IOperation Interface
//
// What it does:
// This defines the basic "contract" for any image operation used in the app.
// All operations (like Rotate, Resize, Grayscale, etc.) must follow this structure.
//
// In simple terms:
// - Think of this like a checklist for building image tools.
// - Any class that wants to be used as an operation must provide:
//     1. A `Configure()` method — for asking the user what settings to use.
//     2. An `ApplyAsync(...)` method — for actually changing the image.
//
// Why this is useful:
// - It allows the pipeline to treat all operations the same way,
//   even if they're doing completely different things (like rotating or renaming).
// - It makes the system flexible — you can add new operations
//   without changing the pipeline code.
//
// Breakdown of the methods:
// ● `void Configure()`
//     - Called once before processing starts.
//     - Used to ask the user for input (e.g., angle, size, filename pattern).
//
// ● `Task<ByteArrayImageContext> ApplyAsync(ByteArrayImageContext context)`
//     - Called for each image.
//     - Applies the operation and returns the updated image context.
//     - Runs asynchronously to support high-performance processing.
//
// Example:
// - A `GrayscaleOperation` class implements this interface.
//   - In `Configure()`, it might say "No setup needed."
//   - In `ApplyAsync()`, it turns each pixel into grayscale.
//
// ------------------------------------------------------------------

namespace MainApp;

public interface IOperation
{
    void Configure();
    Task<ByteArrayImageContext> ApplyAsync(ByteArrayImageContext context);
}
