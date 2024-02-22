using BadScript2.IO.Virtual;

using ewu.adam.openkm.rest;
using ewu.adam.openkm.rest.Bean;

///<summary>
///	File System Implementation for OpenKM
/// </summary>
namespace BadScript2.IO.OpenKM;

/// <summary>
///     Implements a FileSystem for OpenKM
/// </summary>
public class BadOpenKmFileSystem : IFileSystem
{
    /// <summary>
    ///     The OpenKM Webservice
    /// </summary>
    private readonly IOkmWebservice m_Webservice;

    /// <summary>
    ///     The Current Directory
    /// </summary>
    private Folder m_Current;

    /// <summary>
    ///     The Startup Directory
    /// </summary>
    private string? m_StartupDirectory;

    /// <summary>
    ///     Constructs a new BadOpenKmFileSystem instance
    /// </summary>
    /// <param name="webService">The OpenKM Webservice</param>
    public BadOpenKmFileSystem(IOkmWebservice webService)
    {
        m_Webservice = webService;
        m_Current = m_Webservice.GetFolderProperties(GetStartupDirectory()).Result;
    }

    /// <summary>
    ///     Constructs a new BadOpenKmFileSystem instance
    /// </summary>
    /// <param name="url">The OpenKM URL</param>
    /// <param name="user">The OpenKM User</param>
    /// <param name="password">The OpenKM Password</param>
    public BadOpenKmFileSystem(string url, string user, string password) : this(
        OkmWebserviceFactory.NewInstance(url, user, password)
    ) { }


    /// <inheritdoc />
    public string GetStartupDirectory()
    {
        return m_StartupDirectory ??= m_Webservice.GetPersonalFolder().Result.path;
    }

    /// <inheritdoc />
    public bool Exists(string path)
    {
        return m_Webservice.HasNode(MakeFullPath(path)).Result;
    }

    /// <inheritdoc />
    public bool IsFile(string path)
    {
        return m_Webservice.IsValidDocument(MakeFullPath(path)).Result;
    }

