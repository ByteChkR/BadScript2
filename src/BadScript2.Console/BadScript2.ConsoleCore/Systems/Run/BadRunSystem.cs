using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using BadScript2.Common.Logging;
using BadScript2.ConsoleAbstraction.Implementations.Remote;
using BadScript2.Debugger;
using BadScript2.Interactive;
using BadScript2.Interop.Common;
using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Settings;
using BadScript2.Settings;

namespace BadScript2.ConsoleCore.Systems.Run;

/// <summary>
///     Runs one or more BadScript scripts
/// </summary>
public class BadRunSystem : BadConsoleSystem<BadRunSystemSettings>
{
    /// <summary>
    ///     Creates a new BadRunSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    public BadRunSystem(BadRuntime runtime) : base(runtime) { }

    /// <summary>
    ///     The Startup Directory where all containing scripts will be loaded at every execution
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets raised if the startup directory is not set</exception>
    private static string StartupDirectory
    {
        get
        {
            string? s = BadSettingsProvider.RootSettings.FindProperty<string>("Subsystems.Run.StartupDirectory");

            if (s == null)
            {
                throw new BadRuntimeException("Subsystems.Run.StartupDirectory not set");
            }

            BadFileSystem.Instance.CreateDirectory(s);

            return s;
        }
    }

    /// <inheritdoc />
    public override string Name => "run";


    /// <inheritdoc />
    protected override async Task<int> Run(BadRunSystemSettings settings)
    {
        BadNetworkConsoleHost? host = null;

        if (settings.RemotePort != -1)
        {
            host = new BadNetworkConsoleHost(new TcpListener(IPAddress.Any, settings.RemotePort));
            host.Start();
            Runtime.UseConsole(host);
        }

        Runtime.UseStartupArguments(settings.Args);
        IEnumerable<string> files = BadFileSystem.Instance.GetFiles(
                StartupDirectory,
                $".{BadRuntimeSettings.Instance.FileExtension}",
                true
            )
            .Concat(settings.Files);

        if (settings.Interactive)
        {
            if (settings.Benchmark)
            {
                BadLogger.Warn("Benchmarking is not supported in interactive mode");
            }

            await Runtime.RunInteractiveAsync(files);

            return 0;
        }

        Stopwatch? sw = null;

        if (settings.Benchmark)
        {
            sw = Stopwatch.StartNew();
        }

        if (settings.Debug)
        {
            Runtime.UseScriptDebugger();
        }

        foreach (string file in files)
        {
            Runtime.ExecuteFile(file);
        }

        if (settings.Benchmark)
        {
            sw?.Stop();
            BadLogger.Log($"Execution Time: {sw?.ElapsedMilliseconds ?? 0}ms", "Benchmark");
        }

        host?.Stop();

        return 0;
    }
}