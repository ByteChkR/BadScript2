///<summary>
///	Contains IO Implementation for the BadScript2 Runtime
/// </summary>

namespace BadScript2.IO;

/// <summary>
///     Public interface for the filesystem abstraction of the BadScript Engine
/// </summary>
public static class BadFileSystem
{
    /// <summary>
    ///     File System implementation
    /// </summary>
    private static IFileSystem? s_FileSystem = new BadSystemFileSystem();

    /// <summary>
    ///     File System implementation
    /// </summary>
    /// <exception cref="InvalidOperationException">Gets thrown if the FileSystem is not set</exception>
    public static IFileSystem Instance =>
        s_FileSystem ?? throw new InvalidOperationException("FileSystem is not initialized");

    /// <summary>
    ///     Sets the FileSystem implementation
    /// </summary>
    /// <param name="fileSystem">The FileSystem implementation</param>
    public static void SetFileSystem(IFileSystem fileSystem)
    {
        s_FileSystem = fileSystem;
    }

    /// <summary>
    ///     Wrapper for <see cref="IFileSystem.OpenWrite" /> that creates the file if it does not exist
    /// </summary>
    /// <param name="path">Path of the file</param>
    /// <param name="contents">Contents to be written</param>
    public static void WriteAllText(string path, string contents)
    {
        Instance.WriteAllText(path, contents);
    }

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
    ///     Wrapper for <see cref="IFileSystem.OpenRead" />
    /// </summary>
    /// <param name="path">Path of the file</param>
    public static string ReadAllText(string path)
    {
        return Instance.ReadAllText(path);
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
    ///     Wrapper for <see cref="IFileSystem.OpenRead" />
    /// </summary>
    /// <param name="path">Path of the file</param>
    public static IEnumerable<string> ReadAllLines(string path)
    {
        return Instance.ReadAllLines(path);
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

    /// <summary>
    ///     Wrapper for <see cref="IFileSystem.OpenWrite" /> that creates the file if it does not exist
    /// </summary>
    /// <param name="path">Path of the file</param>
    /// <param name="lines">lines to be written</param>
    public static void WriteAllLines(string path, IEnumerable<string> lines)
    {
        Instance.WriteAllLines(path, lines);
    }
}