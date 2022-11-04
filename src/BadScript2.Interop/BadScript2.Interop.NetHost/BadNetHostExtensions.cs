using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.NetHost;

public class BadNetHostExtensions : BadInteropExtension
{
    private static BadTable CreateNameValueTable(NameValueCollection headers)
    {
        BadTable t = new BadTable();
        foreach (string key in headers.AllKeys)
        {
            t.SetProperty(key, headers[key]);
        }

        return t;
    }

    private static BadArray CreateCookieTable(CookieCollection cookies)
    {
        BadArray table = new BadArray();
        foreach (Cookie cookie in cookies)
        {
            table.InnerArray.Add(new BadReflectedObject(cookie));
        }

        return table;
    }

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

    private BadTable CreateInputTable(Stream content, Encoding enc)
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
                content.Read(data, 0, data.Length);

                return new BadArray(data.Select(x => (BadObject)x).ToList());
            }
        );

        return table;
    }

    private void AddContextExtensions()
    {
        RegisterObject<BadHttpContext>("Request", c => new BadHttpRequest(c.Context.Request));
        RegisterObject<BadHttpContext>("Response", c => CreateResponseTable(c.Context.Response));
    }

    private void AddRequestExtensions()
    {
        RegisterObject<BadHttpRequest>("ContentLength", r => r.Request.ContentLength64);
        RegisterObject<BadHttpRequest>("ContentType", r => r.Request.ContentType);
        RegisterObject<BadHttpRequest>("Headers", r => CreateNameValueTable(r.Request.Headers));
        RegisterObject<BadHttpRequest>("HttpMethod", r => r.Request.HttpMethod);
        RegisterObject<BadHttpRequest>("IsLocal", r => r.Request.IsLocal);
        RegisterObject<BadHttpRequest>("IsSecureConnection", r => r.Request.IsSecureConnection);
        RegisterObject<BadHttpRequest>("IsAuthenticated", r => r.Request.IsAuthenticated);
        RegisterObject<BadHttpRequest>("IsWebSocketRequest", r => r.Request.IsWebSocketRequest);
        RegisterObject<BadHttpRequest>("KeepAlive", r => r.Request.KeepAlive);
        RegisterObject<BadHttpRequest>("QueryString", r => CreateNameValueTable(r.Request.QueryString));
        RegisterObject<BadHttpRequest>("RawUrl", r => r.Request.RawUrl);
        RegisterObject<BadHttpRequest>("Url", r => new BadReflectedObject(r.Request.Url));
        RegisterObject<BadHttpRequest>("UrlReferrer", r => new BadReflectedObject(r.Request.UrlReferrer));
        RegisterObject<BadHttpRequest>("ServiceName", r => r.Request.ServiceName);
        RegisterObject<BadHttpRequest>("UserAgent", r => r.Request.UserAgent);
        RegisterObject<BadHttpRequest>("ClientCertificateError", r => r.Request.ClientCertificateError);
        RegisterObject<BadHttpRequest>("HasEntityBody", r => r.Request.HasEntityBody);
        RegisterObject<BadHttpRequest>("UserHostAddress", r => r.Request.UserHostAddress);
        RegisterObject<BadHttpRequest>("UserHostName", r => r.Request.UserHostName);
        RegisterObject<BadHttpRequest>("UserLanguages", r => new BadArray((r.Request.UserLanguages ?? Array.Empty<string>()).Select(x => (BadObject)x).ToList()));
        RegisterObject<BadHttpRequest>("AcceptTypes", r => new BadArray((r.Request.AcceptTypes ?? Array.Empty<string>()).Select(x => (BadObject)x).ToList()));
        RegisterObject<BadHttpRequest>("ContentEncoding", r => r.Request.ContentEncoding.EncodingName);
        RegisterObject<BadHttpRequest>("Cookies", r => CreateCookieTable(r.Request.Cookies));
        RegisterObject<BadHttpRequest>("Content", r => CreateInputTable(r.Request.InputStream, r.Request.ContentEncoding));
    }

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
                byte[] data = (resp.ContentEncoding?? Encoding.UTF8).GetBytes(content);
                resp.OutputStream.Write(data, 0, data.Length);
            }
        );

        return table;
    }

    protected override void AddExtensions()
    {
        AddContextExtensions();
        AddRequestExtensions();
    }
}