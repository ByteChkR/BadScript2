using BadScript2.IO;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.IO;

[BadInteropApi("Directory")]
internal partial class BadDirectoryApi
{
    
    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem m_FileSystem;
    public BadDirectoryApi(IFileSystem fileSystem) : this()
    {
        m_FileSystem = fileSystem;
    }
    
    [BadMethod(description:"Creates all directories and subdirectories in the specified path.")]
    private void CreateDirectory([BadParameter(description:"The directory to create.")] string path)
    {
        m_FileSystem.CreateDirectory(path);
    }
    
    [BadMethod(description:"Deletes the specified directory and, if indicated, any subdirectories in the directory.")]
    [return: BadReturn("True if the directory exists; otherwise, false.")]
    private bool Exists([BadParameter(description: "The Path to check")] string path)
    {
        return m_FileSystem.Exists(path) && m_FileSystem.IsDirectory(path);
    }
    
    [BadMethod(description:"Deletes the specified directory and, if indicated, any subdirectories in the directory.")]
    private void Delete([BadParameter(description: "The Path to delete")] string path, [BadParameter(description: "If true, the directory will be deleted recursively")] bool recursive)
    {
        m_FileSystem.DeleteDirectory(path, recursive);
    }
    
    [BadMethod(description:"Returns the Current Working Directory.")]
    [return: BadReturn("The Current Working Directory")]
    private string GetCurrentDirectory() => m_FileSystem.GetCurrentDirectory();
    
    [BadMethod(description:"Sets the Current Working Directory.")]
    private void SetCurrentDirectory([BadParameter(description:"The Path to set as the Current Working Directory.")] string path)
    {
        m_FileSystem.SetCurrentDirectory(path);
    }
    
    [BadMethod(description:"Returns the startup directory")]
    [return: BadReturn("The startup directory")]
    private string GetStartupDirectory()
    {
        return m_FileSystem.GetStartupDirectory();
    }
    
    [BadMethod(description:"Returns the directories in the specified directory.")]
    [return: BadReturn("An array of directories in the specified directory.")]
    private BadArray GetDirectories([BadParameter(description:"The Path to get the directories from.")] string path, [BadParameter(description:"If true, the search will return all subdirectories recursively")] bool recursive = false)
    {
        return new BadArray(m_FileSystem.GetDirectories(path, recursive).Select(x => (BadObject)x).ToList());
    }
    
    [BadMethod(description:"Returns the files in the specified directory.")]
    [return: BadReturn("An array of files in the specified directory.")]
    private BadArray GetFiles([BadParameter(description:"The Path to get the files from.")] string path, [BadParameter(description:"The search pattern.")] string searchPattern = "", [BadParameter(description:"If true, the search will return all subdirectories recursively")] bool recursive = false)
    {
        return new BadArray(m_FileSystem.GetFiles(path, searchPattern, recursive).Select(x => (BadObject)x).ToList());
    }
    
    [BadMethod(description:"Moves a specified file to a new location, providing the option to specify a new file name.")]
    private void Move([BadParameter(description: "The Path of the file to move")] string source, [BadParameter(description: "The Destination Path")] string destination, [BadParameter(description: "If true, allows an existing file to be overwritten; otherwise, false.")] bool overwrite)
    {
        m_FileSystem.Move(source, destination, overwrite);
    }
    
    [BadMethod(description:"Copies a specified file to a new location, providing the option to specify a new file name.")]
    private void Copy([BadParameter(description: "The Path of the file to copy")] string source, [BadParameter(description: "The Destination Path")] string destination, [BadParameter(description: "If true, allows an existing file to be overwritten; otherwise, false.")] bool overwrite)
    {
        m_FileSystem.Copy(source, destination, overwrite);
    }
}