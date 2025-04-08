using System.Globalization;

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
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Module;
using BadScript2.Runtime.Module.Handlers;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

/// <summary>
/// The main namespace for the BadScript2 Language
/// </summary>
namespace BadScript2;

/// <summary>
///     Exposes the BadScript Runtime Functionality to Consumers
/// </summary>
public class BadRuntime : IDisposable
{
    /// <summary>
    ///     Configuration Actions for the Context
    /// </summary>
    private readonly List<Action<BadExecutionContext>> m_ConfigureContext = new List<Action<BadExecutionContext>>();

    private readonly List<Action<BadExecutionContext, string, BadModuleImporter>> m_ConfigureModuleImporter =
        new List<Action<BadExecutionContext, string, BadModuleImporter>>();

    /// <summary>
    ///     Configuration Actions for the Options
    /// </summary>
    private readonly List<Action<BadExecutionContextOptions>> m_ConfigureOptions =
        new List<Action<BadExecutionContextOptions>>();

    /// <summary>
    ///     List of Disposables
    /// </summary>
    private readonly List<IDisposable> m_Disposables = new List<IDisposable>();

    /// <summary>
    ///     The Options for the Context
    /// </summary>
    private readonly BadExecutionContextOptions m_Options;

    /// <summary>
    ///     The Executor Function used to Execute a list of Expressions
    /// </summary>
    private Func<BadExecutionContext, IEnumerable<BadExpression>, BadObject> m_Executor = Executor;

    /// <summary>
    ///     Creates a new BadScript Runtime with the specified options
    /// </summary>
    /// <param name="options">The Options to use</param>
    public BadRuntime(BadExecutionContextOptions options)
    {
        m_Options = options;

        if (!BadSettingsProvider.HasRootSettings)
        {
            BadSettingsProvider.SetRootSettings(new BadSettings(string.Empty));
        }
    }

    /// <summary>
    ///     Creates a new BadScript Runtime with the default options
    /// </summary>
    public BadRuntime() : this(new BadExecutionContextOptions()) { }

    public CultureInfo Culture { get; private set; } = CultureInfo.InvariantCulture;

    private BadModuleStore ModuleStore { get; } = new BadModuleStore();

    private BadModuleImporter? Importer { get; set; }

#region IDisposable Members

    //private BadModuleImporter? Importer { get; set; }

    /// <summary>
    ///     Disposes all Disposables
    /// </summary>
    public void Dispose()
    {
        foreach (IDisposable disposable in m_Disposables)
        {
            disposable.Dispose();
        }
    }

#endregion

    /// <summary>
    ///     Clone this Runtime
    /// </summary>
    /// <returns>Cloned Runtime</returns>
    public BadRuntime Clone()
    {
        BadRuntime r = new BadRuntime(CreateOptions())
            .UseExecutor(m_Executor);

        //Copy the configurators that are not part of the options
        r.m_ConfigureModuleImporter.AddRange(m_ConfigureModuleImporter);
        r.m_ConfigureContext.AddRange(m_ConfigureContext);

        return r;
    }

    /// <summary>
    ///     Configures the Runtime to use the specified Executor
    /// </summary>
    /// <param name="executor">The Executor to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseExecutor(Func<BadExecutionContext, IEnumerable<BadExpression>, BadObject> executor)
    {
        m_Executor = executor;

        return this;
    }

    /// <summary>
    ///     The Default Executor Function
    /// </summary>
    /// <param name="ctx">The Context to use</param>
    /// <param name="exprs">The Expressions to execute</param>
    /// <returns>The Result of the Execution</returns>
    private static BadObject Executor(BadExecutionContext ctx, IEnumerable<BadExpression> exprs)
    {
        return ctx.ExecuteScript(exprs);
    }

    /// <summary>
    ///     Configures the Runtime to use the specified log masks
    /// </summary>
    /// <param name="mask">The Log Masks to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseLogMask(params BadLogMask[] mask)
    {
        BadLogWriterSettings.Instance.Mask = BadLogMask.GetMask(mask);

        return this;
    }

    /// <summary>
    ///     Configures the Runtime to use the specified log masks
    /// </summary>
    /// <param name="mask">The Log Masks to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseLogMask(params string[] mask)
    {
        return UseLogMask(mask.Select(x => (BadLogMask)x)
                              .ToArray()
                         );
    }

