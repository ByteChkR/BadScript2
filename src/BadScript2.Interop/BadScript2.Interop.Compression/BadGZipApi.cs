using System.IO.Compression;
using System.Text;

using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Compression;

[BadInteropApi("GZip")]
internal partial class BadGZipApi
{
    /// <summary>
    ///     GZip Compress the given string
    /// </summary>
    /// <param name="obj">String</param>
    /// <returns>Compressed Array</returns>
    [BadMethod(description:"Compresses the given string using the GZip Algorithm")]
    [return: BadReturn("Compressed Array of bytes")]
    private static BadArray Compress([BadParameter(description:"String to Compress")]string obj)
    {
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(obj));
        MemoryStream compressed = new MemoryStream();
        using GZipStream ds = new GZipStream(compressed, CompressionMode.Compress);
        ms.CopyTo(ds);
        ds.Close();

        return new BadArray(compressed.ToArray().Select(x => (BadObject)new BadNumber(x)).ToList());
    }

    /// <summary>
    ///     GZip Decompress the given array
    /// </summary>
    /// <param name="ctx">The Current Calling Execution Context</param>
    /// <param name="obj">Array</param>
    /// <returns>String</returns>
    [BadMethod(description:"Inflates the given array of bytes using the GZip Algorithm")]
    [return: BadReturn("Decompressed String")]
    private static BadObject Decompress(BadExecutionContext ctx, [BadParameter(description:"Bytes to Decompress")]byte[] obj)
    {
        BadObject[] arr;
        MemoryStream ms = new MemoryStream(obj);
        MemoryStream decompressed = new MemoryStream();
        using GZipStream ds = new GZipStream(ms, CompressionMode.Decompress);
        ds.CopyTo(decompressed);
        ds.Close();

        return new BadString(Encoding.UTF8.GetString(decompressed.ToArray()));
    }
}