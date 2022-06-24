using BadScript2.Interop.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

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
            c => new BadDynamicInteropFunction<BadFunction>(
                "ReadAsString",
                (ctx, func) => Content_ReadAsString(ctx, c, func)
            )
        );
        RegisterObject<HttpContent>(
            "ReadAsArray",
            c => new BadDynamicInteropFunction<BadFunction>(
                "ReadAsArray",
                (ctx, func) => Content_ReadAsArray(ctx, c, func)
            )
        );

        //HttpResponseHeaders
    }

    private BadTask Content_ReadAsString(BadExecutionContext context, HttpContent content, BadFunction onComplete)
    {
        Task<string> task = content.ReadAsStringAsync();

        return new BadTask(BadNetApi.WaitForTask(context, task, onComplete), "HttpContent.ReadAsString");
    }

    private BadTask Content_ReadAsArray(BadExecutionContext context, HttpContent content, BadFunction onComplete)
    {
        Task<byte[]> task = content.ReadAsByteArrayAsync();

        return new BadTask(
            BadNetApi.WaitForTask(
                task,
                o => onComplete.Invoke(
                    new BadObject[] { new BadArray(o.Select(x => (BadObject)(decimal)x).ToList()) },
                    context
                )
            ),
            "HttpContent.ReadAsArray"
        );
    }
}