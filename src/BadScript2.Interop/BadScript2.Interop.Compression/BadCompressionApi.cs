using BadScript2.IO;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

///<summary>
///	Contains Compression Extensions and APIs for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.Compression;

/// <summary>
///     Implements the "Compression" API
/// </summary>
[BadInteropApi("Compression", true)]
internal partial class BadCompressionApi
{
    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem? m_FileSystem;


    /// <summary>
    ///     Creates a new API Instance
    /// </summary>
    /// <param name="fileSystem">File System Instance to use</param>
    public BadCompressionApi(IFileSystem fileSystem) : this()
    {
        m_FileSystem = fileSystem;
    }


    protected override void AdditionalData(BadTable target)
    {
        BadTable zip = new BadTable();
        BadTable gzip = new BadTable();
        BadTable deflate = new BadTable();
        BadTable zlib = new BadTable();
        BadTable base64 = new BadTable();
        target.SetProperty("Zip", zip);
        target.SetProperty("GZip", gzip);
        target.SetProperty("Deflate", deflate);
        target.SetProperty("ZLib", zlib);
        target.SetProperty("Base64", base64);
        new BadZipApi(m_FileSystem ?? BadFileSystem.Instance).LoadRawApi(zip);
        new BadGZipApi().LoadRawApi(gzip);
        new BadDeflateApi().LoadRawApi(deflate);
        new BadZLibApi().LoadRawApi(zlib);
        new BadBase64Api().LoadRawApi(base64);
    }
}