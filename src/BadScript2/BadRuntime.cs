using BadScript2.Common.Logging;
using BadScript2.Common.Logging.Writer;
using BadScript2.ConsoleAbstraction;
using BadScript2.Debugging;
using BadScript2.IO;
using BadScript2.Optimizations.Folding;
using BadScript2.Optimizations.Substitution;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

namespace BadScript2;

public class BadRuntime : IDisposable
{
    private readonly List<IDisposable> m_Disposables = new List<IDisposable>();
    public readonly BadExecutionContextOptions Options;
    private Func<BadExecutionContext, IEnumerable<BadExpression>, BadObject> m_Executor = Executor;

    public BadRuntime(BadExecutionContextOptions options)
    {
        Options = options;
        BadSettingsProvider.SetRootSettings(new BadSettings());
    }

    public BadRuntime() : this(BadExecutionContextOptions.Default) { }

    public void Dispose()
    {
        foreach (IDisposable disposable in m_Disposables)
        {
            disposable.Dispose();
        }
    }

    public BadRuntime UseExecutor(Func<BadExecutionContext, IEnumerable<BadExpression>, BadObject> executor)
    {
        m_Executor = executor;

        return this;
    }

    private static BadObject Executor(BadExecutionContext ctx, IEnumerable<BadExpression> exprs)
    {
        return ctx.ExecuteScript(exprs);
    }

    public BadRuntime UseLogMask(params BadLogMask[] mask)
    {
        BadLogWriterSettings.Instance.Mask = BadLogMask.GetMask(mask);

        return this;
    }

    public BadRuntime UseLogMask(params string[] mask)
    {
        return UseLogMask(mask.Select(x => (BadLogMask)x).ToArray());
    }

    public BadRuntime UseConsole(IBadConsole console)
    {
        BadConsole.SetConsole(console);

        return this;
    }

    public BadRuntime UseLogWriter(BadLogWriter writer)
    {
        writer.Register();
        m_Disposables.Add(writer);

        return this;
    }


    public BadRuntime UseConsoleLogWriter()
    {
        return UseLogWriter(new BadConsoleLogWriter());
    }

    public BadRuntime UseFileLogWriter(string path)
    {
        return UseLogWriter(new BadFileLogWriter(path));
    }

    public BadRuntime LoadSettings(string settingsFile)
    {
        BadLogger.Log("Loading Settings...", "Settings");
        BadSettingsReader settingsReader = new BadSettingsReader(
            BadSettingsProvider.RootSettings,
            Path.Combine(settingsFile)
        );

        BadSettingsProvider.SetRootSettings(settingsReader.ReadSettings());
        BadLogger.Log("Settings loaded!", "Settings");

        return this;
    }

    public BadRuntime UseCompilerApi()
    {
        Options.AddApi(new BadCompilerApi());

        return this;
    }

    public BadRuntime UseDebugger(IBadDebugger debugger)
    {
        if (BadDebugger.IsAttached)
        {
            BadDebugger.Detach();
        }

        BadDebugger.Attach(debugger);

        return this;
    }


    public BadObject Execute(IEnumerable<BadExpression> expressions)
    {
        BadExecutionContext ctx = Options.Build();

        return m_Executor(ctx, expressions);
    }
    

    public BadObject Execute(string source)
    {
        return Execute(Parse(source));
    }

    public BadObject Execute(string source, string file)
    {
        return Execute(Parse(source, file));
    }

    public BadObject ExecuteFile(string file)
    {
        return Execute(ParseFile(file));
    }

    public IEnumerable<BadExpression> Parse(string source)
    {
        return Parse("<memory>", source);
    }

    public IEnumerable<BadExpression> Parse(string source, string file)
    {
        BadSourceParser parser = BadSourceParser.Create(file, source);

        IEnumerable<BadExpression> result = parser.Parse();
        if (BadNativeOptimizationSettings.Instance.UseConstantFoldingOptimization)
        {
            result = BadConstantFoldingOptimizer.Optimize(result);
        }

        if (BadNativeOptimizationSettings.Instance.UseConstantSubstitutionOptimization)
        {
            result = BadConstantSubstitutionOptimizer.Optimize(result);
        }

        return result;
    }

    public IEnumerable<BadExpression> ParseFile(string file)
    {
        return Parse(BadFileSystem.ReadAllText(file), file);
    }
}