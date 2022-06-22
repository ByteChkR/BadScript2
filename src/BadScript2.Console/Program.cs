using BadScript2.Common.Logging;
using BadScript2.Common.Logging.Writer;
using BadScript2.Console.Debugging.Scriptable;
using BadScript2.Console.Systems.Run;
using BadScript2.Console.Systems.Settings;
using BadScript2.Console.Systems.Test;
using BadScript2.Interop.Common;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Settings;

namespace BadScript2.Console;

internal class BadProgram
{
    public const string SETTINGS_FILE = "Settings.json";

    private static void LoadSettings()
    {
        BadLogger.Log("Loading Settings...", "Settings");
        BadSettings consoleSettings = new BadSettings();
        string rootDir = AppDomain.CurrentDomain.BaseDirectory;
        rootDir = rootDir.Remove(rootDir.Length - 1, 1);

        consoleSettings.SetProperty("RootDirectory", new BadSettings(rootDir));
        consoleSettings.SetProperty("DataDirectory", new BadSettings(BadConsoleDirectories.DataDirectory));
        BadSettings root = new BadSettings();
        root.SetProperty("Console", consoleSettings);
        BadSettingsReader settingsReader = new BadSettingsReader(
            root,
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_FILE)
        );

        BadSettingsProvider.SetRootSettings(settingsReader.ReadSettings());
        BadLogger.Log("Settings loaded!", "Settings");
    }

    private static int Main(string[] args)
    {
        using BadConsoleLogWriter cWriter = new BadConsoleLogWriter();
        using BadFileLogWriter lWriter = new BadFileLogWriter(BadConsoleDirectories.LogFile);
        cWriter.Register();

        lWriter.Register();

        LoadSettings();
        BadNativeClassBuilder.AddNative(BadTask.Prototype);
        BadCommonInterop.AddExtensions();
        BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
        
        BadExecutionContextOptions.Default.Apis.AddRange(BadCommonInterop.Apis);
        BadExecutionContextOptions.Default.Apis.Add(new BadIOApi());
        BadExecutionContextOptions.Default.Apis.Add(new BadJsonApi());


        BadConsoleRunner runner = new BadConsoleRunner(
            new BadTestSystem(),
            new BadRunSystem(),
            new BadSettingsSystem()
        );

        return runner.Run(args);
    }
}