    /// <summary>
    ///     Configures the Runtime to use the specified console implementation
    /// </summary>
    /// <param name="console">The Console to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseConsole(IBadConsole console)
    {
        BadConsole.SetConsole(console);

        return this;
    }

    /// <summary>
    ///     Configures the Runtime to use the specified logwriter implementation
    /// </summary>
    /// <param name="writer">The LogWriter to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseLogWriter(BadLogWriter writer)
    {
        writer.Register();
        m_Disposables.Add(writer);

        return this;
    }


    /// <summary>
    ///     Configures the Runtime to use the default log writer implementation(log to console)
    /// </summary>
    /// <returns>This Runtime</returns>
    public BadRuntime UseConsoleLogWriter()
    {
        return UseLogWriter(new BadConsoleLogWriter());
    }

    /// <summary>
    ///     Configures the Runtime to use the file log writer implementation
    /// </summary>
    /// <param name="path">The path to the log file</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseFileLogWriter(string path)
    {
        return UseLogWriter(new BadFileLogWriter(path));
    }

    /// <summary>
    ///     Loads the specified settings file
    /// </summary>
    /// <param name="settingsFile">The path to the settings file</param>
    /// <returns>This Runtime</returns>
    public BadRuntime LoadSettings(string settingsFile, IFileSystem? fileSystem = null)
    {
        BadLogger.Log("Loading Settings...", "Settings");

        BadSettingsReader settingsReader = new BadSettingsReader(BadSettingsProvider.RootSettings,
                                                                 fileSystem ?? BadFileSystem.Instance,
                                                                 settingsFile
                                                                );

        BadSettingsProvider.SetRootSettings(settingsReader.ReadSettings());
        BadLogger.Log("Settings loaded!", "Settings");

        return this;
    }

    /// <summary>
    ///     Configures the Runtime to expose the CompilerAPI to the Scripts
    /// </summary>
    /// <returns>This Runtime</returns>
    public BadRuntime UseCompilerApi()
    {
        m_Options.AddApi(new BadCompilerApi());

        return this;
    }

    /// <summary>
    ///     Configures the Runtime to use the specified Debugger
    /// </summary>
    /// <param name="debugger">The Debugger to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseDebugger(IBadDebugger debugger)
    {
        if (BadDebugger.IsAttached)
        {
            BadDebugger.Detach();
        }

        BadDebugger.Attach(debugger);

        return this;
    }

    /// <summary>
    ///     Creates and configures the Options for the Context
    /// </summary>
    /// <returns>The Options for the Context</returns>
    private BadExecutionContextOptions CreateOptions()
    {
        BadExecutionContextOptions opts = m_Options.Clone();

        foreach (Action<BadExecutionContextOptions> config in m_ConfigureOptions)
        {
            config(opts);
        }

        return opts;
    }

    /// <summary>
    ///     Creates a new Context with the configured Options
    /// </summary>
    /// <returns>The new Context</returns>
    public BadExecutionContext CreateContext(string workingDirectory)
    {
        BadExecutionContext ctx = CreateOptions()
            .Build();

        foreach (Action<BadExecutionContext> config in m_ConfigureContext)
        {
            config(ctx);
        }

        ctx.Scope.AddSingleton(this);
        BadModuleImporter importer;

        if (Importer == null)
        {
            importer = Importer = new BadModuleImporter(ModuleStore);
        }
        else
        {
            importer = Importer = Importer.Clone();
        }

        foreach (Action<BadExecutionContext, string, BadModuleImporter> action in m_ConfigureModuleImporter)
        {
            action(ctx, workingDirectory, importer);
        }

        ctx.Scope.AddSingleton(importer);

        return ctx;
    }

    /// <summary>
    ///     Executes the specified expressions
    /// </summary>
    /// <param name="expressions">The Expressions to execute</param>
    /// <returns>The Result of the Execution</returns>
    public BadRuntimeExecutionResult Execute(IEnumerable<BadExpression> expressions, string workingDirectory)
    {
        BadExecutionContext ctx = CreateContext(workingDirectory);

        BadObject? result = m_Executor(ctx, expressions);
        BadObject exports = ctx.Scope.GetExports();

        return new BadRuntimeExecutionResult(result, exports);
    }


    /// <summary>
    ///     Executes the specified script
    /// </summary>
    /// <param name="source">The Script Source to execute</param>
    /// <returns>The Result of the Execution</returns>
    public BadRuntimeExecutionResult Execute(string source)
    {
        return Execute(Parse(source), BadFileSystem.Instance.GetCurrentDirectory());
    }

