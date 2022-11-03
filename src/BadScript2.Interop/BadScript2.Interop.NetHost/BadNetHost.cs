using System;
using System.Collections.Generic;
using System.Net;

using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.NetHost;

public class BadNetHost
{
    private readonly HttpListener m_Listener = new HttpListener();

    public BadNetHost(string[] prefixes)
    {
        foreach (string prefix in prefixes)
        {
            m_Listener.Prefixes.Add(prefix);
        }
    }

    private static IEnumerator<BadObject> AcceptClient(HttpListener listener, Action<BadObject> callback)
    {
        bool accepted = false;
        listener.BeginGetContext(
            r =>
            {
                try {
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


    public BadTask AcceptClient()
    {
        BadInteropRunnable runnable = null;
        runnable = new BadInteropRunnable(AcceptClient(m_Listener, r => runnable.SetReturn(r)));

        return new BadTask(runnable, "AcceptClient");
    }

    public void Start()
    {
        m_Listener.Start();
    }

    public void Stop()
    {
        m_Listener.Stop();
    }

    public void Close()
    {
        m_Listener.Close();
    }

    public void Abort()
    {
        m_Listener.Abort();
    }
}