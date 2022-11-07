using BadScript2.Common.Logging;

namespace BadScript2.IO.Virtual;

/// <summary>
///     Represents a Virtual File System Directory Entry
/// </summary>
public class BadVirtualDirectory : BadVirtualNode
{
    /// <summary>
    ///     Subdirectories
    /// </summary>
    private readonly List<BadVirtualDirectory> m_Directories = new List<BadVirtualDirectory>();

    /// <summary>
    ///     Files inside this directory
    /// </summary>
    private readonly List<BadVirtualFile> m_Files = new List<BadVirtualFile>();

    /// <summary>
    ///     Creates a new Virtual Directory
    /// </summary>
    /// <param name="name">Name of the Directory</param>
    /// <param name="parent">Parent Directory</param>
    protected BadVirtualDirectory(string name, BadVirtualDirectory? parent) : base(name, parent) { }

    /// <summary>
    ///     Returns true if the Directory contains no files and no files in the subdirectories
    /// </summary>
    public bool IsEmpty => m_Files.Count == 0 && m_Directories.All(x => x.IsEmpty);


    /// <summary>
    ///     Subdirectories
    /// </summary>
    public IEnumerable<BadVirtualDirectory> Directories => m_Directories;

    /// <summary>
    ///     Files inside this directory
    /// </summary>
    public IEnumerable<BadVirtualFile> Files => m_Files;

    /// <summary>
    ///     The Absolute Path of this Directory
    /// </summary>
    public string AbsolutePath => Parent?.AbsolutePath + Name + "/";

    /// <summary>
    ///     Returns true if the Directory contains a at least one subdirectory or file.
    /// </summary>
    public override bool HasChildren => m_Directories.Count != 0 || m_Files.Count != 0;

    /// <summary>
    ///     Returns a list of all children of this Directory
    /// </summary>
    public override IEnumerable<BadVirtualNode> Children => m_Directories.Concat(m_Files.Cast<BadVirtualNode>());

    /// <summary>
    ///     Returns true if the file with the specified name exist in this directory
    /// </summary>
    /// <param name="name">File name</param>
    /// <returns>True if the file exists</returns>
    public bool FileExists(string name)
    {
        return m_Files.Any(x => x.Name == name);
    }

    /// <summary>
    ///     Returns true if the directory with the specified name exist in this directory
    /// </summary>
    /// <param name="name">Directory name</param>
    /// <returns>True if the directory exists</returns>
    public bool DirectoryExists(string name)
    {
        return m_Directories.Any(x => x.Name == name);
    }

    /// <summary>
    ///     Deletes a subdirectory with the specified name
    /// </summary>
    /// <param name="name">Directory Name</param>
    /// <param name="recursive">If true, Delete all child entries</param>
    /// <exception cref="IOException">Gets Thrown if the directory is not empty and recursive is set to false.</exception>
    public void DeleteDirectory(string name, bool recursive = true)
    {
        BadLogger.Log($"Deleting Directory {AbsolutePath}{name}", "BFS");
        BadVirtualDirectory? dir = m_Directories.FirstOrDefault(x => x.Name == name);
        if (dir == null)
        {
            return;
        }

        if (!dir.IsEmpty && !recursive)
        {
            throw new IOException("Directory is not empty");
        }

        m_Directories.Remove(dir);
    }

    /// <summary>
    ///     Deletes a file from the current directory
    /// </summary>
    /// <param name="name">File Name</param>
    public void DeleteFile(string name)
    {
        BadLogger.Log($"Deleting File {AbsolutePath}{name}", "BFS");
        m_Files.RemoveAll(x => x.Name == name);
    }

    /// <summary>
    ///     Returns the existing directory or Creates a new directory with the specified name
    /// </summary>
    /// <param name="name">Directory Name</param>
    /// <returns>The Directory Object that was created</returns>
    public BadVirtualDirectory CreateDirectory(string name)
    {
        BadLogger.Log($"Create Directory {AbsolutePath}{name}", "BFS");
        BadVirtualDirectory dir;
        if (!DirectoryExists(name))
        {
            dir = new BadVirtualDirectory(name, this);
            m_Directories.Add(dir);
        }
        else
        {
            dir = GetDirectory(name);
        }

        return dir;
    }

    /// <summary>
    ///     Returns an existing directory
    /// </summary>
    /// <param name="name">Name of the Directory</param>
    /// <returns>The Directory</returns>
    /// <exception cref="Exception">Gets thrown if the directory is not found.</exception>
    public BadVirtualDirectory GetDirectory(string name)
    {
        return m_Directories.FirstOrDefault(x => x.Name == name) ?? throw new Exception($"Directory '{name}' not found in {this}");
    }

    /// <summary>
    ///     Returns the existing file or Creates a new file with the specified name
    /// </summary>
    /// <param name="name">file Name</param>
    /// <returns>The File Object that was created</returns>
    public BadVirtualFile CreateFile(string name)
    {
        BadLogger.Log($"Creating File {AbsolutePath}{name}", "BFS");
        if (FileExists(name))
        {
            throw new Exception($"File {name} already exists in {this}");
        }

        BadVirtualFile file = new BadVirtualFile(name, this);
        m_Files.Add(file);

        return file;
    }

    /// <summary>
    ///     Returns an existing file or creates a new file with the specified name
    /// </summary>
    /// <param name="name">File Name</param>
    /// <returns>File Object</returns>
    public BadVirtualFile GetOrCreateFile(string name)
    {
        return FileExists(name) ? GetFile(name) : CreateFile(name);
    }

    /// <summary>
    ///     Returns an existing File
    /// </summary>
    /// <param name="name">File Name</param>
    /// <returns>File Object</returns>
    /// <exception cref="Exception">Gets raised if the file does not exist</exception>
    public BadVirtualFile GetFile(string name)
    {
        return m_Files.FirstOrDefault(x => x.Name == name) ?? throw new Exception($"File '{name}' not found in {this}");
    }
}