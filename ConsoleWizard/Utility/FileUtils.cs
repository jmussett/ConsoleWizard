namespace ConsoleWizard;

internal class FileUtils : AdapterProvider
{
    private FileUtils() {}

    internal static bool TryGetChildDirectories(string parentDirectory, out IEnumerable<string> directories, bool root = false)
    {
        try
        {
            directories = Directory.GetDirectories(parentDirectory).OrderBy(x => x);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            if (root) throw;

            directories = null!;

            Console.Clear();

            Console.WriteLine("Access Denied. Press any key to return to parent directory...");

            _ = Console.ReadKey();

            return false;
        }
    }

    internal static bool TryGetChildFilesAndDirectories(string parentDirectory, string searchPattern, out IEnumerable<string> filesAndDirectories, bool root = false)
    {
        try
        {
            var directories = Directory.GetDirectories(parentDirectory).OrderBy(x => x).ToList();
            var files = Directory.GetFiles(parentDirectory, searchPattern).OrderBy(x => x).ToList();
            directories.AddRange(files);
            filesAndDirectories = directories;
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            if (root) throw;

            filesAndDirectories = null!;

            Console.Clear();

            Console.WriteLine("Access Denied. Press any key to return to parent directory...");

            _ = Console.ReadKey();

            return false;
        }
    }
}
