using System.IO.Compression;

using BadScript2.Common.Logging;
using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common;
using BadScript2.Interop.Compression;
using BadScript2.Interop.Html;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Interop.Net;
using BadScript2.Interop.NetHost;
using BadScript2.IO;
using BadScript2.IO.Virtual;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Settings;

using Microsoft.AspNetCore.Components.CompilerServices;

using Newtonsoft.Json;
namespace BadScript2.Web.Frontend.Utils;

public static class BadRuntimeBuilder
{
    /// <summary>
    ///     Defines the Settings file
    /// </summary>
    private const string SETTINGS_FILE = "Settings.json";


    public static async Task<byte[]> GetWorkspace(string workspace, HttpClient client)
    {
        try
        {
            var result = await client.GetByteArrayAsync(workspace+".zip");
            return result;
        }
        catch (Exception e)
        {
            BadConsole.WriteLine(e.ToString());
            throw;
        }
    }
    public static async Task<IFileSystem> BuildFileSystem(string workspace, byte[] data)
    {
        BadConsole.WriteLine("Building File System...");
        BadVirtualFileSystem fs = new BadVirtualFileSystem();
        
        fs.CreateDirectory($"/.workspace/{workspace}", true);
        var meta = new
        {
            Workspace = workspace,
            BasedOn = "default"
        };
        fs.WriteAllText($"/.workspace/{workspace}/meta.json", JsonConvert.SerializeObject(meta, Formatting.Indented));

        var ms = new MemoryStream(data);
        fs.ImportZip(ms);
        
        return fs;
    }

    /// <summary>
    ///     Loads the Settings
    /// </summary>
    private static BadRuntime LoadConsoleSettings(this BadRuntime runtime, IFileSystem fs)
    {
        BadSettings consoleSettings = new BadSettings(string.Empty);
        string rootDir = fs.GetStartupDirectory();
        rootDir = rootDir.Remove(rootDir.Length - 1, 1);

        consoleSettings.SetProperty("RootDirectory", new BadSettings(rootDir, string.Empty));
        consoleSettings.SetProperty("DataDirectory", new BadSettings(Path.Combine(fs.GetStartupDirectory(), "data"), string.Empty));
        BadSettingsProvider.RootSettings.SetProperty("Console", consoleSettings);

        string settingsFile = Path.Combine(fs.GetStartupDirectory(), SETTINGS_FILE);
        if (!fs.Exists(settingsFile))
        {
            return runtime;
        }
        return runtime.LoadSettings(settingsFile, fs);
    }

    public static Task<BadRuntime> BuildRuntime(IFileSystem fs)
    {
        BadLogMask mask = BadLogMask.None;
        // Build the runtime
        BadConsole.WriteLine("Building Runtime...");
        BadRuntime runtime = new BadRuntime()
            .LoadConsoleSettings(fs)
            .UseLogMask(mask)
            .UseConsoleLogWriter()
            .UseCommonInterop()
            .UseCompressionApi()
            .UseFileSystemApi(fs)
            .UseHtmlApi()
            .UseJsonApi()
            .UseLinqApi()
            .UseNetApi()
            .UseNetHostApi()
            .UseLocalModules();

        return Task.FromResult(runtime);
    }
}