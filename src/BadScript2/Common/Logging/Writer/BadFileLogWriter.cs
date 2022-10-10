using BadScript2.IO;

namespace BadScript2.Common.Logging.Writer
{
    /// <summary>
    ///     Implements a simple file writer for the log system
    /// </summary>
    public class BadFileLogWriter : BadStreamLogWriter
    {
        /// <summary>
        ///     Creates a new File Log Writer
        /// </summary>
        /// <param name="file">The File name of the log file</param>
        public BadFileLogWriter(string file) : base(BadFileSystem.Instance.OpenWrite(file, BadFileSystem.Instance.IsFile(file) ? BadWriteMode.Append : BadWriteMode.CreateNew)) { }
    }
}