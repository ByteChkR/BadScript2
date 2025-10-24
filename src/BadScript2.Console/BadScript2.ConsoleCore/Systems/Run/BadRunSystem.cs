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
    public BadRunSystem(Func<BadRuntime> runtime) : base(runtime) { }

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


    private async Task RunParallel(BadRunSystemSettings settings)
    {
        BadNetworkConsoleHost? host = null;

        using var runtime = RuntimeFactory();
        if (settings.RemotePort != -1)
        {
            host = new BadNetworkConsoleHost(new TcpListener(IPAddress.Any, settings.RemotePort));
            host.Start();
            runtime.UseConsole(host);
        }

        runtime.UseStartupArguments(settings.Args);

        IEnumerable<string> files = BadFileSystem.Instance.GetFiles(StartupDirectory,
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

            await runtime.RunInteractiveAsync(files);

            return;
        }

        Stopwatch? sw = null;

        if (settings.Benchmark)
        {
            sw = Stopwatch.StartNew();
        }

        if (settings.Debug)
        {
            runtime.UseScriptDebugger();
        }

        foreach (string file in files)
        {
            runtime.ExecuteFile(file);
        }

        if (settings.Benchmark)
        {
            sw?.Stop();
            BadLogger.Log($"Execution Time: {sw?.ElapsedMilliseconds ?? 0}ms", "Benchmark");
        }

        host?.Stop();
    }
    /// <inheritdoc />
    protected override async Task<int> Run(BadRunSystemSettings settings)
    {
        if (settings.Parallelization <= 1)
        {
            await RunParallel(settings);
            return 0;
        }

        Console.WriteLine("Waiting 1 second before starting parallel execution...");
        await Task.Delay(1000);
        BadRuntimeSettings.Instance.CatchRuntimeExceptions = false;
        BadRuntimeSettings.Instance.WriteStackTraceInRuntimeErrors = true;
        await Parallel.ForAsync(0, 100000, new ParallelOptions{MaxDegreeOfParallelism = settings.Parallelization}, async (i, _) =>
        {
            try
            {
                await RunParallel(settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
        return 0;
    }
}