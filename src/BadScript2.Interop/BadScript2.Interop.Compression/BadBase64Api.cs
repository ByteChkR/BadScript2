using BadScript2.Runtime.Objects;
namespace BadScript2.Interop.Compression;

[BadInteropApi("Base64")]
internal partial class BadBase64Api
{
    /// <summary>
    ///     Encodes the given object to a base64 string
    /// </summary>
    /// <param name="ctx">The Current Calling Execution Context</param>
    /// <param name="obj">Object</param>
    /// <returns>Base64 String</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the argument is not of type IEnumerable</exception>
    [BadMethod(description: "Encodes the given string to a base64 string")]
    [return: BadReturn("Base64 String")]
    private static string Encode([BadParameter(description: "Bytes to Encode")] byte[] obj)
    {
        return Convert.ToBase64String(obj);
    }

    /// <summary>
    ///     Decodes the given base64 string to an array
    /// </summary>
    /// <param name="str">Base64 String</param>
    /// <returns>Array</returns>
    [BadMethod(description: "Decodes a base64 string to an array of bytes")]
    [return: BadReturn("Bytes")]
    private static BadArray Decode([BadParameter(description: "String to Decode")] string str)
    {
        return new BadArray(Convert.FromBase64String(str).Select(x => (BadObject)(decimal)x).ToList());
    }
}