using BadScript2.IO.Virtual;
using Newtonsoft.Json.Linq;

namespace BadScript2.Container
{
    /// <summary>
    /// Implements a Layered File System Layer
    /// </summary>
    public class BadLayeredFileSystemLayer : IBadLayeredFileSystemLayer
    {
        /// <summary>
        /// The Name of the Layer
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The Meta Data of the Layer
        /// </summary>
        public JObject MetaData { get; set; } = new JObject();
        /// <summary>
        /// The File System of the Layer
        /// </summary>
        public BadVirtualFileSystem FileSystem { get; set; } = new BadVirtualFileSystem();
        /// <summary>
        /// Creates a new Layered File System Layer
        /// </summary>
        /// <param name="name">The Name of the Layer</param>
        /// <param name="fs">The File System of the Layer</param>
        /// <param name="metaData">The Meta Data of the Layer</param>
        /// <returns>A new Layered File System Layer</returns>
        public static BadLayeredFileSystemLayer Create(string name, BadVirtualFileSystem fs, JObject? metaData = null)
        {
            return new BadLayeredFileSystemLayer() {Name = name,FileSystem = fs, MetaData = metaData ?? new JObject() };
        }
    }
}