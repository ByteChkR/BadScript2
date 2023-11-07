using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using BadScript2.ConsoleAbstraction.Implementations.Remote;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.NetHost;

/// <summary>
///     Implements Extensions for the "NetHost" Api
/// </summary>
public class BadNetHostExtensions : BadInteropExtension
{
	/// <summary>
	///     Creates a table from a name value collection
	/// </summary>
	/// <param name="headers">The Collection</param>
	/// <returns>Table</returns>
	private static BadTable CreateNameValueTable(NameValueCollection headers)
    {
        BadTable t = new BadTable();

        foreach (string key in headers.AllKeys)
        {
            t.SetProperty(key, headers[key]);
        }

        return t;
    }

	/// <summary>
	///     Creates a Cookie Table from the Cookie Collection
	/// </summary>
	/// <param name="cookies">The Collection</param>
	/// <returns>A Cookie Table</returns>
	private static BadArray CreateCookieTable(CookieCollection cookies)
    {
        BadArray table = new BadArray();

        foreach (Cookie cookie in cookies)
        {
            table.InnerArray.Add(new BadReflectedObject(cookie));
        }

        return table;
    }

	/// <summary>
	///     Copies the Headers from the BadScript Table to the Http Header Table
	/// </summary>
	/// <param name="dst">The Destination Table</param>
	/// <param name="src">The Source Table</param>
	/// <exception cref="BadRuntimeException">Gets raised if the Source Table contains invalid object keys</exception>
	private static void CopyHeaderTable(NameValueCollection dst, BadTable src)
    {
        foreach (KeyValuePair<BadObject, BadObject> kvp in src.InnerTable)
        {
            if (kvp.Key is not IBadString k || kvp.Value is not IBadString v)
            {
                throw new BadRuntimeException("Header Table must only contain string keys and values");
            }

            dst[k.Value] = v.Value;
        }
    }

	/// <summary>
	///     Creates an Http Content Table
	/// </summary>
	/// <param name="content">The Content Stream</param>
	/// <param name="enc">The Encoding</param>
	/// <returns>Http Response Table</returns>
	private BadTable CreateContentTable(Stream content, Encoding enc)
    {
        BadTable table = new BadTable();
        table.SetFunction(
            "AsString",
            () =>
            {
                StreamReader sr = new StreamReader(content, enc);

                return sr.ReadToEnd();
            }
        );
        table.SetFunction(
            "AsBytes",
            () =>
            {
                byte[] data = new byte[content.Length];
                int read = content.Read(data, 0, data.Length);

                if (read != data.Length)
                {
                    throw new BadNetworkConsoleException("Could not read all data from stream");
                }

                return new BadArray(data.Select(x => (BadObject)x).ToList());
            }
        );

        return table;
    }

