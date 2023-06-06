namespace BadScript2.IO;

/// <summary>
///     Defines the interface for a file system
/// </summary>
public interface IFileSystem
{
    /// <summary>
    ///     The Startup Directory of the Application
    /// </summary>
    /// <returns>The Startup Directory of the Application</returns>
    string GetStartupDirectory();

    /// <summary>
    ///     Returns true if the given path is a file or directory
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>true if the given path is a file or directory</returns>
    bool Exists(string path);

    /// <summary>
    ///     Returns true if the given path is a file
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>true if the given path is a file </returns>
    bool IsFile(string path);

    /// <summary>
    ///     Returns true if the given path is a directory
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>true if the given path is a directory </returns>
    bool IsDirectory(string path);

    /// <summary>
    ///     Returns all files in the given directory that match the specified extension
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <param name="extension">The extension to match</param>
    /// <param name="recursive">True if the search should be recursive</param>
    /// <returns>Enumeration of all files in the given directory that match the specified extension</returns>
    IEnumerable<string> GetFiles(string path, string extension, bool recursive);

    /// <summary>
    ///     Returns all directories in the given directory
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <param name="recursive">True if the search should be recursive</param>
    /// <returns>Enumeration of all directories in the given directory that match the specified extension</returns>
    IEnumerable<string> GetDirectories(string path, bool recursive);

    /// <summary>
    ///     Creates a new directory
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <param name="recursive">If true, all parent directories will be created if they do not exist</param>
    void CreateDirectory(string path, bool recursive = false);

    /// <summary>
    ///     Deletes a directory
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <param name="recursive">If true, all subdirectories will be deleted</param>
    void DeleteDirectory(string path, bool recursive);

    /// <summary>
    ///     Deletes a file
    /// </summary>
    /// <param name="path">The path to the directory</param>
    void DeleteFile(string path);

    /// <summary>
    ///     Returns the full path of the given path
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>Full Path</returns>
    string GetFullPath(string path);

    /// <summary>
    ///     Opens a file for reading
    /// </summary>
    /// <param name="path">The path to the file</param>
    /// <returns>File Stream</returns>
    Stream OpenRead(string path);

    /// <summary>
    ///     Opens a file for writing
    /// </summary>
    /// <param name="path">The path to the file</param>
    /// <param name="mode">The Write Mode</param>
    /// <returns>File Stream</returns>
    Stream OpenWrite(string path, BadWriteMode mode);

    /// <summary>
    ///     Returns the Current Directory
    /// </summary>
    /// <returns>The Current Directory</returns>
    string GetCurrentDirectory();

    /// <summary>
    ///     Sets the current Directory
    /// </summary>
    /// <param name="path">the new current directory</param>
    void SetCurrentDirectory(string path);

    /// <summary>
    ///     Copies a file or directory to a new location
    /// </summary>
    /// <param name="src">Source path</param>
    /// <param name="dst">Destination path</param>
    /// <param name="overwrite">Should file entries be overwritten?</param>
    void Copy(string src, string dst, bool overwrite = true);

    /// <summary>
    ///     Moves a file or directory to a new location
    /// </summary>
    /// <param name="src">Source path</param>
    /// <param name="dst">Destination path</param>
    /// <param name="overwrite">Should file entries be overwritten?</param>
    void Move(string src, string dst, bool overwrite = true);
}
