using BadScript2.IO;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

///<summary>
///	Contains IO Extensions and APIs for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.IO;

/// <summary>
///     Implements the "IO" Api
/// </summary>
[BadInteropApi("IO", true)]
internal partial class BadIOApi
{
    /// <summary>
    /// Directory Api Implementation
    /// </summary>
    private readonly BadDirectoryApi m_DirectoryApi;
    /// <summary>
    /// File Api Implementation
    /// </summary>
    private readonly BadFileApi m_FileApi;
    /// <summary>
    /// Path Api Implementation
    /// </summary>
    private readonly BadPathApi m_PathApi;


    /// <summary>
    ///     Creates a new API Instance
    /// </summary>
    /// <param name="fileSystem">File System Instance</param>
    public BadIOApi(IFileSystem fileSystem) : this()
    {
        m_FileApi = new BadFileApi(fileSystem);
        m_PathApi = new BadPathApi(fileSystem);
        m_DirectoryApi = new BadDirectoryApi(fileSystem);
    }

    /// <inheritdoc />
    protected override void AdditionalData(BadTable target)
    {
        BadTable file = new BadTable();
        BadTable path = new BadTable();
        BadTable directory = new BadTable();
        m_FileApi.LoadRawApi(file);
        m_PathApi.LoadRawApi(path);
        m_DirectoryApi.LoadRawApi(directory);
        target.SetProperty("File", file);
        target.SetProperty("Path", path);
        target.SetProperty("Directory", directory);
    }
}