	/// <summary>
	///     Adds Extensions for the BadHttpContext Object
	/// </summary>
	private void AddContextExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadHttpContext>("Request", c => new BadHttpRequest(c.Context.Request));
        provider.RegisterObject<BadHttpContext>("Response", c => CreateResponseTable(c.Context.Response));
    }

	/// <summary>
	///     Adds Extensions for the BadHttpRequest Object
	/// </summary>
	private void AddRequestExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadHttpRequest>("ContentLength", r => r.Request.ContentLength64);
        provider.RegisterObject<BadHttpRequest>("ContentType", r => r.Request.ContentType);
        provider.RegisterObject<BadHttpRequest>("Headers", r => CreateNameValueTable(r.Request.Headers));
        provider.RegisterObject<BadHttpRequest>("HttpMethod", r => r.Request.HttpMethod);
        provider.RegisterObject<BadHttpRequest>("IsLocal", r => r.Request.IsLocal);
        provider.RegisterObject<BadHttpRequest>("IsSecureConnection", r => r.Request.IsSecureConnection);
        provider.RegisterObject<BadHttpRequest>("IsAuthenticated", r => r.Request.IsAuthenticated);
        provider.RegisterObject<BadHttpRequest>("IsWebSocketRequest", r => r.Request.IsWebSocketRequest);
        provider.RegisterObject<BadHttpRequest>("KeepAlive", r => r.Request.KeepAlive);
        provider.RegisterObject<BadHttpRequest>("QueryString", r => CreateNameValueTable(r.Request.QueryString));
        provider.RegisterObject<BadHttpRequest>("RawUrl", r => r.Request.RawUrl);
        provider.RegisterObject<BadHttpRequest>("Url", r => new BadReflectedObject(r.Request.Url));
        provider.RegisterObject<BadHttpRequest>("UrlReferrer", r => new BadReflectedObject(r.Request.UrlReferrer));
        provider.RegisterObject<BadHttpRequest>("ServiceName", r => r.Request.ServiceName);
        provider.RegisterObject<BadHttpRequest>("UserAgent", r => r.Request.UserAgent);
        provider.RegisterObject<BadHttpRequest>("ClientCertificateError", r => r.Request.ClientCertificateError);
        provider.RegisterObject<BadHttpRequest>("HasEntityBody", r => r.Request.HasEntityBody);
        provider.RegisterObject<BadHttpRequest>("UserHostAddress", r => r.Request.UserHostAddress);
        provider.RegisterObject<BadHttpRequest>("UserHostName", r => r.Request.UserHostName);
        provider.RegisterObject<BadHttpRequest>(
            "UserLanguages",
            r => new BadArray((r.Request.UserLanguages ?? Array.Empty<string>()).Select(x => (BadObject)x).ToList())
        );
        provider.RegisterObject<BadHttpRequest>(
            "AcceptTypes",
            r => new BadArray((r.Request.AcceptTypes ?? Array.Empty<string>()).Select(x => (BadObject)x).ToList())
        );
        provider.RegisterObject<BadHttpRequest>("ContentEncoding", r => r.Request.ContentEncoding.EncodingName);
        provider.RegisterObject<BadHttpRequest>("Cookies", r => CreateCookieTable(r.Request.Cookies));
        provider.RegisterObject<BadHttpRequest>(
            "Content",
            r => CreateContentTable(r.Request.InputStream, r.Request.ContentEncoding)
        );
    }

	/// <summary>
	///     Creates a Response Table for the given Response Object
	/// </summary>
	/// <param name="resp">Resonse Object</param>
	/// <returns>Response Table</returns>
	private BadTable CreateResponseTable(HttpListenerResponse resp)
    {
        BadTable table = new BadTable();
        table.SetFunction("SetHeader", (string key, string value) => resp.Headers[key] = value);
        table.SetFunction("SetHeaders", (BadTable headers) => CopyHeaderTable(resp.Headers, headers));
        table.SetFunction("SetCookie", (Cookie cookie) => resp.Cookies.Add(cookie));
        table.SetFunction(
            "SetCookies",
            (BadArray cookies) =>
            {
                foreach (BadObject cookie in cookies.InnerArray)
                {
                    if (cookie is BadReflectedObject cro && cro.Instance is Cookie c)
                    {
                        resp.Cookies.Add(c);
                    }
                    else
                    {
                        throw new BadRuntimeException("Cookies must be Cookie objects");
                    }
                }
            }
        );
        table.SetFunction("SetStatusCode", (int code) => resp.StatusCode = code);
        table.SetFunction("SetStatusDescription", (string desc) => resp.StatusDescription = desc);
        table.SetFunction("SetContentType", (string type) => resp.ContentType = type);
        table.SetFunction("SetContentEncoding", (string enc) => resp.ContentEncoding = Encoding.GetEncoding(enc));
        table.SetFunction("SetContentLength", (long len) => resp.ContentLength64 = len);
        table.SetFunction("SetKeepAlive", (bool keepAlive) => resp.KeepAlive = keepAlive);
        table.SetFunction("Redirect", (string location) => resp.Redirect(location));
        table.SetFunction("Abort", resp.Abort);
        table.SetFunction("Close", resp.Close);
        table.SetFunction(
            "SetContent",
            (string content) =>
            {
                byte[] data = (resp.ContentEncoding ?? Encoding.UTF8).GetBytes(content);
                resp.OutputStream.Write(data, 0, data.Length);
            }
        );

        return table;
    }

    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        AddContextExtensions(provider);
        AddRequestExtensions(provider);
    }
}