namespace BadScript2.Container
{
    /// <summary>
    /// Information about a layered file system stack
    /// </summary>
    public class BadLayeredFileSystemStackInfo
    {
        /// <summary>
        /// List of file systems in the stack
        /// </summary>
        public BadLayeredFileSystemInfo[] FileSystems { get; set; } = [];
        /// <summary>
        /// List of all Files in the stack
        /// </summary>
        public BadLayeredFileSystemFileInfo[] Files { get;set; } = [];
    }
}