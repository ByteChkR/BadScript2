using BadScript2.Common.Logging;

namespace BadScript2.IO.Virtual;

public class BadVirtualFile : BadVirtualNode
{
    private byte[] m_Data;

    internal BadVirtualFile(string name, BadVirtualDirectory? parent) : base(name, parent)
    {
        m_Data = Array.Empty<byte>();
    }

    public string AbsolutePath => Parent?.AbsolutePath + Name;

    public override bool HasChildren => false;
    public override IEnumerable<BadVirtualNode> Children => Enumerable.Empty<BadVirtualNode>();

    public Stream OpenRead()
    {
        BadLogger.Log($"Opening File(READ) {AbsolutePath}", "BFS");

        return new MemoryStream(m_Data, false);
    }

    public Stream OpenWrite(BadWriteMode mode)
    {
        BadLogger.Log($"Opening File(WRITE: {mode}) {AbsolutePath}", "BFS");
        BadVirtualFileStream s = new BadVirtualFileStream();
        if (mode == BadWriteMode.CreateNew)
        {
            m_Data = Array.Empty<byte>();
        }

        s.Write(m_Data, 0, m_Data.Length);

        s.OnDispose += () => m_Data = s.ToArray();

        return s;
    }
}