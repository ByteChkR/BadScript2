using System.Diagnostics;

using BadScript2.Common.Logging;
using BadScript2.Console.Debugging.Scriptable;
using BadScript2.Debugging;
using BadScript2.Interactive;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Apis;
using BadScript2.Optimizations;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;
using BadScript2.Settings;

namespace BadScript2.Console.Systems.Run;

public class BadRunSystem : BadConsoleSystem<BadRunSystemSettings>
{
    private readonly BadTaskRunner m_Runner = new BadTaskRunner();

    private string StartupDirectory
    {
        get
        {
            string? s = BadSettingsProvider.RootSettings.FindProperty<string>("Subsystems.Run.StartupDirectory");
            if (s == null)
            {
                throw new BadRuntimeException("Subsystems.Run.StartupDirectory not set");
            }

            Directory.CreateDirectory(s);

            return s;
        }
    }

    public override string Name => "run";

    private int RunInteractive(BadExecutionContextOptions options, IEnumerable<string> files)
    {
        BadInteractiveConsole console = new BadInteractiveConsole(options, m_Runner, files);
        while (true)
        {
            System.Console.Write(">");
            string cmd = System.Console.ReadLine()!;

            if (cmd == "exit")
            {
                return -1;
            }

            console.Run(cmd);
        }
    }

    private BadExecutionContextOptions CreateOptions()
    {
        BadExecutionContextOptions options = new BadExecutionContextOptions(BadExecutionContextOptions.Default.Apis);
        options.Apis.Add(new BadTaskRunnerApi(m_Runner));

        return options;
    }


    private IEnumerable<BadObject> Run(BadExecutionContext context, IEnumerable<BadObject> exprs)
    {
        foreach (BadObject o in exprs)
        {
            yield return o;
        }

        if (context.Scope.IsError)
        {
            System.Console.WriteLine("Error: " + context.Scope.Error);
        }
    }

    protected override int Run(BadRunSystemSettings settings)
    {
        BadExecutionContextOptions options = CreateOptions();
        BadRuntimeApi.StartupArguments = settings.Args;
        IEnumerable<string> files = Directory.GetFiles(
                StartupDirectory,
                $"*.{BadRuntimeSettings.Instance.FileExtension}",
                SearchOption.AllDirectories
            )
            .Concat(settings.Files);
        if (settings.Interactive)
        {
            if (settings.Benchmark)
            {
                BadLogger.Warn("Benchmarking is not supported in interactive mode");
            }

            return RunInteractive(options, files);
        }

        Stopwatch? sw = null;
        if (settings.Benchmark)
        {
            sw = Stopwatch.StartNew();
        }

        if (settings.Debug)
        {
            BadDebugger.Attach(new BadScriptDebugger(options));
        }

        foreach (string file in files)
        {
            BadSourceParser parser = BadSourceParser.Create(file, File.ReadAllText(file));
            BadExecutionContext context = options.Build();

            IEnumerable<BadExpression> exprs = parser.Parse();
            if (BadNativeOptimizationSettings.Instance.UseConstantExpressionOptimization)
            {
                exprs = BadExpressionOptimizer.Optimize(exprs);
            }

            m_Runner.AddTask(new BadTask(Run(context, context.Execute(exprs)).GetEnumerator(), "Main"), true);
            while (!m_Runner.IsIdle)
            {
                m_Runner.RunStep();
            }
        }

        if (settings.Benchmark)
        {
            sw?.Stop();
            BadLogger.Log($"Execution Time: {sw?.ElapsedMilliseconds ?? 0}ms", "Benchmark");
        }

        return -1;
    }
}