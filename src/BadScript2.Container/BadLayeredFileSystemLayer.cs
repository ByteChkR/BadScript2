using BadScript2.IO.Virtual;
using Newtonsoft.Json.Linq;

namespace BadScript2.Container
{
    public class BadLayeredFileSystemLayer : IBadLayeredFileSystemLayer
    {
        public string Name { get; set; }
        public JObject MetaData { get; set; }
        public BadVirtualFileSystem FileSystem { get; set; }
        public static BadLayeredFileSystemLayer Create(string name, BadVirtualFileSystem fs, JObject? metaData = null)
        {
            return new BadLayeredFileSystemLayer() {Name = name,FileSystem = fs, MetaData = metaData ?? new JObject() };
        }
    }
}