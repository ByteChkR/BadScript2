﻿using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Net;

/// <summary>
///     Implements Extensions for the "Net" Api
/// </summary>
public class BadNetInteropExtensions : BadInteropExtension
{
	protected override void AddExtensions(BadInteropExtensionProvider provider)
	{
		provider.RegisterObject<HttpResponseMessage>("Status", resp => (decimal)resp.StatusCode);
		provider.RegisterObject<HttpResponseMessage>("Reason", resp => resp.ReasonPhrase ?? "");
		provider.RegisterObject<HttpResponseMessage>("Headers",
			resp =>
			{
				Dictionary<BadObject, BadObject> v = resp.Headers.ToDictionary(x => (BadObject)x.Key,
					x => (BadObject)new BadArray(x.Value.Select(y => (BadObject)y).ToList()));

				return new BadTable(v);
			});
		provider.RegisterObject<HttpResponseMessage>("Content", resp => BadObject.Wrap(resp.Content));

		provider.RegisterObject<HttpContent>("ReadAsString",
			c => new BadDynamicInteropFunction("ReadAsString",
				_ => Content_ReadAsString(c)));
		provider.RegisterObject<HttpContent>("ReadAsArray",
			c => new BadDynamicInteropFunction("ReadAsArray",
				_ => Content_ReadAsArray(c)));
	}

    /// <summary>
    ///     Reads the content as a string
    /// </summary>
    /// <param name="content">The Http Content</param>
    /// <returns>Awaitable Task with result string</returns>
    private BadTask Content_ReadAsString(HttpContent content)
	{
		Task<string> task = content.ReadAsStringAsync();

		return new BadTask(BadTaskUtils.WaitForTask(task), "HttpContent.ReadAsString");
	}

    /// <summary>
    ///     Reads the content as array
    /// </summary>
    /// <param name="content">The Http Content</param>
    /// <returns>Awaitable Task with result array</returns>
    private BadTask Content_ReadAsArray(HttpContent content)
	{
		Task<byte[]> task = content.ReadAsByteArrayAsync();

		return new BadTask(BadTaskUtils.WaitForTask(task,
				o => new BadArray(o.Select(x => (BadObject)(decimal)x).ToList())),
			"HttpContent.ReadAsArray");
	}
}
