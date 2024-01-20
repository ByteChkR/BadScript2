using System.Collections.Generic;
using System.Net;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.NetHost;

/// <summary>
///     Wrapper for the HttpListenerRequest
/// </summary>
public class BadHttpRequest : BadObject
{
	/// <summary>
	///     Class Prototype Instance
	/// </summary>
	private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadHttpContext>(
        "HttpRequest",
        (_, _) => throw new BadRuntimeException("Cannot create new Http Request")
    );

	/// <summary>
	///     Constructs a new BadHttpRequest
	/// </summary>
	/// <param name="request">The underlying HttpListenerRequest</param>
	public BadHttpRequest(HttpListenerRequest request)
    {
        Request = request;
    }

	/// <summary>
	///     The underlying HttpListenerRequest
	/// </summary>
	public HttpListenerRequest Request { get; }

	/// <inheritdoc/>
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

	/// <inheritdoc/>
    public override string ToSafeString(List<BadObject> done)
    {
        return Request.ToString();
    }
}