    /// <summary>
    ///     Executes the specified script
    /// </summary>
    /// <param name="source">The Script Source to execute</param>
    /// <param name="file">The File Path of the Script</param>
    /// <returns>The Result of the Execution</returns>
    public BadRuntimeExecutionResult Execute(string source, string file)
    {
        return Execute(Parse(source, file),
                       Path.GetDirectoryName(BadFileSystem.Instance.GetFullPath(file)) ??
                       BadFileSystem.Instance.GetCurrentDirectory()
                      );
    }

    /// <summary>
    ///     Executes the specified script file
    /// </summary>
    /// <param name="file">The File Path of the Script</param>
    /// <param name="fileSystem">The (optional) Filesystem Instance to use</param>
    /// <returns>The Result of the Execution</returns>
    public BadRuntimeExecutionResult ExecuteFile(string file, IFileSystem? fileSystem = null)
    {
        IFileSystem? fs = fileSystem ?? BadFileSystem.Instance;

        return Execute(ParseFile(file, fs),
                       fs.GetFullPath(Path.GetDirectoryName(fs.GetFullPath(file)) ?? fs.GetCurrentDirectory())
                      );
    }

    /// <summary>
    ///     Parses the specified source
    /// </summary>
    /// <param name="source">The Source to parse</param>
    /// <returns>The Parsed Expressions</returns>
    public static IEnumerable<BadExpression> Parse(string source)
    {
        return Parse(source, "<memory>");
    }

    /// <summary>
    ///     Parses the specified source
    /// </summary>
    /// <param name="source">The Source to parse</param>
    /// <param name="file">The File Path of the Script</param>
    /// <returns>The Parsed Expressions</returns>
    public static IEnumerable<BadExpression> Parse(string source, string file)
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

    /// <summary>
    ///     Parses the specified script file
    /// </summary>
    /// <param name="file">The File Path of the Script</param>
    /// <param name="fileSystem">The (optional) Filesystem Instance to use</param>
    /// <returns>The Parsed Expressions</returns>
    public static IEnumerable<BadExpression> ParseFile(string file, IFileSystem? fileSystem = null)
    {
        IFileSystem? fs = fileSystem ?? BadFileSystem.Instance;

        return Parse(fs.ReadAllText(file), file);
    }


    /// <summary>
    ///     Registers the Local Path Handler to the Module Importer
    /// </summary>
    /// <returns>This Runtime</returns>
    public BadRuntime UseLocalModules(IFileSystem? fs = null)
    {
        return UseImportHandler((workingDir, _) =>
                                    new BadLocalPathImportHandler(this, workingDir, fs ?? BadFileSystem.Instance)
                               );
    }

    /// <summary>
    ///     Adds the specified Option Configuration Actions
    /// </summary>
    /// <param name="action">The Configuration Actions</param>
    /// <returns>This Runtime</returns>
    public BadRuntime ConfigureContextOptions(params Action<BadExecutionContextOptions>[] action)
    {
        m_ConfigureOptions.AddRange(action);

        return this;
    }

    /// <summary>
    ///     Adds or Replaces a specified API
    /// </summary>
    /// <param name="api">The API to add or replace</param>
    /// <param name="replace">If the API should be replaced if it already exists</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseApi(BadInteropApi api, bool replace = false)
    {
        if (replace)
        {
            return ConfigureContextOptions(opt => opt.AddOrReplaceApi(api));
        }

        return ConfigureContextOptions(opt => opt.AddApi(api));
    }


    /// <summary>
    ///     Adds or Replaces a specified APIs
    /// </summary>
    /// <param name="api">The APIs to add or replace</param>
    /// <param name="replace">If the APIs should be replaced if they already exist</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseApis(IEnumerable<BadInteropApi> apis, bool replace = false)
    {
        if (replace)
        {
            return ConfigureContextOptions(opt => opt.AddOrReplaceApis(apis));
        }

        return ConfigureContextOptions(opt => opt.AddApis(apis));
    }


    /// <summary>
    ///     Uses a specified Extension
    /// </summary>
    /// <typeparam name="T">The Extension Type</typeparam>
    /// <returns>This Runtime</returns>
    public BadRuntime UseExtension<T>() where T : BadInteropExtension, new()
    {
        return ConfigureContextOptions(opts => opts.AddExtension<T>());
    }


