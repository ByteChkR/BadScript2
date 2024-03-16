using System.Collections.Generic;
using System.Net;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

///<summary>
///	Contains Network Hosting Extensions and APIs for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.NetHost;

/// <summary>
///     Wrapper for the HttpListenerContext
/// </summary>
public class BadHttpContext : BadObject
{
	/// <summary>
	///     Class Prototype Instance
	/// </summary>
	private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadHttpContext>(
        "HttpContext",
        (_, _) => throw new BadRuntimeException("Cannot create new Http Contexts")
    );

	/// <summary>
	///     Constructs a new BadHttpContext
	/// </summary>
	/// <param name="context">The underlying HttpListenerContext</param>
	public BadHttpContext(HttpListenerContext context)
    {
        Context = context;
    }

	/// <summary>
	///     The underlying HttpListenerContext
	/// </summary>
	public HttpListenerContext Context { get; }

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return Context.ToString();
    }
}