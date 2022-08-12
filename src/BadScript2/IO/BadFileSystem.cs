namespace BadScript2.IO;

public static class BadFileSystem
{
    private static IFileSystem? s_FileSystem = new BadSystemFileSystem();
    public static IFileSystem Instance => s_FileSystem ?? throw new InvalidOperationException("FileSystem is not initialized");

    public static void SetFileSystem(IFileSystem fileSystem)
    {
        s_FileSystem = fileSystem;
    }

    public static void WriteAllText(string path, string contents)
    {
        using Stream s = Instance.OpenWrite(path, BadWriteMode.CreateNew);
        using StreamWriter sw = new StreamWriter(s);
        sw.Write(contents);
    }

    public static string ReadAllText(string path)
    {
        using Stream s = Instance.OpenRead(path);
        using StreamReader sw = new StreamReader(s);

        return sw.ReadToEnd();
    }

    public static IEnumerable<string> ReadAllLines(string path)
    {
        using Stream s = Instance.OpenRead(path);
        using StreamReader sw = new StreamReader(s);


        while (sw.EndOfStream)
        {
            yield return sw.ReadLine() ?? "";
        }
    }

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