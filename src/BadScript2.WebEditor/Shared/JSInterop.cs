using Microsoft.JSInterop;

namespace BadScript2.WebEditor.Shared;

public static class JSInterop
{

    public static async void ClickElement(this IJSRuntime runtime, string name)
    {
        await runtime.InvokeVoidAsync("ClickElement", name);
    }
    public static async void DownloadFile(this IJSRuntime runtime, string name, byte[] data)
    {
        string contentType = "application/octet-stream";

        // Check if the IJSRuntime is the WebAssembly implementation of the JSRuntime
        if (runtime is IJSUnmarshalledRuntime webAssemblyJSRuntime)
        {
            webAssemblyJSRuntime.InvokeUnmarshalled<string, string, byte[], bool>("BlazorDownloadFileFast", name, contentType, data);
        }
        else
        {
            // Fall back to the slow method if not in WebAssembly
            await runtime.InvokeVoidAsync("BlazorDownloadFile", name, contentType, data);
        }
    }
}