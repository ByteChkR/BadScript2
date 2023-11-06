using BadScript2.IO.Virtual;

using ewu.adam.openkm.rest;
using ewu.adam.openkm.rest.Bean;

namespace BadScript2.IO.OpenKM;

public class BadOpenKmFileSystem : IFileSystem
{
    private readonly IOkmWebservice m_Webservice;
    private Folder m_Current;

    private string? m_StartupDirectory;

    public BadOpenKmFileSystem(IOkmWebservice webService)
    {
        m_Webservice = webService;
        m_Current = m_Webservice.GetFolderProperties(GetStartupDirectory()).Result;
    }

    public BadOpenKmFileSystem(string url, string user, string password) : this(
        OkmWebserviceFactory.NewInstance(url, user, password)
    ) { }

    public string GetStartupDirectory()
    {
        if (m_StartupDirectory == null)
        {
            m_StartupDirectory = m_Webservice.GetPersonalFolder().Result.path;
        }

        return m_StartupDirectory;
    }

    public bool Exists(string path)
    {
        return m_Webservice.HasNode(MakeFullPath(path)).Result;
    }

    public bool IsFile(string path)
    {
        return m_Webservice.IsValidDocument(MakeFullPath(path)).Result;
    }

    public bool IsDirectory(string path)
    {
        return m_Webservice.IsValidFolder(MakeFullPath(path)).Result;
    }

    public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
    {
        path = MakeFullPath(path);

        if (recursive)
        {
            return InnerGetFilesRecursive(path, extension);
        }

        return InnerGetFiles(path, extension);
    }

    public IEnumerable<string> GetDirectories(string path, bool recursive)
    {
        path = MakeFullPath(path);

        if (recursive)
        {
            return InnerGetDirectoriesRecursive(path);
        }

        return InnerGetDirectories(path);
    }

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

    public void DeleteFile(string path)
    {
        path = MakeFullPath(path);

        if (!IsFile(path))
        {
            throw new Exception("File does not exist.");
        }

        m_Webservice.DeleteDocument(path).Wait();
    }

    public string GetFullPath(string path)
    {
        return MakeFullPath(path);
    }

    public Stream OpenRead(string path)
    {
        Stream result = m_Webservice.GetContent(MakeFullPath(path)).Result;

        return result;
    }

    public Stream OpenWrite(string path, BadWriteMode mode)
    {
        return new BadOpenKmWritableStream(m_Webservice, MakeFullPath(path), mode);
    }

    public string GetCurrentDirectory()
    {
        return m_Current.path;
    }

    public void SetCurrentDirectory(string path)
    {
        m_Current = m_Webservice.GetFolderProperties(MakeFullPath(path)).Result;
    }

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

    private string MakeFullPath(string path)
    {
        string full = BadVirtualPathReader.ResolvePath(path, GetCurrentDirectory());

        return full;
    }

    private bool HasChildren(string path)
    {
        return m_Webservice.GetFolderChildren(path).Result.folder.Count > 0 ||
               m_Webservice.GetDocumentChildren(path).Result.document.Count > 0;
    }

    private IEnumerable<string> InnerGetDirectories(string path)
    {
        FolderList? current = m_Webservice.GetFolderChildren(path).Result;

        if (current == null)
        {
            return Enumerable.Empty<string>();
        }

        return current.folder.Select(x => x.path);
    }

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


    private void InnerCreateDirectoryRecursive(string path)
    {
        if (!IsDirectory(path))
        {
            string? parent = Path.GetDirectoryName(path);

            if (parent != null)
            {
                InnerCreateDirectoryRecursive(parent);
            }

            InnerCreateDirectory(path);
        }
    }

    private void InnerCreateDirectory(string path)
    {
        m_Webservice.CreateFolderSimple(path).Wait();
    }
}