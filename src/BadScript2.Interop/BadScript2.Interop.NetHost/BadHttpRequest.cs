using System.Collections.Generic;
using System.Net;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.NetHost;

public class BadHttpRequest : BadObject
{
	private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadHttpContext>("HttpRequest",
		(_, _) => throw new BadRuntimeException("Cannot create new Http Request"));

	public BadHttpRequest(HttpListenerRequest request)
	{
		Request = request;
	}

	public HttpListenerRequest Request { get; }

	public override BadClassPrototype GetPrototype()
	{
		return s_Prototype;
	}

	public override string ToSafeString(List<BadObject> done)
	{
		return Request.ToString();
	}
}
