using BadScript2.IO;

namespace BadScript2.Interop.IO;

[BadInteropApi("Path", true)]
internal partial class BadPathApi
{
    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem m_FileSystem = BadFileSystem.Instance;

    public BadPathApi(IFileSystem fileSystem) : this()
    {
        m_FileSystem = fileSystem;
    }

    [BadMethod(description: "Returns the file name and extension of the specified path string.")]
    [return: BadReturn("The characters after the last directory character in path.")]
    private string GetFileName([BadParameter(description: "The Path to get the file name from.")] string path)
    {
        return Path.GetFileName(path);
    }

    [BadMethod(description: "Returns the file name of the specified path string without the extension")]
    [return: BadReturn("The string returned by GetFileName(string), minus the last period (.) and all characters following it")]
    private string GetFileNameWithoutExtension([BadParameter(description: "The Path to get the file name from.")] string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    [BadMethod(description: "Returns the directory information for the specified path string")]
    [return: BadReturn("Directory information for path, or null if path denotes a root directory or is null. Returns Empty if path does not contain directory information.")]
    private string? GetDirectoryName([BadParameter(description: "The path of a file or directory.")] string path)
    {
        return Path.GetDirectoryName(path);
    }

    [BadMethod(description: "Returns the extension of the specified path string.")]
    [return: BadReturn("The extension of the specified path (including the period).")]
    private string GetExtension([BadParameter(description: "The path of a file or directory.")] string path)
    {
        return Path.GetExtension(path);
    }

    [BadMethod(description: "Returns the absolute path for the specified path string.")]
    [return: BadReturn("The fully qualified location of path, such as \"C:\\MyFile.txt\" or \"/home/user/myFolder\".")]
    private string GetFullPath([BadParameter(description: "The path of a file or directory.")] string path)
    {
        return m_FileSystem.GetFullPath(path);
    }

    [BadMethod(description: "Returns the Startup Directory.")]
    [return: BadReturn("The Startup Directory")]
    private string GetStartupPath()
    {
        return m_FileSystem.GetStartupDirectory();
    }

    [BadMethod(description: "Changes the extension of a path string.")]
    [return: BadReturn("The modified path information.")]
    private string ChangeExtension(
        [BadParameter(description: "The path information to modify.")]
        string path,
        [BadParameter(description: "The new extension (with or without a leading period). Specify null to remove an existing extension from path")]
        string extension)
    {
        return Path.ChangeExtension(path, extension);
    }

    [BadMethod(description: "Combines two or more strings into a path.")]
    [return: BadReturn("The combined path.")]
    private string Combine([BadParameter(description: "The Path Parts")] params string[] parts)
    {
        return Path.Combine(parts);
    }
}