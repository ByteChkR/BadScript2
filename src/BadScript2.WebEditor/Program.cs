using BadScript2.Common.Logging;
using BadScript2.ConsoleCore;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Net;
using BadScript2.IO;
using BadScript2.IO.Virtual;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Settings;
using BadScript2.WebEditor;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
BadLogger.OnLog += l => Console.WriteLine(l);
HttpClient client = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

async Task LoadZip()
{
    Console.WriteLine("Loading Zip");

    byte[] zipData = await client.GetByteArrayAsync("images/bootstrap/RootFS.zip");
    Console.WriteLine($"Data Size: {zipData.Length} Bytes");
    MemoryStream ms = new MemoryStream(zipData, false);
    ms.Position = 0;
    Console.WriteLine("Importing Zip");
    BadFileSystem.Instance.ImportZip(ms);
}


BadFileSystem.SetFileSystem(new BadVirtualFileSystem());

await LoadZip();

const string settingsFile = "Settings.json";

// using BadConsoleLogWriter cWriter = new BadConsoleLogWriter();
// cWriter.Register();


BadSettingsProvider.SetRootSettings(new BadSettings());

static void LoadSettings()
{
    BadLogger.Log("Loading Settings...", "Settings");
    BadSettings consoleSettings = new BadSettings();
    string rootDir = BadFileSystem.Instance.GetStartupDirectory();
    rootDir = rootDir.Remove(rootDir.Length - 1, 1);

    consoleSettings.SetProperty("RootDirectory", new BadSettings(rootDir));
    consoleSettings.SetProperty("DataDirectory", new BadSettings(BadConsoleDirectories.DataDirectory));
    BadSettings root = new BadSettings();
    root.SetProperty("Console", consoleSettings);
    BadSettingsReader settingsReader = new BadSettingsReader(
        root,
        Path.Combine(BadFileSystem.Instance.GetStartupDirectory(), settingsFile)
    );

    BadSettingsProvider.SetRootSettings(settingsReader.ReadSettings());
    BadLogger.Log("Settings loaded!", "Settings");
}

LoadSettings();
BadNativeClassBuilder.AddNative(BadTask.Prototype);
BadCommonInterop.AddExtensions();

//BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
BadInteropExtension.AddExtension<BadNetInteropExtensions>();

BadExecutionContextOptions.Default.Apis.AddRange(BadCommonInterop.Apis);
BadExecutionContextOptions.Default.Apis.Add(new BadIOApi());
BadExecutionContextOptions.Default.Apis.Add(new BadJsonApi());
BadExecutionContextOptions.Default.Apis.Add(new BadNetApi());

builder.Services
    .AddBlazorise(options => { options.Immediate = true; })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();