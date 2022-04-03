namespace System;

public partial class ConsoleWizard
{
    /// <summary>
    /// Waits for the user to navigate and search for a directory in a parent directory.
    /// </summary>
    /// <param name="parentDirectory">The parent directory to search in.</param>
    /// <returns>The path for the selected directory.</returns>
    public static string? SearchForDirectory(string parentDirectory)
    {
        return SearchFromTree(
            new List<string> { parentDirectory },
            (child, all) => FileUtils.TryGetChildDirectories(child, out var childDirectories) ? childDirectories : all,
            (parent, all) =>
            {
                var parentDirectory = parent == null ? null : Directory.GetParent(parent);
                return parentDirectory?.GetDirectories().Select(x => x.FullName) ?? new List<string?> { parent };
            },
            (search, all) => all.Where(x => x.ToLower().Contains(search.ToLower())),
            x => true,
            (directory, i) => directory,
            200
        );
    }

    /// <summary>
    /// Waits for the user to navigate and search for a file in a parent directory.
    /// </summary>
    /// <param name="parentDirectory">The parent directory to search in.</param>
    /// <param name="searchPattern">The pattern that the file names need to be matched by.</param>
    /// <returns>The path for the selected file.</returns>
    public static string? SearchForFile(string parentDirectory, string searchPattern = "*")
    {
        _ = FileUtils.TryGetChildFilesAndDirectories(parentDirectory, searchPattern, out var filesAndDirectories, true);

        return SearchFromTree(
            filesAndDirectories,
            (child, all) =>
            {
                if (Directory.Exists(child))
                    return FileUtils.TryGetChildFilesAndDirectories(child, searchPattern, out var filesAndDirectries) ? filesAndDirectries : all;

                return all;

            },
            (parent, all) =>
            {
                var parentDirectory = parent == null ? null : Directory.GetParent(parent);

                if (parentDirectory == null) return all;

                return FileUtils.TryGetChildFilesAndDirectories(parentDirectory.FullName, searchPattern, out var filesAndDirectries) ? filesAndDirectries : all;
            },
            (search, all) => all.Where(x => x.ToLower().Contains(search.ToLower())),
            value => File.Exists(value),
            (directory, i) => directory,
            200
        );
    }
}