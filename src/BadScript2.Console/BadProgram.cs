using BadScript2.Common.Logging;
using BadScript2.Common.Logging.Writer;
using BadScript2.Console.Debugging.Scriptable;
using BadScript2.Console.Systems.Run;
using BadScript2.Console.Systems.Settings;
using BadScript2.Console.Systems.Test;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Net;
using BadScript2.IO;
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
        string rootDir = BadFileSystem.Instance.GetStartupDirectory();
        rootDir = rootDir.Remove(rootDir.Length - 1, 1);

        consoleSettings.SetProperty("RootDirectory", new BadSettings(rootDir));
        consoleSettings.SetProperty("DataDirectory", new BadSettings(BadConsoleDirectories.DataDirectory));
        BadSettings root = new BadSettings();
        root.SetProperty("Console", consoleSettings);
        BadSettingsReader settingsReader = new BadSettingsReader(
            root,
            Path.Combine(BadFileSystem.Instance.GetStartupDirectory(), SETTINGS_FILE)
        );

        BadSettingsProvider.SetRootSettings(settingsReader.ReadSettings());
        BadLogger.Log("Settings loaded!", "Settings");
    }

    private static int Main(string[] args)
    {
        //BadFileSystem.SetFileSystem(new BadVirtualFileSystem());


        using BadConsoleLogWriter cWriter = new BadConsoleLogWriter();
        cWriter.Register();
        BadFileLogWriter lWriter = new BadFileLogWriter(BadConsoleDirectories.LogFile);
        lWriter.Register();

        // using (Stream s = File.OpenRead("C:\\Users\\Tim\\Documents\\LanguageProjects\\BadScript2\\FS.zip"))
        // {
        //     Action<BadLog> onLog = l => System.Console.WriteLine(l.ToString());
        //     BadLogger.OnLog += onLog;
        //     BadFileSystem.Instance.ImportZip(s);
        //     BadLogger.OnLog -= onLog;
        //     BadFileSystem.Instance.SetCurrentDirectory("projects/System");
        // }

        LoadSettings();
        BadNativeClassBuilder.AddNative(BadTask.Prototype);
        BadCommonInterop.AddExtensions();
        BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
        BadInteropExtension.AddExtension<BadNetInteropExtensions>();

        BadExecutionContextOptions.Default.Apis.AddRange(BadCommonInterop.Apis);
        BadExecutionContextOptions.Default.Apis.Add(new BadIOApi());
        BadExecutionContextOptions.Default.Apis.Add(new BadJsonApi());
        BadExecutionContextOptions.Default.Apis.Add(new BadNetApi());


        BadConsoleRunner runner = new BadConsoleRunner(
            new BadDefaultRunSystem(),
            new BadTestSystem(),
            new BadRunSystem(),
            new BadSettingsSystem()
        );

        int r = runner.Run(args);
        lWriter.Dispose();

        // using (MemoryStream s = new MemoryStream())
        // {
        //     BadFileSystem.Instance.ExportZip(s);
        //     s.Position = 0;
        //     File.WriteAllBytes("C:\\Users\\Tim\\Documents\\LanguageProjects\\BadScript2\\FS_out.zip",s.ToArray());
        // }

        return r;
    }
}