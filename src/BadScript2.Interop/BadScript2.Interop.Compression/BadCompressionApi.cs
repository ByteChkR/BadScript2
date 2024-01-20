using System.IO.Compression;
using System.Text;

using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Compression;

/// <summary>
///     Implements the "Compression" API
/// </summary>
public class BadCompressionApi : BadInteropApi
{
    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem m_FileSystem;

    /// <summary>
    ///     Creates a new API Instance
    /// </summary>
    public BadCompressionApi() : this(BadFileSystem.Instance) { }

    /// <summary>
    ///     Creates a new API Instance
    /// </summary>
    /// <param name="fileSystem">File System Instance to use</param>
    public BadCompressionApi(IFileSystem fileSystem) : base("Compression")
    {
        m_FileSystem = fileSystem;
    }


    /// <summary>
    ///     Deflates the given string
    /// </summary>
    /// <param name="obj">String</param>
    /// <returns>Compressed Array</returns>
    private static BadObject Deflate(IBadString obj)
    {
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(obj.Value));
        MemoryStream compressed = new MemoryStream();
        using DeflateStream ds = new DeflateStream(compressed, CompressionMode.Compress);
        ms.CopyTo(ds);
        ds.Close();

        return new BadArray(compressed.ToArray().Select(x => (BadObject)new BadNumber(x)).ToList());
    }

    /// <summary>
    ///     Inflate the given array
    /// </summary>
    /// <param name="obj">Array</param>
    /// <returns>String</returns>
    private static BadObject Inflate(BadExecutionContext ctx, BadObject obj)
    {
        BadObject[] arr;
        if (obj is BadArray a)
        {
            arr = a.InnerArray.ToArray();
        }
        else if (BadNativeClassBuilder.Enumerable.IsSuperClassOf(obj.GetPrototype()))
        {
            arr = BadNativeClassHelper.ExecuteEnumerate(ctx, obj).ToArray();
        }
        else
        {
            throw new BadRuntimeException("Argument is not of type IEnumerable");
        }

        MemoryStream ms = new MemoryStream(arr.Select(x => (byte)((BadNumber)x).Value).ToArray());
        MemoryStream decompressed = new MemoryStream();
        using DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress);
        ds.CopyTo(decompressed);
        ds.Close();

        return new BadString(Encoding.UTF8.GetString(decompressed.ToArray()));
    }

    /// <summary>
    ///     GZip Compress the given string
    /// </summary>
    /// <param name="obj">String</param>
    /// <returns>Compressed Array</returns>
    private static BadObject GZipCompress(IBadString obj)
    {
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(obj.Value));
        MemoryStream compressed = new MemoryStream();
        using GZipStream ds = new GZipStream(compressed, CompressionMode.Compress);
        ms.CopyTo(ds);
        ds.Close();

        return new BadArray(compressed.ToArray().Select(x => (BadObject)new BadNumber(x)).ToList());
    }

    /// <summary>
    ///     GZip Decompress the given array
    /// </summary>
    /// <param name="obj">Array</param>
    /// <returns>String</returns>
    private static BadObject GZipDecompress(BadExecutionContext ctx, BadObject obj)
    {
        BadObject[] arr;
        if (obj is BadArray a)
        {
            arr = a.InnerArray.ToArray();
        }
        else if (BadNativeClassBuilder.Enumerable.IsSuperClassOf(obj.GetPrototype()))
        {
            arr = BadNativeClassHelper.ExecuteEnumerate(ctx, obj).ToArray();
        }
        else
        {
            throw new BadRuntimeException("Argument is not of type IEnumerable");
        }

        MemoryStream ms = new MemoryStream(arr.Select(x => (byte)((BadNumber)x).Value).ToArray());
        MemoryStream decompressed = new MemoryStream();
        using GZipStream ds = new GZipStream(ms, CompressionMode.Decompress);
        ds.CopyTo(decompressed);
        ds.Close();

        return new BadString(Encoding.UTF8.GetString(decompressed.ToArray()));
    }

    /// <summary>
    ///     ZLib Compress the given string
    /// </summary>
    /// <param name="obj">String</param>
    /// <returns>Compressed Array</returns>
    private static BadObject ZLibCompress(IBadString obj)
    {
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(obj.Value));
        MemoryStream compressed = new MemoryStream();
        using ZLibStream ds = new ZLibStream(compressed, CompressionMode.Compress);
        ms.CopyTo(ds);
        ds.Close();

        return new BadArray(compressed.ToArray().Select(x => (BadObject)new BadNumber(x)).ToList());
    }

    /// <summary>
    ///     ZLib Decompress the given array
    /// </summary>
    /// <param name="obj">Array</param>
    /// <returns>String</returns>
    private static BadObject ZLibDecompress(BadExecutionContext ctx, BadObject obj)
    {
        BadObject[] arr;
        if (obj is BadArray a)
        {
            arr = a.InnerArray.ToArray();
        }
        else if (BadNativeClassBuilder.Enumerable.IsSuperClassOf(obj.GetPrototype()))
        {
            arr = BadNativeClassHelper.ExecuteEnumerate(ctx, obj).ToArray();
        }
        else
        {
            throw new BadRuntimeException("Argument is not of type IEnumerable");
        }

        MemoryStream ms = new MemoryStream(arr.Select(x => (byte)((BadNumber)x).Value).ToArray());
        MemoryStream decompressed = new MemoryStream();
        using ZLibStream ds = new ZLibStream(ms, CompressionMode.Decompress);
        ds.CopyTo(decompressed);
        ds.Close();

        return new BadString(Encoding.UTF8.GetString(decompressed.ToArray()));
    }

    protected override void LoadApi(BadTable target)
    {
        BadTable deflate = new BadTable();
        deflate.SetFunction<IBadString>("Compress", Deflate, BadNativeClassBuilder.GetNative("Array"));
        deflate.SetFunction<BadObject>("Decompress", Inflate, BadNativeClassBuilder.GetNative("string"));
        target.SetProperty("Deflate", deflate);

        BadTable gzip = new BadTable();
        gzip.SetFunction<IBadString>("Compress", GZipCompress, BadNativeClassBuilder.GetNative("Array"));
        gzip.SetFunction<BadObject>("Decompress", GZipDecompress, BadNativeClassBuilder.GetNative("string"));
        target.SetProperty("GZip", gzip);

        BadTable zlib = new BadTable();
        zlib.SetFunction<IBadString>("Compress", ZLibCompress, BadNativeClassBuilder.GetNative("Array"));
        zlib.SetFunction<BadObject>("Decompress", ZLibDecompress, BadNativeClassBuilder.GetNative("string"));
        target.SetProperty("ZLib", zlib);

        BadTable zip = new BadTable();
        zip.SetFunction<string, string>("FromDirectory", FromDirectoryApi);
        zip.SetFunction<string, string>("ToDirectory", ToDirectoryApi);
        target.SetProperty("Zip", zip);

        BadTable base64 = new BadTable();
        base64.SetFunction<BadObject>("Encode", Base64Encode, BadNativeClassBuilder.GetNative("string"));
        base64.SetFunction<IBadString>("Decode", Base64Decode, BadNativeClassBuilder.GetNative("Array"));
        target.SetProperty("Base64", base64);
    }

    private static BadObject Base64Encode(BadExecutionContext ctx, BadObject obj)
    {
        BadObject[] array;
        if (obj is BadArray a)
        {
            array = a.InnerArray.ToArray();
        }
        else if (BadNativeClassBuilder.Enumerable.IsSuperClassOf(obj.GetPrototype()))
        {
            array = BadNativeClassHelper.ExecuteEnumerate(ctx, obj).ToArray();
        }
        else
        {
            throw new BadRuntimeException("Argument is not of type IEnumerable");
        }

        return Convert.ToBase64String(array.Cast<IBadNumber>().Select(x => (byte)x.Value).ToArray());
    }

    private static BadObject Base64Decode(IBadString str)
    {
        return new BadArray(Convert.FromBase64String(str.Value).Select(x => (BadObject)(decimal)x).ToList());
    }

    /// <summary>
    ///     Decompresses the given file to the given output directory
    /// </summary>
    /// <param name="outputDir">Output Directory</param>
    /// <param name="inputFile">Input File</param>
    private void ToDirectoryApi(string outputDir, string inputFile)
    {
        using Stream s = m_FileSystem.OpenRead(inputFile);
        ToDirectory(outputDir, s);
    }

    /// <summary>
    ///     Compreses the given directory to the given output file
    /// </summary>
    /// <param name="inputDir">Input Directory</param>
    /// <param name="outputFile">Output File</param>
    private void FromDirectoryApi(string inputDir, string outputFile)
    {
        using Stream s = m_FileSystem.OpenWrite(outputFile, BadWriteMode.CreateNew);
        FromDirectory(inputDir, s);
    }

    /// <summary>
    ///     Decompresses the given stream to the given output directory
    /// </summary>
    /// <param name="outputDir">directory</param>
    /// <param name="input">Input Stream</param>
    private void ToDirectory(string outputDir, Stream input)
    {
        using ZipArchive archive = new ZipArchive(input, ZipArchiveMode.Read);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string outputFile = Path.Combine(outputDir, entry.FullName);
            string outputParent = Path.GetDirectoryName(outputFile)!;
            m_FileSystem.CreateDirectory(outputParent, true);
            using Stream s = m_FileSystem.OpenWrite(outputFile, BadWriteMode.CreateNew);
            using Stream e = entry.Open();
            e.CopyTo(s);
        }
    }

    /// <summary>
    ///     Compreses the given directory to the given output stream
    /// </summary>
    /// <param name="inputDir">Input Directory</param>
    /// <param name="output">Stream</param>
    private void FromDirectory(string inputDir, Stream output)
    {
        using ZipArchive archive = new ZipArchive(output, ZipArchiveMode.Create);
        string[] files = m_FileSystem.GetFiles(inputDir, "", true).ToArray();

        foreach (string file in files)
        {
            string zipPath = file.Remove(0, inputDir.Length + 1).Replace('\\', '/');
            ZipArchiveEntry entry = archive.CreateEntry(zipPath);
            using Stream es = entry.Open();
            using Stream s = m_FileSystem.OpenRead(file);
            s.CopyTo(es);
        }
    }
}