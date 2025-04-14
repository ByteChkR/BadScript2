namespace BadScript2.IO;

/// <summary>
///     Implements a wrapper for the actual OS file system
/// </summary>
public class BadSystemFileSystem : IFileSystem
{
#region IFileSystem Members

/// <inheritdoc />
    public string GetStartupDirectory()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    /// <inheritdoc />
    public bool Exists(string path)
    {
        return Directory.Exists(path) || File.Exists(path);
    }

    /// <inheritdoc />
    public bool IsFile(string path)
    {
        return File.Exists(path);
    }

    /// <inheritdoc />
    public bool IsDirectory(string path)
    {
        return Directory.Exists(path);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
    {
        return Directory.GetFiles(path,
                                  $"*{extension}",
                                  recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                                 );
    }

    /// <inheritdoc />
    public IEnumerable<string> GetDirectories(string path, bool recursive)
    {
        return Directory.GetDirectories(path,
                                        "*",
                                        recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                                       );
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void DeleteDirectory(string path, bool recursive)
    {
        Directory.Delete(path, recursive);
    }

    /// <inheritdoc />
    public void DeleteFile(string path)
    {
        File.Delete(path);
    }

    /// <inheritdoc />
    public string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    /// <inheritdoc />
    public Stream OpenRead(string path)
    {
        return File.OpenRead(path);
    }

    /// <inheritdoc />
    public Stream OpenWrite(string path, BadWriteMode mode)
    {
        FileMode fileMode = mode switch
        {
            BadWriteMode.CreateNew => FileMode.Create,
            BadWriteMode.Append    => FileMode.Append,
            _                      => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };

        return File.Open(path, fileMode);
    }

    /// <inheritdoc />
    public string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }

    /// <inheritdoc />
    public void SetCurrentDirectory(string path)
    {
        Directory.SetCurrentDirectory(path);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

#endregion
}