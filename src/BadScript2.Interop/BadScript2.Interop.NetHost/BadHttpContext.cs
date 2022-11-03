using System.Collections.Generic;
using System.Net;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.NetHost;

public class BadHttpContext : BadObject
{
    private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadHttpContext>(
        "HttpContext",
        (_, _) => throw new BadRuntimeException("Cannot create new Http Contexts")
    );
    public readonly HttpListenerContext Context;
    public BadHttpContext(HttpListenerContext context)
    {
        Context = context;
    }

    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return Context.ToString();
    }
}