    /// <summary>
    ///     Configures a Module Importer to use the specified Import Handler
    /// </summary>
    /// <param name="f">Handler Factory</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseImportHandler(Func<BadExecutionContext, string, BadModuleImporter, BadImportHandler> f)
    {
        return ConfigureModuleImporter((ctx, workingDir, importer) => importer.AddHandler(f(ctx, workingDir, importer))
                                      );
    }

    /// <summary>
    ///     Configures a Module Importer to use the specified Import Handler
    /// </summary>
    /// <param name="f">Handler Factory</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseImportHandler(Func<BadExecutionContext, BadModuleImporter, BadImportHandler> f)
    {
        return ConfigureModuleImporter((ctx, importer) => importer.AddHandler(f(ctx, importer)));
    }

    /// <summary>
    ///     Configures a Module Importer to use the specified Import Handler
    /// </summary>
    /// <param name="f">Handler Factory</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseImportHandler(Func<BadModuleImporter, BadImportHandler> f)
    {
        return ConfigureModuleImporter(importer => importer.AddHandler(f(importer)));
    }

    /// <summary>
    ///     Configures a Module Importer to use the specified Import Handler
    /// </summary>
    /// <param name="f">Handler Factory</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseImportHandler(Func<string, BadModuleImporter, BadImportHandler> f)
    {
        return ConfigureModuleImporter((workingDir, importer) => importer.AddHandler(f(workingDir, importer)));
    }

    /// <summary>
    ///     Configures a Module Importer to use the specified Import Handler
    /// </summary>
    /// <param name="handler">The Import Handler to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseImportHandler(BadImportHandler handler)
    {
        return ConfigureModuleImporter(importer => importer.AddHandler(handler));
    }


    /// <summary>
    ///     Configures the Module Importer
    /// </summary>
    /// <param name="action">Module Importer Configuration Action</param>
    /// <returns>This Runtime</returns>
    public BadRuntime ConfigureModuleImporter(Action<BadExecutionContext, string, BadModuleImporter> action)
    {
        m_ConfigureModuleImporter.Add(action);

        return this;
    }

    /// <summary>
    ///     Configures the Module Importer
    /// </summary>
    /// <param name="action">Module Importer Configuration Action</param>
    /// <returns>This Runtime</returns>
    public BadRuntime ConfigureModuleImporter(Action<BadExecutionContext, BadModuleImporter> action)
    {
        return ConfigureModuleImporter((ctx, _, importer) => action(ctx, importer));
    }

    /// <summary>
    ///     Configures the Module Importer
    /// </summary>
    /// <param name="action">Module Importer Configuration Action</param>
    /// <returns>This Runtime</returns>
    public BadRuntime ConfigureModuleImporter(Action<BadModuleImporter> action)
    {
        return ConfigureModuleImporter((_, _, importer) => action(importer));
    }

    /// <summary>
    ///     Configures the Module Importer
    /// </summary>
    /// <param name="action">Module Importer Configuration Action</param>
    /// <returns>This Runtime</returns>
    public BadRuntime ConfigureModuleImporter(Action<string, BadModuleImporter> action)
    {
        return ConfigureModuleImporter((_, workingDir, importer) => action(workingDir, importer));
    }

    /// <summary>
    ///     Uses the specified Singleton Object
    /// </summary>
    /// <param name="obj">The Object to use</param>
    /// <typeparam name="T">Type of the Object</typeparam>
    /// <returns>This Runtime</returns>
    public BadRuntime UseSingleton<T>(T obj) where T : class
    {
        return ConfigureContext(ctx => ctx.Scope.AddSingleton(obj));
    }

    /// <summary>
    ///     Adds the specified Context Configuration Actions
    /// </summary>
    /// <param name="action">The Configuration Actions</param>
    /// <returns>This Runtime</returns>
    public BadRuntime ConfigureContext(params Action<BadExecutionContext>[] action)
    {
        m_ConfigureContext.AddRange(action);

        return this;
    }

    /// <summary>
    /// Uses the specified Culture
    /// </summary>
    /// <param name="culture">The Culture to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseCulture(CultureInfo culture)
    {
        Culture = culture;

        return this;
    }

    /// <summary>
    /// Uses the specified Culture
    /// </summary>
    /// <param name="culture">The Culture to use</param>
    /// <returns>This Runtime</returns>
    public BadRuntime UseCulture(string culture)
    {
        return UseCulture(new CultureInfo(culture));
    }
}