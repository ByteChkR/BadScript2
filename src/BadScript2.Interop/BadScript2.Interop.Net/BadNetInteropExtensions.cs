using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Net;

public class BadNetInteropExtensions : BadInteropExtension
{
    protected override void AddExtensions()
    {
        RegisterObject<HttpResponseMessage>("Status", resp => (decimal)resp.StatusCode);
        RegisterObject<HttpResponseMessage>("Reason", resp => resp.ReasonPhrase ?? "");
        RegisterObject<HttpResponseMessage>(
            "Headers",
            resp =>
            {
                Dictionary<BadObject, BadObject> v = resp.Headers.ToDictionary(
                    x => (BadObject)x.Key,
                    x => (BadObject)new BadArray(x.Value.Select(y => (BadObject)y).ToList())
                );

                return new BadTable(v);
            }
        );
        RegisterObject<HttpResponseMessage>("Content", resp => BadObject.Wrap(resp.Content));

        RegisterObject<HttpContent>(
            "ReadAsString",
            c => new BadDynamicInteropFunction(
                "ReadAsString",
                ctx => Content_ReadAsString(c)
            )
        );
        RegisterObject<HttpContent>(
            "ReadAsArray",
            c => new BadDynamicInteropFunction(
                "ReadAsArray",
                ctx => Content_ReadAsArray(c)
            )
        );
    }

    private BadTask Content_ReadAsString(HttpContent content)
    {
        Task<string> task = content.ReadAsStringAsync();

        return new BadTask(BadTaskUtils.WaitForTask(task), "HttpContent.ReadAsString");
    }

    private BadTask Content_ReadAsArray(HttpContent content)
    {
        Task<byte[]> task = content.ReadAsByteArrayAsync();

        return new BadTask(
            BadTaskUtils.WaitForTask(
                task,
                o => new BadArray(o.Select(x => (BadObject)(decimal)x).ToList())
            ),
            "HttpContent.ReadAsArray"
        );
    }
}