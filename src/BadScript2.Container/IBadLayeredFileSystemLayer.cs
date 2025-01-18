using BadScript2.IO.Virtual;
using Newtonsoft.Json.Linq;

namespace BadScript2.Container;

public interface IBadLayeredFileSystemLayer
{
    string Name { get; }
    JObject MetaData { get; }
    BadVirtualFileSystem FileSystem { get; }
}