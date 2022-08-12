using BadScript2.IO;

namespace BadScript2.Common.Logging.Writer;

public class BadFileLogWriter : BadStreamLogWriter
{
    public BadFileLogWriter(string file) : base(BadFileSystem.Instance.OpenWrite(file, BadWriteMode.CreateNew)) { }
}