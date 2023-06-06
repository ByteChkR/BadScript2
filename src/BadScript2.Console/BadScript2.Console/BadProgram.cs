using BadScript2.Common.Logging;
using BadScript2.Common.Logging.Writer;
using BadScript2.ConsoleCore;
using BadScript2.ConsoleCore.Systems.Html;
using BadScript2.ConsoleCore.Systems.Run;
using BadScript2.ConsoleCore.Systems.Settings;
using BadScript2.ConsoleCore.Systems.Shell;
using BadScript2.ConsoleCore.Systems.Test;
using BadScript2.Debugger.Scriptable;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.Common.Versioning;
using BadScript2.Interop.Compression;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Interop.Net;
using BadScript2.Interop.NetHost;
using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

namespace BadScript2.Console;

internal static class BadProgram
{
	private const string SETTINGS_FILE = "Settings.json";


	private static void LoadSettings()
	{
		BadLogger.Log("Loading Settings...", "Settings");
		BadSettings consoleSettings = new BadSettings();
		string rootDir = BadFileSystem.Instance.GetStartupDirectory();
		rootDir = rootDir.Remove(rootDir.Length - 1, 1);

		consoleSettings.SetProperty("RootDirectory", new BadSettings(rootDir));
		consoleSettings.SetProperty("DataDirectory", new BadSettings(BadConsoleDirectories.DataDirectory));
		BadSettings root = new BadSettings();
		root.SetProperty("Console", consoleSettings);
		BadSettingsReader settingsReader = new BadSettingsReader(root,
			Path.Combine(BadFileSystem.Instance.GetStartupDirectory(), SETTINGS_FILE));

		BadSettingsProvider.SetRootSettings(settingsReader.ReadSettings());
		BadLogger.Log("Settings loaded!", "Settings");
	}

	private static int Main(string[] args)
	{
		if (args.Contains("--logmask"))
		{
			int idx = Array.IndexOf(args, "--logmask");

			if (idx + 1 < args.Length)
			{
				string mask = args[idx + 1];
				BadLogWriterSettings.Instance.Mask =
					BadLogMask.GetMask(mask.Split(';').Select(x => (BadLogMask)x).ToArray());
				args = args.Where((_, i) => i != idx && i != idx + 1).ToArray();
			}
		}

		using BadConsoleLogWriter cWriter = new BadConsoleLogWriter();
		cWriter.Register();

		BadFileLogWriter? lWriter = null;

		try
		{
			lWriter = new BadFileLogWriter(BadConsoleDirectories.LogFile);
			lWriter.Register();
		}
		catch (Exception e)
		{
			BadLogger.Error("Can not attach log file writer. " + e.Message, "BadConsole");
		}


		LoadSettings();
		BadNativeClassBuilder.AddNative(BadTask.Prototype);
		BadNativeClassBuilder.AddNative(BadVersion.Prototype);
		BadCommonInterop.AddExtensions();
		BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
		BadInteropExtension.AddExtension<BadNetInteropExtensions>();
		BadInteropExtension.AddExtension<BadLinqExtensions>();
		BadInteropExtension.AddExtension<BadNetHostExtensions>();

		BadExecutionContextOptions.Default.AddApis(BadCommonInterop.Apis);
		BadExecutionContextOptions.Default.AddApi(new BadIOApi());
		BadExecutionContextOptions.Default.AddApi(new BadJsonApi());
		BadExecutionContextOptions.Default.AddApi(new BadNetApi());
		BadExecutionContextOptions.Default.AddApi(new BadCompressionApi());
		BadExecutionContextOptions.Default.AddApi(new BadNetHostApi());
		BadExecutionContextOptions.Default.AddApi(new BadCompilerApi());


		BadConsoleRunner runner = new BadConsoleRunner(new BadDefaultRunSystem(),
			new BadTestSystem(),
			new BadRunSystem(),
			new BadSettingsSystem(),
			new BadHtmlSystem(),
			new BadRemoteConsoleSystem(),
			new BadShellSystem());

		//CompileTest();

		int r = runner.Run(args);
		lWriter?.Dispose();


		return r;
	}
}
