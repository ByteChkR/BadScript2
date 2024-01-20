using BadScript2.Common.Logging;
using BadScript2.ConsoleCore;
using BadScript2.ConsoleCore.Systems.Html;
using BadScript2.ConsoleCore.Systems.Run;
using BadScript2.ConsoleCore.Systems.Settings;
using BadScript2.ConsoleCore.Systems.Test;
using BadScript2.Interop.Common;
using BadScript2.Interop.Compression;
using BadScript2.Interop.Html;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Interop.Net;
using BadScript2.Interop.NetHost;
using BadScript2.IO;
using BadScript2.Settings;

/// <summary>
///	 Contains the Entrypoint for the Console Application
/// </summary>
namespace BadScript2.Console;

/// <summary>
///     Entrypoint for the Console Application
/// </summary>
internal static class BadProgram
{
	/// <summary>
	///     Defines the Settings file
	/// </summary>
	private const string SETTINGS_FILE = "Settings.json";


	/// <summary>
	///     Loads the Settings
	/// </summary>
	private static BadRuntime LoadConsoleSettings(this BadRuntime runtime)
    {
        BadSettings consoleSettings = new BadSettings();
        string rootDir = BadFileSystem.Instance.GetStartupDirectory();
        rootDir = rootDir.Remove(rootDir.Length - 1, 1);

        consoleSettings.SetProperty("RootDirectory", new BadSettings(rootDir));
        consoleSettings.SetProperty("DataDirectory", new BadSettings(BadConsoleDirectories.DataDirectory));
        BadSettingsProvider.RootSettings.SetProperty("Console", consoleSettings);

        return runtime.LoadSettings(Path.Combine(BadFileSystem.Instance.GetStartupDirectory(), SETTINGS_FILE));
    }


	/// <summary>
	///     Entrypoint
	/// </summary>
	/// <param name="args">Commandline Arguments</param>
	/// <returns>Return Code</returns>
	private static int Main(string[] args)
    {
        BadLogMask mask = BadLogMask.None;

        if (args.Contains("--logmask"))
        {
            int idx = Array.IndexOf(args, "--logmask");

            if (idx + 1 < args.Length)
            {
                string maskStr = args[idx + 1];
                mask = BadLogMask.GetMask(maskStr.Split(';').Select(x => (BadLogMask)x).ToArray());
                args = args.Where((_, i) => i != idx && i != idx + 1).ToArray();
            }
        }

        using BadRuntime runtime = new BadRuntime()
            .UseLogMask(mask)
            .UseConsoleLogWriter()
            .LoadConsoleSettings()
            .UseCommonInterop()
            .UseCompressionApi()
            .UseFileSystemApi()
            .UseHtmlApi()
            .UseJsonApi()
            .UseLinqApi()
            .UseNetApi()
            .UseNetHostApi();


        BadConsoleRunner runner = new BadConsoleRunner(
            new BadDefaultRunSystem(runtime),
            new BadTestSystem(runtime),
            new BadRunSystem(runtime),
            new BadSettingsSystem(runtime),
            new BadHtmlSystem(runtime),
            new BadRemoteConsoleSystem(runtime)
        );


        int r = runner.Run(args);

        return r;
    }
}