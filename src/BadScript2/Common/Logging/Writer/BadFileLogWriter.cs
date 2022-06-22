namespace BadScript2.Common.Logging.Writer;

public class BadFileLogWriter : BadStreamLogWriter
{
    public BadFileLogWriter(string file) : base(File.OpenWrite(file)) { }
}