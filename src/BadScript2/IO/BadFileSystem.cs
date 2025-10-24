///<summary>
///	Contains IO Implementation for the BadScript2 Runtime
/// </summary>

namespace BadScript2.IO;

/// <summary>
///     Extensions for the filesystem abstraction of the BadScript Engine
/// </summary>
public static class BadFileSystemExtensions
{

    /// <summary>
    /// Writes all text to a file
    /// </summary>
    /// <param name="fileSystem">The FileSystem implementation</param>
    /// <param name="path">Path of the file</param>
    /// <param name="contents">Contents to be written</param>
    public static void WriteAllText(this IFileSystem fileSystem, string path, string contents)
    {
        using Stream s = fileSystem.OpenWrite(path, BadWriteMode.CreateNew);
        using StreamWriter sw = new StreamWriter(s);
        sw.Write(contents);
    }

    /// <summary>
    /// Reads all text from a file
    /// </summary>
    /// <param name="fileSystem">The FileSystem implementation</param>
    /// <param name="path">Path of the file</param>
    /// <returns>The contents of the file</returns>
    public static string ReadAllText(this IFileSystem fileSystem, string path)
    {
        using Stream s = fileSystem.OpenRead(path);
        using StreamReader sw = new StreamReader(s);

        return sw.ReadToEnd();
    }

    /// <summary>
    /// Reads all lines from a file
    /// </summary>
    /// <param name="fileSystem">The FileSystem implementation</param>
    /// <param name="path">Path of the file</param>
    /// <returns>The contents of the file</returns>
    public static IEnumerable<string> ReadAllLines(this IFileSystem fileSystem, string path)
    {
        using Stream s = fileSystem.OpenRead(path);
        using StreamReader sw = new StreamReader(s);

        while (!sw.EndOfStream)
        {
            yield return sw.ReadLine() ?? "";
        }
    }
    
    /// <summary>
    /// Writes all lines to a file
    /// </summary>
    /// <param name="fileSystem">The FileSystem implementation</param>
    /// <param name="path">Path of the file</param>
    /// <param name="lines">Contents to be written</param>
    public static void WriteAllLines(this IFileSystem fileSystem, string path, IEnumerable<string> lines)
    {
        using Stream s = fileSystem.OpenWrite(path, BadWriteMode.CreateNew);
        using StreamWriter sw = new StreamWriter(s);

        foreach (string line in lines)
        {
            sw.WriteLine(line);
        }
    }
}