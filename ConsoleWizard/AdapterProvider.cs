
using Thinktecture;
using Thinktecture.Adapters;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

namespace ConsoleWizard;

/// <summary>
/// Provides adapter access for sub-class code use, and adapter configuration for external use.
/// </summary>
public abstract class AdapterProvider
{
    protected static IConsole Console => Adapters.Console;
    protected static IDirectory Directory => Adapters.Directory;
    protected static IFile File => Adapters.File;

    /// <summary>
    /// A set of adapters that can be used to swap out the underlying implementations.
    /// </summary>
    public static class Adapters
    {
        /// <summary>
        /// The adapter responsible for managing the input and output of console applications.
        /// </summary>
        public static IConsole Console { internal get; set; } = new ConsoleAdapter();

        /// <summary>
        /// The adapter for managing directories and sub-directories in a file system.
        /// </summary>
        public static IDirectory Directory { internal get; set; } = new DirectoryAdapter();

        /// <summary>
        /// The adapter for managing files in a file system.
        /// </summary>
        public static IFile File { internal get; set; } = new FileAdapter();
    }
}
