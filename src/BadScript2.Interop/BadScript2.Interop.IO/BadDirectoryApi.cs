using BadScript2.IO;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.IO;

/// <summary>
/// Implements the "IO.Directory" API
/// </summary>
[BadInteropApi("Directory", true)]
internal partial class BadDirectoryApi
{
    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem m_FileSystem;

    /// <summary>
    /// Creates a new API Instance
    /// </summary>
    /// <param name="fileSystem">File System Instance to use</param>
    public BadDirectoryApi(IFileSystem fileSystem) : this()
    {
        m_FileSystem = fileSystem;
    }

    /// <summary>
    /// Creates all directories and subdirectories in the specified path, unless they already exist.
    /// </summary>
    /// <param name="path">The directory to create.</param>
    [BadMethod(description: "Creates all directories and subdirectories in the specified path.")]
    private void CreateDirectory([BadParameter(description: "The directory to create.")] string path)
    {
        m_FileSystem.CreateDirectory(path);
    }

    /// <summary>
    /// Tests if the path points to an existing directory.
    /// </summary>
    /// <param name="path">The Path to check</param>
    /// <returns>True if the directory exists; otherwise, false.</returns>
    [BadMethod(description: "Tests if the path points to an existing directory.")]
    [return: BadReturn("True if the directory exists; otherwise, false.")]
    private bool Exists([BadParameter(description: "The Path to check")] string path)
    {
        return m_FileSystem.Exists(path) && m_FileSystem.IsDirectory(path);
    }

    /// <summary>
    /// Deletes the specified directory and, if indicated, any subdirectories in the directory.
    /// </summary>
    /// <param name="path">The Path to delete</param>
    /// <param name="recursive">If true, the directory will be deleted recursively</param>
    [BadMethod(description: "Deletes the specified directory and, if indicated, any subdirectories in the directory.")]
    private void Delete([BadParameter(description: "The Path to delete")] string path,
                        [BadParameter(description: "If true, the directory will be deleted recursively")]
                        bool recursive)
    {
        m_FileSystem.DeleteDirectory(path, recursive);
    }

    /// <summary>
    /// Returns the Current Working Directory.
    /// </summary>
    /// <returns>The Current Working Directory</returns>
    [BadMethod(description: "Returns the Current Working Directory.")]
    [return: BadReturn("The Current Working Directory")]
    private string GetCurrentDirectory()
    {
        return m_FileSystem.GetCurrentDirectory();
    }

    /// <summary>
    /// Sets the Current Working Directory.
    /// </summary>
    /// <param name="path">The Path to set as the Current Working Directory</param>
    [BadMethod(description: "Sets the Current Working Directory.")]
    private void SetCurrentDirectory(
        [BadParameter(description: "The Path to set as the Current Working Directory.")] string path)
    {
        m_FileSystem.SetCurrentDirectory(path);
    }

    /// <summary>
    /// Returns the startup directory.
    /// </summary>
    /// <returns>The startup directory</returns>
    [BadMethod(description: "Returns the startup directory")]
    [return: BadReturn("The startup directory")]
    private string GetStartupDirectory()
    {
        return m_FileSystem.GetStartupDirectory();
    }

    /// <summary>
    /// Returns the directories in the specified directory.
    /// </summary>
    /// <param name="path">The Path to get the directories from.</param>
    /// <param name="recursive">If true, the search will return all subdirectories recursively</param>
    /// <returns>An array of directories in the specified directory.</returns>
    [BadMethod(description: "Returns the directories in the specified directory.")]
    [return: BadReturn("An array of directories in the specified directory.")]
    private BadArray GetDirectories([BadParameter(description: "The Path to get the directories from.")] string path,
                                    [BadParameter(description:
                                                     "If true, the search will return all subdirectories recursively"
                                                 )]
                                    bool recursive = false)
    {
        return new BadArray(m_FileSystem.GetDirectories(path, recursive)
                                        .Select(x => (BadObject)x)
                                        .ToList()
                           );
    }

    /// <summary>
    /// Returns the files in the specified directory.
    /// </summary>
    /// <param name="path">The Path to get the files from.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <param name="recursive">If true, the search will return all files recursively</param>
    /// <returns>An array of files in the specified directory.</returns>
    [BadMethod(description: "Returns the files in the specified directory.")]
    [return: BadReturn("An array of files in the specified directory.")]
    private BadArray GetFiles([BadParameter(description: "The Path to get the files from.")] string path,
                              [BadParameter(description: "The search pattern.")]
                              string searchPattern = "",
                              [BadParameter(description:
                                               "If true, the search will return all files recursively"
                                           )]
                              bool recursive = false)
    {
        return new BadArray(m_FileSystem.GetFiles(path, searchPattern, recursive)
                                        .Select(x => (BadObject)x)
                                        .ToList()
                           );
    }

    /// <summary>
    /// Moves a specified file to a new location, providing the option to specify a new file name.
    /// </summary>
    /// <param name="source">The Path of the file to move</param>
    /// <param name="destination">The Destination Path</param>
    /// <param name="overwrite">If true, allows an existing file to be overwritten; otherwise, false.</param>
    [BadMethod(description: "Moves a specified file to a new location, providing the option to specify a new file name."
              )]
    private void Move([BadParameter(description: "The Path of the file to move")] string source,
                      [BadParameter(description: "The Destination Path")]
                      string destination,
                      [BadParameter(description: "If true, allows an existing file to be overwritten; otherwise, false."
                                   )]
                      bool overwrite = false)
    {
        m_FileSystem.Move(source, destination, overwrite);
    }

    /// <summary>
    /// Copies a specified file to a new location, providing the option to specify a new file name.
    /// </summary>
    /// <param name="source">The Path of the file to copy</param>
    /// <param name="destination">The Destination Path</param>
    /// <param name="overwrite">If true, allows an existing file to be overwritten; otherwise, false.</param>
    [BadMethod(description:
                  "Copies a specified file to a new location, providing the option to specify a new file name."
              )]
    private void Copy([BadParameter(description: "The Path of the file to copy")] string source,
                      [BadParameter(description: "The Destination Path")]
                      string destination,
                      [BadParameter(description: "If true, allows an existing file to be overwritten; otherwise, false."
                                   )]
                      bool overwrite = false)
    {
        m_FileSystem.Copy(source, destination, overwrite);
    }
}