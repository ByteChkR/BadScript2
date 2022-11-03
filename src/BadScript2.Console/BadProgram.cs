﻿using System.Collections;
using System.Text;

using BadScript2.Common.Logging;
using BadScript2.Common.Logging.Writer;
using BadScript2.ConsoleAbstraction.Implementations.Remote.Client;
using BadScript2.ConsoleCore;
using BadScript2.ConsoleCore.Systems.Html;
using BadScript2.ConsoleCore.Systems.Run;
using BadScript2.ConsoleCore.Systems.Settings;
using BadScript2.ConsoleCore.Systems.Test;
using BadScript2.ConsoleCore.Systems.VirtualMachine;
using BadScript2.Debugger.Scriptable;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.Compression;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Interop.Net;
using BadScript2.Interop.NetHost;
using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Settings;
using BadScript2.Utility.Linq.Queries;

namespace BadScript2.Console
{
    internal static class BadProgram
    {
        private const string SETTINGS_FILE = "Settings.json";

        private static void LinqTest()
        {
            while (true)
            {
                System.Console.WriteLine("BLINQ: ");
                StringBuilder sb = new StringBuilder();
                string? current = null;
                while (current != "exit")
                {
                    if (current != null)
                    {
                        sb.AppendLine(current);
                    }

                    current = System.Console.ReadLine()!;
                }

                string statement = sb.ToString();
                if (statement == "")
                {
                    return;
                }

                try
                {
                    IEnumerable result = BadLinqQuery.Parse(statement, AppDomain.CurrentDomain.GetAssemblies());

                    object?[] arr = result.Cast<object?>().ToArray();

                    if (arr.All(x => x is BadObject))
                    {
                        List<BadObject> objs = arr.Cast<BadObject>().ToList();
                        System.Console.WriteLine(BadJson.ToJson(new BadArray(objs)));
                    }
                    else
                    {
                        foreach (object? item in arr)
                        {
                            System.Console.WriteLine(item);
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                }
            }
        }

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
            if (args.Contains("--logmask"))
            {
                int idx = Array.IndexOf(args, "--logmask");
                if (idx + 1 < args.Length)
                {
                    string mask = args[idx + 1];
                    BadLogWriterSettings.Instance.Mask = BadLogMask.GetMask(mask.Split(';').Select(x => (BadLogMask)x).ToArray());
                    args = args.Where((_, i) => i != idx && i != idx + 1).ToArray();
                }
            }

            if (args.Length == 3 && args[0] == "remote")
            {
                string host = args[1];
                int port = int.Parse(args[2]);
                BadNetworkConsoleClient client = new BadNetworkConsoleClient(host, port);
                client.Start();

                return -1;
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
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
            BadInteropExtension.AddExtension<BadNetInteropExtensions>();
            BadInteropExtension.AddExtension<BadLinqExtensions>();
            BadInteropExtension.AddExtension<BadNetHostExtensions>();

            BadExecutionContextOptions.Default.Apis.AddRange(BadCommonInterop.Apis);
            BadExecutionContextOptions.Default.Apis.Add(new BadIOApi());
            BadExecutionContextOptions.Default.Apis.Add(new BadJsonApi());
            BadExecutionContextOptions.Default.Apis.Add(new BadNetApi());
            BadExecutionContextOptions.Default.Apis.Add(new BadCompressionApi());
            BadExecutionContextOptions.Default.Apis.Add(new BadNetHostApi());


            BadConsoleRunner runner = new BadConsoleRunner(
                new BadDefaultRunSystem(),
                new BadTestSystem(),
                new BadRunSystem(),
                new BadSettingsSystem(),
                new BadHtmlSystem(),
                new BadVirtualMachineRunSystem(),
                new BadVirtualMachineNewSystem(),
                new BadVirtualMachineManagerSystem(),
                new BadVirtualMachineManagerClientSystem()
            );


            //LinqTest();


            int r = runner.Run(args);
            lWriter?.Dispose();


            return r;
        }
    }
}