using BadScript2.Common.Logging;
using BadScript2.ConsoleAbstraction.Implementations;
using BadScript2.ConsoleCore;
using BadScript2.ConsoleCore.Systems.Docs;
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
	private static void LoadConsoleSettings(IFileSystem fileSystem)
    {
	    if (!BadSettingsProvider.HasRootSettings)
	    {
		    BadSettingsProvider.SetRootSettings(new BadSettings(string.Empty));
	    }
        BadSettings consoleSettings = new BadSettings(string.Empty);
        string rootDir = fileSystem.GetStartupDirectory();
        rootDir = rootDir.Remove(rootDir.Length - 1, 1);

        consoleSettings.SetProperty("RootDirectory", new BadSettings(rootDir, string.Empty));
        
        BadSettingsProvider.RootSettings.SetProperty("DataDirectory",
	        new BadSettings(BadConsoleDirectories.GetDataDirectory(fileSystem), string.Empty)
        );
        BadSettingsProvider.RootSettings.SetProperty("Console", consoleSettings);

        BadSettingsProvider.LoadSettings(Path.Combine(fileSystem.GetStartupDirectory(), SETTINGS_FILE), fileSystem);
    }


	/// <summary>
	///     Entrypoint
	/// </summary>
	/// <param name="args">Commandline Arguments</param>
	/// <returns>Return Code</returns>
	private static Task<int> Main(string[] args)
    {
        BadLogMask mask = BadLogMask.None;

        if (args.Contains("--logmask"))
        {
            int idx = Array.IndexOf(args, "--logmask");

            if (idx + 1 < args.Length)
            {
                string maskStr = args[idx + 1];

                mask = BadLogMask.GetMask(maskStr.Split(';')
                                                 .Select(x => (BadLogMask)x)
                                                 .ToArray()
                                         );

                args = args.Where((_, i) => i != idx && i != idx + 1)
                           .ToArray();
            }
        }

        var fs = new BadSystemFileSystem();
        LoadConsoleSettings(fs);

        Func<BadRuntime> factory = () =>
        {
	        return new BadRuntime()
		        .UseLogMask(mask)
		        .UseConsoleLogWriter()
		        .UseCommonInterop()
		        .UseCompressionApi(fs)
		        .UseFileSystemApi(fs)
		        .UseHtmlApi()
		        .UseJsonApi()
		        .UseLinqApi()
		        .UseNetApi()
		        .UseNetHostApi()
		        .UseLocalModules(fs);
        };

        BadConsoleRunner runner = new BadConsoleRunner(new BadDefaultRunSystem(factory),
                                                       new BadTestSystem(factory),
                                                       new BadRunSystem(factory),
                                                       new BadSettingsSystem(factory),
                                                       new BadHtmlSystem(factory),
                                                       new BadRemoteConsoleSystem(factory),
                                                       new BadDocsSystem(factory)
                                                      );

        return runner.Run(args);
    }
}