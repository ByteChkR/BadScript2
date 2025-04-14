using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Task;
using BadScript2.IO;
using BadScript2.Optimizations.Folding;
using BadScript2.Optimizations.Substitution;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;


/// <summary>
/// Contains the interactive console Implementation
/// </summary>
namespace BadScript2.Interactive;

/// <summary>
///     Implements an Interactive Console for the BadScript Language
/// </summary>
public class BadInteractiveConsole : IDisposable
{
	/// <summary>
	///     The Interactive API
	/// </summary>
	private readonly BadInteractiveConsoleApi m_Api;

	/// <summary>
	///     The Task runner
	/// </summary>
	private readonly BadTaskRunner m_Runner;

	/// <summary>
	///     The Execution Context Options
	/// </summary>
	private readonly BadRuntime m_Runtime;

	/// <summary>
	///     The Execution Context
	/// </summary>
	private BadExecutionContext? m_Context;

	/// <summary>
	///     Constructs a new BadInteractiveConsole instance
	/// </summary>
	/// <param name="runtime">The Runtime that the Interactive Console will be started from.</param>
	/// <param name="runner">The Task runner</param>
	/// <param name="files">The Files that are loaded before the interactive session begins</param>
	public BadInteractiveConsole(BadRuntime runtime, BadTaskRunner runner, IEnumerable<string> files)
    {
        m_Runner = runner;
        m_Api = new BadInteractiveConsoleApi(this);
        m_Runtime = runtime;
        Reset();

        foreach (string file in files)
        {
            LoadIsolated(file);
        }
    }

	/// <summary>
	///     The Current Scope of the Interactive Console
	/// </summary>
	public BadScope? CurrentScope => m_Context?.Scope;

	/// <summary>
	///     If true, the Interactive Console will catch and print errors
	/// </summary>
	public bool CatchErrors { get; set; }

	/// <summary>
	///     If true, the Interactive Console will pre-parse the input before executing it
	/// </summary>
	public bool PreParse { get; set; }

	/// <summary>
	///     Creates a new Execution Context with the Interactive Console Api
	/// </summary>
	/// <returns>Execution Context</returns>
	private BadExecutionContext CreateContext()
    {
        BadExecutionContext ctx = m_Runtime.CreateContext(BadFileSystem.Instance.GetCurrentDirectory());
        BadTable apiTable = new BadTable();
        m_Api.Load(ctx, apiTable);

        ctx.Scope.DefineVariable(m_Api.Name, apiTable);

        return ctx;
    }


	/// <summary>
	///     Resets the Current Context
	/// </summary>
	public void Reset()
    {
	    m_Context?.Dispose();
        m_Context = CreateContext();
    }

	/// <summary>
	///     Loads a File isolated from the Interactive Session
	/// </summary>
	/// <param name="file">The File to be Executed</param>
	/// <returns>Result of the Execution</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the context was not initialized</exception>
	public BadObject LoadIsolated(string file)
    {
        if (m_Context == null)
        {
            throw new BadRuntimeException("Context is not initialized");
        }

        Reset();
        BadExecutionContext current = m_Context;
        BadSourceParser parser = BadSourceParser.Create(file, BadFileSystem.ReadAllText(file));
        Run(parser.Parse());

        Reset();

        return current.Scope.ReturnValue ?? current.Scope.GetTable();
    }

	/// <summary>
	///     Loads a File into the Interactive Session
	/// </summary>
	/// <param name="file">The File to be Loaded</param>
	public void Load(string file)
    {
        BadSourceParser parser = BadSourceParser.Create(file, BadFileSystem.ReadAllText(file));
        Run(parser.Parse());
    }

