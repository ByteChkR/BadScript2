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
[BadInteropApi("IO")]
internal partial class BadIOApi
{
    private readonly BadDirectoryApi m_DirectoryApi;
    private readonly BadFileApi m_FileApi;

    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem m_FileSystem;

    private readonly BadPathApi m_PathApi;


    /// <summary>
    ///     Creates a new API Instance
    /// </summary>
    /// <param name="fileSystem">File System Instance</param>
    public BadIOApi(IFileSystem fileSystem) : this()
    {
        m_FileSystem = fileSystem;
        m_FileApi = new BadFileApi(fileSystem);
        m_PathApi = new BadPathApi(fileSystem);
        m_DirectoryApi = new BadDirectoryApi(fileSystem);
    }

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