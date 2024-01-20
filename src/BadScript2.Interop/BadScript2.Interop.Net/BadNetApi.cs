using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Net;

/// <summary>
///     Implements the "Net" Api
/// </summary>
public class BadNetApi : BadInteropApi
{
	/// <summary>
	///     Public Constructor
	/// </summary>
	public BadNetApi() : base("Net") { }

    protected override void LoadApi(BadTable target)
    {
        target.SetFunction<string>("Get", Get, BadTask.Prototype);
        target.SetFunction<string, string>("Post", Post, BadTask.Prototype);
        target.SetFunction<string>(
            "EncodeUriComponent",
            s => Uri.EscapeDataString(s),
            BadNativeClassBuilder.GetNative("string")
        );
        target.SetFunction<string>(
            "DecodeUriComponent",
            s => Uri.UnescapeDataString(s),
            BadNativeClassBuilder.GetNative("string")
        );
    }


    /// <summary>
    ///     Creates a new BadTask that performs a POST request to the given url with the given content
    /// </summary>
    /// <param name="url">Url</param>
    /// <param name="content">Body</param>
    /// <returns>Awaitable Task</returns>
    private static BadTask Post(string url, string content)
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
    private static BadTask Get(string url)
    {
        HttpClient cl = new HttpClient();
        Task<HttpResponseMessage>? task = cl.GetAsync(url);

        return new BadTask(BadTaskUtils.WaitForTask(task), $"Net.Get(\"{url}\")");
    }
}