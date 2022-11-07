using BadScript2.Common.Logging;

namespace BadScript2.IO.Virtual;

/// <summary>
///     Implements a Virtual File System File
/// </summary>
public class BadVirtualFile : BadVirtualNode
{
    /// <summary>
    ///     The File Data
    /// </summary>
    private byte[] m_Data;

    /// <summary>
    ///     Creates a new Virtual File
    /// </summary>
    /// <param name="name">Name of the File</param>
    /// <param name="parent">The parent directory</param>
    internal BadVirtualFile(string name, BadVirtualDirectory? parent) : base(name, parent)
    {
        m_Data = Array.Empty<byte>();
    }

    /// <summary>
    ///     The Absolute Path of the File
    /// </summary>
    public string AbsolutePath => Parent?.AbsolutePath + Name;

    public override bool HasChildren => false;
    public override IEnumerable<BadVirtualNode> Children => Enumerable.Empty<BadVirtualNode>();

    /// <summary>
    ///     Returns a readable stream for the file
    /// </summary>
    /// <returns>File Stream</returns>
    public Stream OpenRead()
    {
        BadLogger.Log($"Opening File(READ) {AbsolutePath}", "BFS");

        return new MemoryStream(m_Data, false);
    }

    /// <summary>
    ///     Returns a writable stream for the file
    /// </summary>
    /// <param name="mode">Write Mode</param>
    /// <returns>File Stream</returns>
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