namespace BadScript2.Common.Logging.Writer;

/// <summary>
/// Implements a simple stream writer for the log system
/// </summary>
public abstract class BadStreamLogWriter : BadLogWriter
{
    /// <summary>
    /// The underlying streamwriter
    /// </summary>
    private readonly StreamWriter m_Stream;

    /// <summary>
    /// Creates a new StreamLogWriter
    /// </summary>
    /// <param name="stream">The underlying stream</param>
    protected BadStreamLogWriter(Stream stream)
    {
        m_Stream = new StreamWriter(stream);
    }

    /// <summary>
    /// Implements the IDisposable interface
    /// </summary>
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