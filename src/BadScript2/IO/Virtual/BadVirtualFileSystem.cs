using BadScript2.Utility;

namespace BadScript2.IO.Virtual;

/// <summary>
///     Virtual File System Implementation for the BadScript Engine
/// </summary>
public class BadVirtualFileSystem : IVirtualFileSystem
{
    /// <summary>
    ///     The Root Directory
    /// </summary>
    private readonly BadVirtualRoot m_Root = new BadVirtualRoot();

    /// <summary>
    ///     The Current Directory
    /// </summary>
    private string m_CurrentDirectory = "/";

#region IFileSystem Members

    public string GetStartupDirectory()
    {
        return "/";
    }

    public bool Exists(string path)
    {
        return IsFile(path) || IsDirectory(path);
    }

    public bool IsFile(string path)
    {
        string[] parts = BadVirtualPathReader.SplitPath(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory));
        BadVirtualDirectory current = m_Root;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (!current.DirectoryExists(parts[i]))
            {
                return false;
            }

            current = current.GetDirectory(parts[i]);
        }

        return current.FileExists(parts[parts.Length - 1]);
    }

    public bool IsDirectory(string path)
    {
        string[] parts = BadVirtualPathReader.SplitPath(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory));

        if (parts.Length == 1 && string.IsNullOrEmpty(parts[0]))
        {
            return true;
        }

        BadVirtualDirectory current = m_Root;

        foreach (string t in parts)
        {
            if (!current.DirectoryExists(t))
            {
                return false;
            }

            current = current.GetDirectory(t);
        }

        return true;
    }

    public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
    {
        if (recursive)
        {
            return GetFilesRecursive(GetDirectory(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory)),
                                     extension
                                    );
        }

        return GetFiles(GetDirectory(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory)), extension);
    }

    public IEnumerable<string> GetDirectories(string path, bool recursive)
    {
        return recursive
                   ? GetDirectoriesRecursive(GetDirectory(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory)))
                   : GetDirectories(GetDirectory(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory)));
    }

    public void CreateDirectory(string path, bool recursive = false)
    {
        string[] parts = BadVirtualPathReader.SplitPath(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory));
        BadVirtualDirectory current = m_Root;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (!current.DirectoryExists(parts[i]))
            {
                if (!recursive)
                {
                    throw new IOException("Directory does not exist");
                }

                current = current.CreateDirectory(parts[i]);
            }
            else
            {
                current = current.GetDirectory(parts[i]);
            }
        }

        current.CreateDirectory(parts[parts.Length - 1]);
    }

    public void DeleteDirectory(string path, bool recursive)
    {
        BadVirtualDirectory parent = GetParentDirectory(path);
        parent.DeleteDirectory(Path.GetFileName(path), recursive);
    }

    public void DeleteFile(string path)
    {
        BadVirtualDirectory parent = GetParentDirectory(path);
        parent.DeleteFile(Path.GetFileName(path));
    }

    public string GetFullPath(string path)
    {
        return BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory);
    }

    public Stream OpenRead(string path)
    {
        string fullPath = BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory);

        if (!Exists(fullPath) || !IsFile(fullPath))
        {
            throw new FileNotFoundException(fullPath);
        }

        BadVirtualDirectory dir = GetParentDirectory(fullPath);

        string[] paths = BadVirtualPathReader.SplitPath(fullPath);

        return dir.GetFile(paths[paths.Length - 1])
                  .OpenRead();
    }

    public Stream OpenWrite(string path, BadWriteMode mode)
    {
        string fullPath = BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory);

        if (mode == BadWriteMode.Append && (!Exists(fullPath) || !IsFile(fullPath)))
        {
            throw new FileNotFoundException(path);
        }

        BadVirtualDirectory dir = GetParentDirectory(fullPath);
        string[] paths = BadVirtualPathReader.SplitPath(fullPath);

        return dir.GetOrCreateFile(paths[paths.Length - 1])
                  .OpenWrite(mode);
    }

    public string GetCurrentDirectory()
    {
        return m_CurrentDirectory;
    }

    public void SetCurrentDirectory(string path)
    {
        if (!Exists(path) || !IsDirectory(path))
        {
            throw new DirectoryNotFoundException(path);
        }

        m_CurrentDirectory = BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory);
    }

    public void Copy(string src, string dst, bool overwrite = true)
    {
        if (IsDirectory(src))
        {
            if (IsSubfolderOf(src, dst))
            {
                throw new IOException("Cannot copy a directory to a subfolder of itself.");
            }

            if (!overwrite && IsDirectory(src))
            {
                throw new IOException("Directory already exists.");
            }

            CopyDirectoryToDirectory(src, dst);
        }
        else if (IsFile(src))
        {
            if (!overwrite && IsFile(src))
            {
                throw new IOException("File already exists.");
            }

            CopyFileToFile(src, dst);
        }
        else
        {
            throw new IOException("Source path is not a file or directory");
        }
    }

    public void Move(string src, string dst, bool overwrite = true)
    {
        Copy(src, dst, overwrite);

        if (IsDirectory(src))
        {
            DeleteDirectory(src, true);
        }
        else
        {
            DeleteFile(src);
        }
    }

