using System;
using System.Collections.Generic;
using System.Net;

using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.NetHost;

public static class BadNetApiExtensions
{
	public static BadRuntime UseNetHostApi(this BadRuntime runtime)
	{
		BadInteropExtension.AddExtension<BadNetHostExtensions>();
		runtime.ConfigureContextOptions(opts=>opts.AddOrReplaceApi(new BadNetHostApi()));

		return runtime;
	}
}
/// <summary>
///     Implements a BadScript HttpListener Host
/// </summary>
public class BadNetHost
{
	/// <summary>
	///     The Listener that is to be used.
	/// </summary>
	private readonly HttpListener m_Listener = new HttpListener();

	/// <summary>
	///     Constructs a new BadNetHost with the given prefixes
	/// </summary>
	/// <param name="prefixes">Prefixes</param>
	public BadNetHost(string[] prefixes)
    {
        foreach (string prefix in prefixes)
        {
            m_Listener.Prefixes.Add(prefix);
        }
    }

	/// <summary>
	///     Accepts a Client
	/// </summary>
	/// <param name="listener">The Http Listener</param>
	/// <param name="callback">The Callback</param>
	/// <returns>Enumeration</returns>
	private static IEnumerator<BadObject> AcceptClient(HttpListener listener, Action<BadObject> callback)
    {
        bool accepted = false;
        listener.BeginGetContext(
            r =>
            {
                try
                {
                    HttpListenerContext ctx = listener.EndGetContext(r);
                    callback(new BadHttpContext(ctx));
                }
                catch (Exception)
                {
                    //Do Nothing
                }

                accepted = true;
            },
            null
        );

        while (!accepted)
        {
            yield return BadObject.Null;
        }
    }


	/// <summary>
	///     Accepts a Client
	/// </summary>
	/// <returns>Awaitable task</returns>
	public BadTask AcceptClient()
    {
        BadInteropRunnable runnable = null;
        runnable = new BadInteropRunnable(AcceptClient(m_Listener, r => runnable.SetReturn(r)));

        return new BadTask(runnable, "AcceptClient");
    }

	/// <summary>
	///     Starts the Listener
	/// </summary>
	public void Start()
    {
        m_Listener.Start();
    }

	/// <summary>
	///     Stops the Listener
	/// </summary>
	public void Stop()
    {
        m_Listener.Stop();
    }

	/// <summary>
	///     Close the Listener
	/// </summary>
	public void Close()
    {
        m_Listener.Close();
    }

	/// <summary>
	///     Aborts the Listener
	/// </summary>
	public void Abort()
    {
        m_Listener.Abort();
    }
}