
using Thinktecture;
using Thinktecture.Adapters;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

namespace System;

/// <summary>
/// Provides adapter access for sub-class code use, and adapter configuration for external use.
/// </summary>
public abstract class AdapterProvider
{
    protected static IConsole Console => ConsoleWizard.Adapters.Console;
    protected static IDirectory Directory => ConsoleWizard.Adapters.Directory;
    protected static IFile File => ConsoleWizard.Adapters.File;
}

public partial class ConsoleWizard
{
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

