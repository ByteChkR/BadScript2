using BadScript2.IO.Virtual;
using Newtonsoft.Json.Linq;

namespace BadScript2.Container;

/// <summary>
/// Defines a Layered File System Layer
/// </summary>
public interface IBadLayeredFileSystemLayer
{
    /// <summary>
    /// The Name of the Layer
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Meta Data about the Layer
    /// </summary>
    JObject MetaData { get; }
    /// <summary>
    /// The File System of the Layer
    /// </summary>
    BadVirtualFileSystem FileSystem { get; }
}