#endregion

    /// <summary>
    ///     Returns the root directory of the filesystem
    /// </summary>
    /// <returns>The root directory</returns>
    public BadVirtualRoot GetRoot()
    {
        return m_Root;
    }

    /// <summary>
    ///     Returns the Parent directory of the specified path
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>Parent Directory Path</returns>
    private BadVirtualDirectory GetParentDirectory(string path)
    {
        return GetDirectory(BadVirtualPathReader.JoinPath(BadVirtualPathReader
                                                          .SplitPath(BadVirtualPathReader.ResolvePath(path,
                                                                          m_CurrentDirectory
                                                                         )
                                                                    )
                                                          .SkipLast(1)
                                                         )
                           );
    }

    /// <summary>
    ///     Returns the directory at the specified path
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>Directory Object</returns>
    private BadVirtualDirectory GetDirectory(string path)
    {
        if (string.IsNullOrEmpty(path) || BadVirtualPathReader.IsRootPath(path))
        {
            return m_Root;
        }

        string fullPath = BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory);
        string[] parts = BadVirtualPathReader.SplitPath(fullPath);

        return parts.Aggregate<string, BadVirtualDirectory>(m_Root, (current1, t) => current1.GetDirectory(t));
    }

    /// <summary>
    ///     Returns the directories in the specified path
    /// </summary>
    /// <param name="directory">Path</param>
    /// <returns>Directory Paths</returns>
    private static IEnumerable<string> GetDirectories(BadVirtualDirectory directory)
    {
        return directory.Directories.Select(sub => sub.AbsolutePath);
    }

    /// <summary>
    ///     Returns the directories in the specified path recursively
    /// </summary>
    /// <param name="directory">Path</param>
    /// <returns>Directory Paths</returns>
    private IEnumerable<string> GetDirectoriesRecursive(BadVirtualDirectory directory)
    {
        foreach (string s in GetDirectories(directory))
        {
            yield return s;
        }

        foreach (BadVirtualDirectory sub in directory.Directories)
        {
            foreach (string s in GetDirectoriesRecursive(sub))
            {
                yield return s;
            }
        }
    }

    /// <summary>
    ///     Returns the files in the specified path that match the specified extension
    /// </summary>
    /// <param name="directory">Path</param>
    /// <param name="extension">The File Extension to be matched</param>
    /// <returns>File Paths</returns>
    private static IEnumerable<string> GetFiles(BadVirtualDirectory directory, string extension)
    {
        return from file in directory.Files
               where extension == "" || Path.GetExtension(file.Name) == extension
               select file.AbsolutePath;
    }

    /// <summary>
    ///     Returns the files in the specified path that match the specified extension recursively
    /// </summary>
    /// <param name="directory">Path</param>
    /// <param name="extension">The File Extension to be matched</param>
    /// <returns>File Paths</returns>
    private IEnumerable<string> GetFilesRecursive(BadVirtualDirectory directory, string extension)
    {
        foreach (string file in GetFiles(directory, extension))
        {
            yield return file;
        }

        foreach (BadVirtualDirectory subDirectory in directory.Directories)
        {
            foreach (string file in GetFilesRecursive(subDirectory, extension))
            {
                yield return file;
            }
        }
    }

    /// <summary>
    ///     Returns true if sub is a subfolder of root
    /// </summary>
    /// <param name="root">Root Directory</param>
    /// <param name="sub">Sub Directory</param>
    /// <returns>True if sub is a subfolder of root</returns>
    private bool IsSubfolderOf(string root, string sub)
    {
        return GetFullPath(sub)
            .StartsWith(GetFullPath(root));
    }

    /// <summary>
    ///     Copies a file to a file
    /// </summary>
    /// <param name="src">Source File</param>
    /// <param name="dst">Destination File</param>
    private void CopyFileToFile(string src, string dst)
    {
        using Stream s = OpenRead(src);
        using Stream d = OpenWrite(dst, BadWriteMode.CreateNew);
        s.CopyTo(d);
    }

    /// <summary>
    ///     Copies a directory to a directory
    /// </summary>
    /// <param name="src">Source Directory</param>
    /// <param name="dst">Destination Directory</param>
    private void CopyDirectoryToDirectory(string src, string dst)
    {
        foreach (string directory in GetDirectories(src, true))
        {
            CreateDirectory(directory);
        }

        foreach (string file in GetFiles(src, "*", true))
        {
            CopyFileToFile(file, file.Replace(src, dst));
        }
    }
}