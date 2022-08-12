namespace BadScript2.IO.Virtual;

public class BadVirtualFileSystem : IFileSystem
{
    private readonly BadVirtualRoot m_Root = new BadVirtualRoot();
    private string m_CurrentDirectory = "/";

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

        return current.FileExists(parts[^1]);
    }

    public bool IsDirectory(string path)
    {
        string[] parts = BadVirtualPathReader.SplitPath(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory));
        BadVirtualDirectory current = m_Root;
        for (int i = 0; i < parts.Length; i++)
        {
            if (!current.DirectoryExists(parts[i]))
            {
                return false;
            }

            current = current.GetDirectory(parts[i]);
        }

        return true;
    }

    public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
    {
        if (recursive)
        {
            return GetFilesRecursive(GetDirectory(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory)), extension);
        }

        return GetFiles(GetDirectory(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory)), extension);
    }

    public IEnumerable<string> GetDirectories(string path, bool recursive)
    {
        if (recursive)
        {
            return GetDirectoriesRecursive(GetDirectory(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory)));
        }

        return GetDirectories(GetDirectory(BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory)));
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

        current.CreateDirectory(parts[^1]);
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

        return dir.GetFile(BadVirtualPathReader.SplitPath(fullPath)[^1]).OpenRead();
    }

    public Stream OpenWrite(string path, BadWriteMode mode)
    {
        string fullPath = BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory);
        if (mode == BadWriteMode.Append && (!Exists(fullPath) || !IsFile(fullPath)))
        {
            throw new FileNotFoundException(path);
        }

        BadVirtualDirectory dir = GetParentDirectory(fullPath);

        return dir.GetOrCreateFile(BadVirtualPathReader.SplitPath(fullPath)[^1]).OpenWrite(mode);
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


    public BadVirtualRoot GetRoot()
    {
        return m_Root;
    }

    private BadVirtualDirectory GetParentDirectory(string path)
    {
        return GetDirectory(BadVirtualPathReader.JoinPath(BadVirtualPathReader.SplitPath(path).SkipLast(1)));
    }

    private BadVirtualDirectory GetDirectory(string path)
    {
        if (string.IsNullOrEmpty(path) || BadVirtualPathReader.IsRootPath(path))
        {
            return m_Root;
        }

        string fullPath = BadVirtualPathReader.ResolvePath(path, m_CurrentDirectory);
        string[] parts = BadVirtualPathReader.SplitPath(fullPath);
        BadVirtualDirectory current = m_Root;
        for (int i = 0; i < parts.Length; i++)
        {
            current = current.GetDirectory(parts[i]);
        }

        return current;
    }

    private IEnumerable<string> GetDirectories(BadVirtualDirectory directory)
    {
        foreach (BadVirtualDirectory sub in directory.Directories)
        {
            yield return sub.AbsolutePath;
        }
    }

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

    private IEnumerable<string> GetFiles(BadVirtualDirectory directory, string extension)
    {
        foreach (BadVirtualFile file in directory.Files)
        {
            if (extension == "" || Path.GetExtension(file.Name) == extension)
            {
                yield return file.AbsolutePath;
            }
        }
    }

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

    private bool IsSubfolderOf(string root, string sub)
    {
        return GetFullPath(sub).StartsWith(GetFullPath(root));
    }

    private void CopyFileToFile(string src, string dst)
    {
        using Stream s = OpenRead(src);
        using Stream d = OpenWrite(dst, BadWriteMode.CreateNew);
        s.CopyTo(d);
    }

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