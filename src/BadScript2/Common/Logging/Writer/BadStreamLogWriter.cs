namespace BadScript2.Common.Logging.Writer;

public abstract class BadStreamLogWriter : BadLogWriter
{
    private readonly StreamWriter m_Stream;

    public BadStreamLogWriter(Stream stream)
    {
        m_Stream = new StreamWriter(stream);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_Stream.Dispose();
    }

    protected override void Write(BadLog log)
    {
        m_Stream.WriteLine(log);
    }
}