	/// <summary>
	///     The Routine that is used to execute the Interactive Session
	/// </summary>
	/// <param name="expressions">The Expressions to be executed</param>
	/// <returns>Enumeration of objects</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the context was not initialized</exception>
	private IEnumerable<object?> RunRoutine(IEnumerable<BadExpression> expressions)
    {
        IEnumerable<BadExpression> exprs = expressions;

        if (PreParse)
        {
            exprs = exprs.ToArray();
        }

        if (BadNativeOptimizationSettings.Instance.UseConstantFoldingOptimization)
        {
            exprs = BadConstantFoldingOptimizer.Optimize(exprs);
        }

        if (BadNativeOptimizationSettings.Instance.UseConstantSubstitutionOptimization)
        {
            exprs = BadConstantSubstitutionOptimizer.Optimize(exprs);
        }

        if (m_Context == null)
        {
            throw new BadRuntimeException("Context is not initialized");
        }

        m_Runner.AddTask(new BadTask(BadRunnable.Create(m_Context.Execute(exprs)), "Main"), true);

        while (!m_Runner.IsIdle)
        {
            m_Runner.RunStep();

            yield return null;
        }
    }

	/// <summary>
	///     Runs a set of Expressions
	/// </summary>
	/// <param name="expressions">The Expressions to be Executed</param>
	/// <exception cref="BadRuntimeException">Gets raised if the context was not initialized</exception>
	private void Run(IEnumerable<BadExpression> expressions)
    {
        IEnumerable<BadExpression> exprs = expressions;

        if (PreParse)
        {
            exprs = exprs.ToArray();
        }

        if (BadNativeOptimizationSettings.Instance.UseConstantFoldingOptimization)
        {
            exprs = BadConstantFoldingOptimizer.Optimize(exprs);
        }

        if (m_Context == null)
        {
            throw new BadRuntimeException("Context is not initialized");
        }

        m_Runner.AddTask(new BadTask(BadRunnable.Create(m_Context.Execute(exprs)), "Main"), true);

        if (CatchErrors)
        {
            try
            {
                while (!m_Runner.IsIdle)
                {
                    m_Runner.RunStep();
                }
            }
            catch (Exception e)
            {
                BadConsole.WriteLine(e.Message);
            }
        }
        else
        {
            while (!m_Runner.IsIdle)
            {
                m_Runner.RunStep();
            }
        }
    }

	/// <summary>
	///     Runs a set of Expressions isolated from the Interactive Session
	/// </summary>
	/// <param name="code">The Source Code</param>
	/// <returns>The Results of the Execution</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the context was not initialized</exception>
	public IEnumerable<object?> RunIsolatedRoutine(string code)
    {
        if (m_Context == null)
        {
            throw new BadRuntimeException("Context is not initialized");
        }

        BadExecutionContext ctx = m_Context;
        Reset();
        BadSourceParser parser = BadSourceParser.Create("<stdin>", code);

        foreach (object? o in RunRoutine(parser.Parse()))
        {
            yield return o;
        }

        m_Context = ctx;
    }

	/// <summary>
	///     Runs a set of Expressions isolated from the Interactive Session
	/// </summary>
	/// <param name="code">The Source Code</param>
	/// <returns>The Result of the Execution</returns>
	/// <exception cref="BadRuntimeException">Gets raised if the context was not initialized</exception>
	public BadObject RunIsolated(string code)
    {
        if (m_Context == null)
        {
            throw new BadRuntimeException("Context is not initialized");
        }

        BadExecutionContext ctx = m_Context;
        Reset();
        BadExecutionContext current = m_Context;
        BadSourceParser parser = BadSourceParser.Create("<stdin>", code);
        Run(parser.Parse());
        m_Context = ctx;

        return current.Scope.ReturnValue ?? current.Scope.GetTable();
    }

	/// <summary>
	///     Runs a set of Expressions
	/// </summary>
	/// <param name="code">The Source Code</param>
	public void Run(string code)
    {
        BadSourceParser parser = BadSourceParser.Create("<stdin>", code);
        Run(parser.Parse());
    }

	/// <inheritdoc />
	public void Dispose()
	{
		m_Context?.Dispose();
	}
}