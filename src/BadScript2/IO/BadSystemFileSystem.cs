namespace BadScript2.IO;

/// <summary>
///     Implements a wrapper for the actual OS file system
/// </summary>
public class BadSystemFileSystem : IFileSystem
{
	public string GetStartupDirectory()
	{
		return AppDomain.CurrentDomain.BaseDirectory;
	}

	public bool Exists(string path)
	{
		return Directory.Exists(path) || File.Exists(path);
	}

	public bool IsFile(string path)
	{
		return File.Exists(path);
	}

	public bool IsDirectory(string path)
	{
		return Directory.Exists(path);
	}

	public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
	{
		return Directory.GetFiles(path,
			$"*{extension}",
			recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
	}

	public IEnumerable<string> GetDirectories(string path, bool recursive)
	{
		return Directory.GetDirectories(path,
			"*",
			recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
	}

	public void CreateDirectory(string path, bool recursive = false)
	{
		List<string> directories = new List<string>();
		string current = GetFullPath(path);

		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		while (current != null)
		{
			directories.Add(current);
			current = Path.GetDirectoryName(current)!;
		}

		for (int i = directories.Count - 1; i >= 0; i--)
		{
			if (!Directory.Exists(directories[i]))
			{
				Directory.CreateDirectory(directories[i]);
			}
		}
	}

	public void DeleteDirectory(string path, bool recursive)
	{
		Directory.Delete(path, recursive);
	}

	public void DeleteFile(string path)
	{
		File.Delete(path);
	}

	public string GetFullPath(string path)
	{
		return Path.GetFullPath(path);
	}

	public Stream OpenRead(string path)
	{
		return File.OpenRead(path);
	}

	public Stream OpenWrite(string path, BadWriteMode mode)
	{
		FileMode fileMode;

		switch (mode)
		{
			case BadWriteMode.CreateNew:
				fileMode = FileMode.Create;

				break;
			case BadWriteMode.Append:
				fileMode = FileMode.Append;

				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
		}

		return File.Open(path, fileMode);
	}

	public string GetCurrentDirectory()
	{
		return Directory.GetCurrentDirectory();
	}

	public void SetCurrentDirectory(string path)
	{
		Directory.SetCurrentDirectory(path);
	}

	public void Copy(string src, string dst, bool overwrite = true)
	{
		if (File.Exists(src))
		{
			if (!overwrite && File.Exists(dst))
			{
				throw new IOException("File already exists");
			}

			File.Copy(src, dst, overwrite);
		}
		else if (Directory.Exists(src))
		{
			if (!overwrite && Directory.Exists(dst))
			{
				throw new IOException("Directory already exists");
			}

			foreach (string directory in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
			{
				Directory.CreateDirectory(directory.Replace(src, dst));
			}

			foreach (string file in Directory.GetFiles(src, "*", SearchOption.AllDirectories))
			{
				File.Copy(file, file.Replace(src, dst), overwrite);
			}
		}
	}

	public void Move(string src, string dst, bool overwrite = true)
	{
		if (File.Exists(src))
		{
			if (!overwrite && File.Exists(dst))
			{
				throw new IOException("File already exists");
			}

			File.Move(src, dst);
		}
		else if (Directory.Exists(src))
		{
			if (!overwrite && Directory.Exists(dst))
			{
				throw new IOException("Directory already exists");
			}

			Directory.Move(src, dst);
		}
	}
}
