namespace BadScript2.IO;

/// <summary>
/// Public interface for the filesystem abstraction of the BadScript Engine
/// </summary>
public static class BadFileSystem
{
    /// <summary>
    /// File System implementation
    /// </summary>
    private static IFileSystem? s_FileSystem = new BadSystemFileSystem();
    /// <summary>
    /// File System implementation
    /// </summary>
    /// <exception cref="InvalidOperationException">Gets thrown if the FileSystem is not set</exception>
    public static IFileSystem Instance => s_FileSystem ?? throw new InvalidOperationException("FileSystem is not initialized");

    /// <summary>
    /// Sets the FileSystem implementation
    /// </summary>
    /// <param name="fileSystem">The FileSystem implementation</param>
    public static void SetFileSystem(IFileSystem fileSystem)
    {
        s_FileSystem = fileSystem;
    }

    /// <summary>
    /// Wrapper for <see cref="IFileSystem.OpenWrite" /> that creates the file if it does not exist 
    /// </summary>
    /// <param name="path">Path of the file</param>
    /// <param name="contents">Contents to be written</param>
    public static void WriteAllText(string path, string contents)
    {
        using Stream s = Instance.OpenWrite(path, BadWriteMode.CreateNew);
        using StreamWriter sw = new StreamWriter(s);
        sw.Write(contents);
    }

    /// <summary>
    /// Wrapper for <see cref="IFileSystem.OpenRead" />
    /// </summary>
    /// <param name="path">Path of the file</param>
    public static string ReadAllText(string path)
    {
        using Stream s = Instance.OpenRead(path);
        using StreamReader sw = new StreamReader(s);

        return sw.ReadToEnd();
    }

    /// <summary>
    /// Wrapper for <see cref="IFileSystem.OpenRead" />
    /// </summary>
    /// <param name="path">Path of the file</param>
    public static IEnumerable<string> ReadAllLines(string path)
    {
        using Stream s = Instance.OpenRead(path);
        using StreamReader sw = new StreamReader(s);


        while (!sw.EndOfStream)
        {
            yield return sw.ReadLine() ?? "";
        }
    }
    /// <summary>
    /// Wrapper for <see cref="IFileSystem.OpenWrite" /> that creates the file if it does not exist 
    /// </summary>
    /// <param name="path">Path of the file</param>
    /// <param name="lines">lines to be written</param>
    public static void WriteAllLines(string path, IEnumerable<string> lines)
    {
        using Stream s = Instance.OpenWrite(path, BadWriteMode.CreateNew);
        using StreamWriter sw = new StreamWriter(s);
        foreach (string line in lines)
        {
            sw.WriteLine(line);
        }
    }
}