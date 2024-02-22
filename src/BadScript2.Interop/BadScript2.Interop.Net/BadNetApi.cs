using BadScript2.Interop.Common.Task;

///<summary>
///	Contains Networking Extensions and APIs for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.Net;

/// <summary>
///     Implements the "Net" Api
/// </summary>
[BadInteropApi("Net")]
internal partial class BadNetApi
{
    [BadMethod(description: "Encodes a URI Component")]
    [return: BadReturn("The encoded URI Component")]
    private string EncodeUriComponent([BadParameter(description: "The component to encode")] string s)
    {
        return Uri.EscapeDataString(s);
    }

    [BadMethod(description: "Decodes a URI Component")]
    [return: BadReturn("The decoded URI Component")]
    private string DecodeUriComponent([BadParameter(description: "The component to decode")] string s)
    {
        return Uri.UnescapeDataString(s);
    }


    /// <summary>
    ///     Creates a new BadTask that performs a POST request to the given url with the given content
    /// </summary>
    /// <param name="url">Url</param>
    /// <param name="content">Body</param>
    /// <returns>Awaitable Task</returns>
    [BadMethod(description: "Performs a POST request to the given url with the given content")]
    [return: BadReturn("The Awaitable Task")]
    private static BadTask Post(
        [BadParameter(description: "The URL of the POST request")] string url,
        [BadParameter(description: "The String content of the post request")] string content)
    {
        HttpClient cl = new HttpClient();

        return new BadTask(
            BadTaskUtils.WaitForTask(cl.PostAsync(url, new StringContent(content))),
            $"Net.Post(\"{url}\")"
        );
    }

    /// <summary>
    ///     Creates a new BadTask that performs a GET request to the given url
    /// </summary>
    /// <param name="url">Url</param>
    /// <returns>Awaitable Task</returns>
    [BadMethod(description: "Performs a GET request to the given url")]
    [return: BadReturn("The Awaitable Task")]
    private static BadTask Get([BadParameter(description: "The URL of the GET request")] string url)
    {
        HttpClient cl = new HttpClient();
        Task<HttpResponseMessage>? task = cl.GetAsync(url);

        return new BadTask(BadTaskUtils.WaitForTask(task), $"Net.Get(\"{url}\")");
    }
}