    /// <inheritdoc />
    public bool IsDirectory(string path)
    {
        return m_Webservice.IsValidFolder(MakeFullPath(path)).Result;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
    {
        path = MakeFullPath(path);

        return recursive ? InnerGetFilesRecursive(path, extension) : InnerGetFiles(path, extension);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetDirectories(string path, bool recursive)
    {
        path = MakeFullPath(path);

        return recursive ? InnerGetDirectoriesRecursive(path) : InnerGetDirectories(path);
    }

    /// <inheritdoc />
    public void CreateDirectory(string path, bool recursive = false)
    {
        path = MakeFullPath(path);

        if (recursive)
        {
            InnerCreateDirectoryRecursive(path);
        }
        else
        {
            InnerCreateDirectory(path);
        }
    }

    /// <inheritdoc />
    public void DeleteDirectory(string path, bool recursive)
    {
        path = MakeFullPath(path);

        if (!Exists(path))
        {
            throw new Exception("Directory does not exist.");
        }

        if (!recursive && HasChildren(path))
        {
            throw new Exception("Directory is not empty.");
        }

        m_Webservice.DeleteFolder(path).Wait();
    }

    /// <inheritdoc />
    public void DeleteFile(string path)
    {
        path = MakeFullPath(path);

        if (!IsFile(path))
        {
            throw new Exception("File does not exist.");
        }

        m_Webservice.DeleteDocument(path).Wait();
    }

    /// <inheritdoc />
    public string GetFullPath(string path)
    {
        return MakeFullPath(path);
    }

    /// <inheritdoc />
    public Stream OpenRead(string path)
    {
        Stream result = m_Webservice.GetContent(MakeFullPath(path)).Result;

        return result;
    }

    /// <inheritdoc />
    public Stream OpenWrite(string path, BadWriteMode mode)
    {
        return new BadOpenKmWritableStream(m_Webservice, MakeFullPath(path), mode);
    }

    /// <inheritdoc />
    public string GetCurrentDirectory()
    {
        return m_Current.path;
    }

    /// <inheritdoc />
    public void SetCurrentDirectory(string path)
    {
        m_Current = m_Webservice.GetFolderProperties(MakeFullPath(path)).Result;
    }

    /// <inheritdoc />
    public void Copy(string src, string dst, bool overwrite = true)
    {
        src = MakeFullPath(src);
        dst = MakeFullPath(dst);

        if (!Exists(src))
        {
            throw new Exception("Source does not exist");
        }

        if (IsDirectory(src))
        {
            throw new NotSupportedException(
                "At the moment, copying directories is not supported. (need to implement recursive move files)"
            );
        }

        if (!Exists(dst))
        {
            throw new Exception("Target directory does not exist");
        }

        string dstFile = MakeFullPath(Path.Combine(dst, Path.GetFileName(src)));

        if (Exists(dstFile))
        {
            if (!IsFile(dstFile))
            {
                throw new Exception("Target file is a directory");
            }

            if (!overwrite)
            {
                throw new Exception("Target already exists");
            }

            m_Webservice.DeleteDocument(dstFile).Wait();
        }

        m_Webservice.CopyDocument(src, dst).Wait();
    }

    /// <inheritdoc />
    public void Move(string src, string dst, bool overwrite = true)
    {
        src = MakeFullPath(src);
        dst = MakeFullPath(dst);

        if (!Exists(src))
        {
            throw new Exception("Source does not exist");
        }

        if (IsDirectory(src))
        {
            throw new NotSupportedException(
                "At the moment, moving directories is not supported. (need to implement recursive move files)"
            );
        }

        if (!Exists(dst))
        {
            throw new Exception("Target directory does not exist");
        }

        string dstFile = MakeFullPath(Path.Combine(dst, Path.GetFileName(src)));

        if (Exists(dstFile))
        {
            if (!IsFile(dstFile))
            {
                throw new Exception("Target file is a directory");
            }

            if (!overwrite)
            {
                throw new Exception("Target already exists");
            }

            m_Webservice.DeleteDocument(dstFile).Wait();
        }

        m_Webservice.MoveDocument(src, dst).Wait();
    }

    /// <summary>
    ///     Makes the given path a full path
    /// </summary>
    /// <param name="path">The path to make full</param>
    /// <returns>The full path</returns>
    private string MakeFullPath(string path)
    {
        string full = BadVirtualPathReader.ResolvePath(path, GetCurrentDirectory());

        return full;
    }

    /// <summary>
    ///     Returns true if the given path has children
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>true if the given path has children</returns>
    private bool HasChildren(string path)
    {
        return m_Webservice.GetFolderChildren(path).Result.folder.Count > 0 ||
               m_Webservice.GetDocumentChildren(path).Result.document.Count > 0;
    }

    /// <summary>
    ///     Returns all directories in the given directory
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <returns>Enumeration of all directories in the given directory</returns>
    private IEnumerable<string> InnerGetDirectories(string path)
    {
        FolderList? current = m_Webservice.GetFolderChildren(path).Result;

        return current == null ? Enumerable.Empty<string>() : current.folder.Select(x => x.path);
    }

    /// <summary>
    ///     Returns all files in the given directory that match the specified extension
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <param name="extension">The extension to match</param>
    /// <returns>Enumeration of all files in the given directory that match the specified extension</returns>
    private IEnumerable<string> InnerGetFiles(string path, string extension)
    {
        DocumentList? current = m_Webservice.GetDocumentChildren(path).Result;

        if (current == null)
        {
            yield break;
        }

        foreach (Document document in current.document)
        {
            string ext = Path.GetExtension(document.path);

            if (string.IsNullOrEmpty(extension) || ext == extension)
            {
                yield return document.path;
            }
        }
    }

    /// <summary>
    ///     Recursively returns all directories in the given directory
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <returns>Enumeration of all directories in the given directory</returns>
    private IEnumerable<string> InnerGetDirectoriesRecursive(string path)
    {
        foreach (string dir in InnerGetDirectories(path))
        {
            yield return dir;

            foreach (string innerDir in InnerGetDirectoriesRecursive(dir))
            {
                yield return innerDir;
            }
        }
    }

    /// <summary>
    ///     Recursively returns all files in the given directory that match the specified extension
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <param name="extension">The extension to match</param>
    /// <returns>Enumeration of all files in the given directory that match the specified extension</returns>
    private IEnumerable<string> InnerGetFilesRecursive(string path, string extension)
    {
        foreach (string file in InnerGetFiles(path, extension))
        {
            yield return file;
        }

        foreach (string directory in InnerGetDirectories(path))
        {
            foreach (string file in InnerGetFilesRecursive(directory, extension))
            {
                yield return file;
            }
        }
    }


    /// <summary>
    ///     Creates a directory recursively
    /// </summary>
    /// <param name="path">The path to the directory</param>
    private void InnerCreateDirectoryRecursive(string path)
    {
        if (IsDirectory(path))
        {
            return;
        }

        string? parent = Path.GetDirectoryName(path);

        if (parent != null)
        {
            InnerCreateDirectoryRecursive(parent);
        }

        InnerCreateDirectory(path);
    }

    /// <summary>
    ///     Creates a directory
    /// </summary>
    /// <param name="path">The path to the directory</param>
    private void InnerCreateDirectory(string path)
    {
        m_Webservice.CreateFolderSimple(path).Wait();
    }
}