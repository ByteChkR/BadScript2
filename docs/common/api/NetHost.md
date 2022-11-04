# NetHost API Reference

The `NetHost` api has the following Properties to Host a HTTP Server.

```
{
        Create: function Create(BadArray)
}
```

## The HTTP Listener object

```js
{
        Start: function Start() //Starts the Listener
        Stop: function Stop()
        Close: function Close()
        Abort: function Abort()
        AcceptClient: function AcceptClient() //Accept Clients asynchonously
}
```


## The HTTP Context Object returned by `Listener.AcceptClient()`

```js
{
        Request: {
                ContentLength,
                ContentType,
                Headers,
                HttpMethod,
                IsLocal,
                IsSecureConnection,
                IsAuthenticated,
                IsWebSocketRequest,
                KeepAlive,
                QueryString,
                RawUrl,
                Url,
                UrlReferrer,
                ServiceName,
                UserAgent,
                ClientCertificateError,
                HasEntityBody,
                UserHostAddress,
                UserHostName,
                UserLanguages,
                AcceptTypes,
                ContentEncoding,
                Cookies,
                Content: {
                        AsString: function AsString(),
                        AsBytes: function AsBytes()
                }
        },
        Response: {
                SetHeader: function SetHeader(name, value),
                SetHeaders: function SetHeaders(BadTable),
                SetCookie: function SetCookie(cookie),
                SetCookies: function SetCookies(BadArray),
                SetStatusCode: function SetStatusCode(code),
                SetStatusDescription: function SetStatusDescription(descr),
                SetContentType: function SetContentType(contentType),
                SetContentEncoding: function SetContentEncoding(encodingName),
                SetContentLength: function SetContentLength(len),
                SetKeepAlive: function SetKeepAlive(keepAlive),
                Redirect: function Redirect(redirectUrl),
                Abort: function Abort(),
                Close: function Close(),
                SetContent: function SetContent(contentText)
        }
}
```


___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)