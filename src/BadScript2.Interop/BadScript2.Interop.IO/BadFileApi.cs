using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.IO;

[BadInteropApi("File", true)]
internal partial class BadFileApi
{
    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem m_FileSystem = BadFileSystem.Instance;

    public BadFileApi(IFileSystem fileSystem) : this()
    {
        m_FileSystem = fileSystem;
    }

    [BadMethod(description: "Writes the specified string to a file, overwriting the file if it already exists.")]
    private void WriteAllText([BadParameter(description: "The Path of the file to write")] string path,
                              [BadParameter(description: "The Content")] string content)
    {
        m_FileSystem.WriteAllText(path, content);
    }

    [BadMethod(description: "Opens a text file, reads all content of the file, and then closes the file.")]
    [return: BadReturn("The content of the file")]
    private string ReadAllText([BadParameter(description: "The Path of the file to read")] string path)
    {
        return m_FileSystem.ReadAllText(path);
    }

    [BadMethod(description: "Determines whether the specified file exists.")]
    [return:
        BadReturn("True if the caller has the required permissions and path contains the name of an existing file; otherwise, false."
                 )]
    private bool Exists([BadParameter(description: "The Path to check")] string path)
    {
        return m_FileSystem.Exists(path) && m_FileSystem.IsFile(path);
    }

    [BadMethod(description: "Opens a file, reads all lines of the file, and then closes the file.")]
    [return: BadReturn("The content of the file")]
    private BadArray ReadAllLines([BadParameter(description: "The Path of the file to read")] string path)
    {
        return new BadArray(m_FileSystem.ReadAllLines(path)
                                        .Select(x => (BadObject)x)
                                        .ToList()
                           );
    }

    [BadMethod(description: "Opens a file, writes all lines to the file, and then closes the file.")]
    private void WriteAllLines([BadParameter(description: "The Path of the file to write")] string path,
                               [BadParameter(description: "The Content")] string[] content)
    {
        m_FileSystem.WriteAllLines(path, content);
    }

    [BadMethod(description: "Opens a file, writes all bytes to the file, and then closes the file.")]
    private void WriteAllBytes([BadParameter(description: "The Path of the file to write")] string path,
                               [BadParameter(description: "The Content")] byte[] content)
    {
        using Stream stream = m_FileSystem.OpenWrite(path, BadWriteMode.CreateNew);
        stream.Write(content, 0, content.Length);
    }

    [BadMethod(description: "Opens a file, reads all bytes of the file, and then closes the file.")]
    [return: BadReturn("The content of the file")]
    private BadArray ReadAllBytes([BadParameter(description: "The Path of the file to read")] string path)
    {
        using Stream stream = m_FileSystem.OpenRead(path);
        byte[] bytes = new byte[stream.Length];
        int read = stream.Read(bytes, 0, bytes.Length);

        if (read != bytes.Length)
        {
            throw new BadRuntimeException("IO.File.ReadAllBytes: Could not read all bytes");
        }

        return new BadArray(bytes.Select(x => (BadObject)x)
                                 .ToList()
                           );
    }

    [BadMethod(description: "Deletes the specified file.")]
    private void Delete([BadParameter(description: "The Path of the file to delete")] string path)
    {
        m_FileSystem.DeleteFile(path);
    }

    [BadMethod(description: "Moves a specified file to a new location, providing the option to specify a new file name."
              )]
    private void Move([BadParameter(description: "The Path of the file to move")] string source,
                      [BadParameter(description: "The Destination Path")]
                      string destination,
                      [BadParameter(description: "If true, allows an existing file to be overwritten; otherwise, false."
                                   )]
                      bool overwrite = false)
    {
        m_FileSystem.Move(source, destination, overwrite);
    }

    [BadMethod(description:
                  "Copies a specified file to a new location, providing the option to specify a new file name."
              )]
    private void Copy([BadParameter(description: "The Path of the file to copy")] string source,
                      [BadParameter(description: "The Destination Path")]
                      string destination,
                      [BadParameter(description: "If true, allows an existing file to be overwritten; otherwise, false."
                                   )]
                      bool overwrite = false)
    {
        m_FileSystem.Copy(source, destination, overwrite);
    }
}