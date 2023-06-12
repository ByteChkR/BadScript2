using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.IO;

/// <summary>
///     Implements the "IO" Api
/// </summary>
public class BadIOApi : BadInteropApi
{
	/// <summary>
	///     The FileSystem Instance
	/// </summary>
	private readonly IFileSystem m_FileSystem;

	/// <summary>
	///     Creates a new API Instance
	/// </summary>
	public BadIOApi() : this(BadFileSystem.Instance) { }

	/// <summary>
	///     Creates a new API Instance
	/// </summary>
	/// <param name="fileSystem">File System Instance</param>
	public BadIOApi(IFileSystem fileSystem) : base("IO")
	{
		m_FileSystem = fileSystem;
	}

	/// <summary>
	///     Creates the "Path" Table
	/// </summary>
	/// <returns>Table</returns>
	private BadTable CreatePath()
	{
		BadTable t = new BadTable();

		t.SetFunction<string>("GetFileName", (_, s) => Path.GetFileName(s));
		t.SetFunction<string>("GetFileNameWithoutExtension", (_, s) => Path.GetFileNameWithoutExtension(s));
		t.SetFunction<string>("GetDirectoryName", (_, s) => Path.GetDirectoryName(s) ?? BadObject.Null);
		t.SetFunction<string>("GetExtension", (_, s) => Path.GetExtension(s));
		t.SetFunction<string>("GetFullPath", (_, s) => m_FileSystem.GetFullPath(s));
		t.SetFunction<string>("GetStartupPath", (_, _) => m_FileSystem.GetStartupDirectory());
		t.SetFunction<string, string>("ChangeExtension", (_, s, ext) => Path.ChangeExtension(s, ext));
		t.SetProperty("Combine",
			new BadInteropFunction("Combine", Combine, new BadFunctionParameter("parts", false, false, true)));

		return t;
	}

	/// <summary>
	///     Combines the given paths
	/// </summary>
	/// <param name="arg">Paths</param>
	/// <returns>Combined Path String</returns>
	private BadObject Combine(BadObject[] arg)
	{
		return Path.Combine(arg.Select(x => x.ToString()!).ToArray());
	}

	/// <summary>
	///     Creates the "Directory" Table
	/// </summary>
	/// <returns>Table</returns>
	private BadTable CreateDirectory()
	{
		BadTable t = new BadTable();

		t.SetFunction<string>("CreateDirectory",
			(_, s) =>
			{
				m_FileSystem.CreateDirectory(s);

				return BadObject.Null;
			});

		t.SetFunction<string>("Exists", s => m_FileSystem.Exists(s) && m_FileSystem.IsDirectory(s));
		t.SetFunction<string, bool>("Delete",
			m_FileSystem.DeleteDirectory);


		t.SetFunction<string, string, bool>("Move",
			m_FileSystem.Move);

		t.SetFunction("GetCurrentDirectory", () => m_FileSystem.GetCurrentDirectory());
		t.SetFunction<string>("SetCurrentDirectory", m_FileSystem.SetCurrentDirectory);
		t.SetFunction("GetStartupDirectory", () => m_FileSystem.GetStartupDirectory());

		t.SetFunction<string, bool>("GetDirectories",
			(_, s, b) => new BadArray(m_FileSystem.GetDirectories(s, b)
				.Select(x => (BadObject)x)
				.ToList()));

		t.SetFunction<string, string, bool>("GetFiles",
			(_, s, p, b) => new BadArray(m_FileSystem.GetFiles(s, p, b)
				.Select(x => (BadObject)x)
				.ToList()));

		return t;
	}

	/// <summary>
	///     Creates the "File" Table
	/// </summary>
	/// <returns>Table</returns>
	private BadTable CreateFile()
	{
		BadTable t = new BadTable();

		t.SetFunction<string, string>("WriteAllText",
			m_FileSystem.WriteAllText);
		t.SetFunction<string>("ReadAllText",
			s => m_FileSystem.ReadAllText(s));
		t.SetFunction<string>("Exists",
			s => m_FileSystem.Exists(s));
		t.SetFunction<string>("ReadAllLines",
			s => new BadArray(m_FileSystem.ReadAllLines(s)
				.Select(x => (BadObject)x)
				.ToList()));
		t.SetFunction<string, BadArray>("WriteAllLines",
			(s, a) =>
			{
				m_FileSystem.WriteAllLines(s, a.InnerArray.Select(x => x.ToString()!));

				return BadObject.Null;
			});


		t.SetFunction<string, BadArray>("WriteAllBytes",
			(s, a) =>
			{
				using Stream stream = m_FileSystem.OpenWrite(s, BadWriteMode.CreateNew);

				foreach (BadObject o in a.InnerArray)
				{
					if (o is not IBadNumber num)
					{
						throw new BadRuntimeException("BadIO.WriteAllBytes: Array contains non-number");
					}

					stream.WriteByte((byte)num.Value);
				}
			});

		t.SetFunction<string>("ReadAllBytes",
			s =>
			{
				using Stream stream = m_FileSystem.OpenRead(s);
				byte[] bytes = new byte[stream.Length];
				stream.Read(bytes, 0, bytes.Length);

				return new BadArray(bytes.Select(x => (BadObject)new BadNumber(x)).ToList());
			});

		t.SetFunction<string>("Delete", m_FileSystem.DeleteFile);

		t.SetFunction<string, string>("Copy",
			(i, o) =>
			{
				using Stream inS = m_FileSystem.OpenRead(i);
				using Stream outS = m_FileSystem.OpenWrite(o, BadWriteMode.CreateNew);
				inS.CopyTo(outS);
			});

		return t;
	}

	protected override void LoadApi(BadTable target)
	{
		target.SetProperty("Path", CreatePath());
		target.SetProperty("Directory", CreateDirectory());
		target.SetProperty("File", CreateFile());
	}
}
