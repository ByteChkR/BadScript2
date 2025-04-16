using System.Collections.Generic;

namespace BadScript2.Container
{
    /// <summary>
    /// Information about a file in a layered file system
    /// </summary>
    public class BadLayeredFileSystemFileInfo
    {
        /// <summary>
        /// List of layers the file is present in
        /// </summary>
        public List<string> PresentIn { get; set; } = [];

        /// <summary>
        /// The Full Path of the file
        /// </summary>
        public string Path { get; set; } = string.Empty;
    }
}