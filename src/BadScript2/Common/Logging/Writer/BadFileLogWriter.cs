using BadScript2.IO;

namespace BadScript2.Common.Logging.Writer;

/// <summary>
///     Implements a simple file writer for the log system
/// </summary>
public class BadFileLogWriter : BadStreamLogWriter
{
	/// <summary>
	///     Creates a new File Log Writer
	/// </summary>
	/// <param name="fs">The File System to use</param>
	/// <param name="file">The File name of the log file</param>
	public BadFileLogWriter(IFileSystem fs, string file) : base(fs.OpenWrite(file,
	                                                                             fs.IsFile(file)
		                                                                             ? BadWriteMode.Append
		                                                                             : BadWriteMode.CreateNew
	                                                                            )
	                                           ) { }
}