using BadScript2.Common.Logging;

namespace BadScript2.IO.Virtual;

public class BadVirtualDirectory : BadVirtualNode
{
    private readonly List<BadVirtualDirectory> m_Directories = new List<BadVirtualDirectory>();
    private readonly List<BadVirtualFile> m_Files = new List<BadVirtualFile>();

    protected BadVirtualDirectory(string name, BadVirtualDirectory? parent) : base(name, parent) { }

    public bool IsEmpty => m_Files.Count == 0 && m_Directories.All(x => x.IsEmpty);


    public IEnumerable<BadVirtualDirectory> Directories => m_Directories;
    public IEnumerable<BadVirtualFile> Files => m_Files;

    public string AbsolutePath => Parent?.AbsolutePath + Name + "/";

    public override bool HasChildren => m_Directories.Count != 0 || m_Files.Count != 0;

    public override IEnumerable<BadVirtualNode> Children => m_Directories.Concat(m_Files.Cast<BadVirtualNode>());

    public bool FileExists(string name)
    {
        return m_Files.Any(x => x.Name == name);
    }

    public bool DirectoryExists(string name)
    {
        return m_Directories.Any(x => x.Name == name);
    }

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

    public void DeleteFile(string name)
    {
        BadLogger.Log($"Deleting File {AbsolutePath}{name}", "BFS");
        m_Files.RemoveAll(x => x.Name == name);
    }

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

    public BadVirtualDirectory GetDirectory(string name)
    {
        return m_Directories.FirstOrDefault(x => x.Name == name) ?? throw new Exception($"Directory '{name}' not found in {this}");
    }

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

    public BadVirtualFile GetOrCreateFile(string name)
    {
        return FileExists(name) ? GetFile(name) : CreateFile(name);
    }

    public BadVirtualFile GetFile(string name)
    {
        return m_Files.FirstOrDefault(x => x.Name == name) ?? throw new Exception($"File '{name}' not found in {this}");
    }
}