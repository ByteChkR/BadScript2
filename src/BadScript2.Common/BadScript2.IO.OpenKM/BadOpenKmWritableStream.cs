using ewu.adam.openkm.rest;

namespace BadScript2.IO.OpenKM;

public class BadOpenKmWritableStream : Stream
{
	private readonly BadWriteMode m_Mode;
	private readonly string m_Path;
	private readonly Stream m_Stream;
	private readonly IOkmWebservice m_Webservice;
	private bool m_Disposed;

	public BadOpenKmWritableStream(IOkmWebservice webservice, string path, BadWriteMode mode)
	{
		m_Webservice = webservice;
		m_Path = path;
		m_Mode = mode;
		m_Stream = new MemoryStream();

		if (mode == BadWriteMode.Append)
		{
			Stream content = m_Webservice.GetContent(path).Result;
			content.CopyTo(m_Stream);
		}
	}

	public override bool CanRead => m_Stream.CanRead;

	public override bool CanSeek => m_Stream.CanSeek;

	public override bool CanWrite => m_Stream.CanWrite;

	public override long Length => m_Stream.Length;

	public override long Position
	{
		get => m_Stream.Position;
		set => m_Stream.Position = value;
	}

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

	public override void Flush()
	{
		m_Stream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return m_Stream.Read(buffer, offset, count);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return m_Stream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		m_Stream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		m_Stream.Write(buffer, offset, count);
	}
}
