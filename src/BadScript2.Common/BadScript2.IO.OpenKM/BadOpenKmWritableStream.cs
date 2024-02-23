using ewu.adam.openkm.rest;

namespace BadScript2.IO.OpenKM;

/// <summary>
///     Implements a Stream for writing to OpenKM
/// </summary>
public class BadOpenKmWritableStream : Stream
{
    /// <summary>
    ///     The Write Mode
    /// </summary>
    private readonly BadWriteMode m_Mode;

    /// <summary>
    ///     The Path to the file
    /// </summary>
    private readonly string m_Path;

    /// <summary>
    ///     The Underlying Stream
    /// </summary>
    private readonly Stream m_Stream;

    /// <summary>
    ///     The OpenKM Webservice
    /// </summary>
    private readonly IOkmWebservice m_Webservice;

    /// <summary>
    ///     Indicates if the stream has been disposed
    /// </summary>
    private bool m_Disposed;

    /// <summary>
    ///     Constructs a new BadOpenKmWritableStream instance
    /// </summary>
    /// <param name="webservice">The OpenKM Webservice</param>
    /// <param name="path">The Path to the file</param>
    /// <param name="mode">The Write Mode</param>
    public BadOpenKmWritableStream(IOkmWebservice webservice, string path, BadWriteMode mode)
    {
        m_Webservice = webservice;
        m_Path = path;
        m_Mode = mode;
        m_Stream = new MemoryStream();

        if (mode != BadWriteMode.Append)
        {
            return;
        }

        Stream content = m_Webservice.GetContent(path).Result;
        content.CopyTo(m_Stream);
    }

    /// <inheritdoc />
    public override bool CanRead => m_Stream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => m_Stream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => m_Stream.CanWrite;

    /// <inheritdoc />
    public override long Length => m_Stream.Length;

    /// <inheritdoc />
    public override long Position
    {
        get => m_Stream.Position;
        set => m_Stream.Position = value;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing && !m_Disposed)
        {
            m_Disposed = true;
            m_Stream.Position = 0;

            if (m_Mode == BadWriteMode.Append)
            {
                //Lock file
                m_Webservice.Checkout(m_Path).Wait();

                //Upload content
                m_Webservice.Checkin(m_Path, "", m_Stream).Wait();
            }
            else
            {
                bool hasNode = m_Webservice.HasNode(m_Path).Result;

                if (hasNode && m_Webservice.IsValidDocument(m_Path).Result)
                {
                    //Lock file
                    m_Webservice.Checkout(m_Path).Wait();

                    //Upload content
                    m_Webservice.Checkin(m_Path, "", m_Stream).Wait();
                }
                else
                {
                    m_Webservice.CreateDocumentSimple(m_Path, m_Stream).Wait();
                }
            }
        }

        m_Stream.Close();
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override void Flush()
    {
        m_Stream.Flush();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        return m_Stream.Read(buffer, offset, count);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        return m_Stream.Seek(offset, origin);
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        m_Stream.SetLength(value);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        m_Stream.Write(buffer, offset, count);
    }
}