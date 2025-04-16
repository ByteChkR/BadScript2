using Newtonsoft.Json.Linq;

namespace BadScript2.Container
{
    /// <summary>
    /// Information about a layered file system
    /// </summary>
    public class BadLayeredFileSystemInfo
    {
        /// <summary>
        /// Indicates if the top element of the filesystem stack is writable
        /// </summary>
        public bool Writable { get;set; }
        /// <summary>
        /// The Name of the Layered File System
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Meta Data about the Layered File System
        /// </summary>
        public JObject MetaData { get; set; } = new JObject();
    }
}