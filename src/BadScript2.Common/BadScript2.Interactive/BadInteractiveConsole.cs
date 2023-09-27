using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Task;
using BadScript2.IO;
using BadScript2.Optimizations;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;

namespace BadScript2.Interactive;

/// <summary>
///     Implements an Interactive Console for the BadScript Language
/// </summary>
public class BadInteractiveConsole
{
	/// <summary>
	///     The Interactive API
	/// </summary>
	private readonly BadInteractiveConsoleApi m_Api;

	/// <summary>
	///     The Execution Context Options
	/// </summary>
	private readonly BadExecutionContextOptions m_Options;

	/// <summary>
	///     The Task runner
	/// </summary>
	private readonly BadTaskRunner m_Runner;

	/// <summary>
	///     The Execution Context
	/// </summary>
	private BadExecutionContext? m_Context;

	/// <summary>
	///     Constructs a new BadInteractiveConsole instance
	/// </summary>
	/// <param name="options">The Execution Context Options</param>
	/// <param name="runner">The Task runner</param>
	/// <param name="files">The Files that are loaded before the interactive session begins</param>
	public BadInteractiveConsole(BadExecutionContextOptions options, BadTaskRunner runner, IEnumerable<string> files)
    {
        m_Runner = runner;
        m_Api = new BadInteractiveConsoleApi(this);
        m_Options = options;
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
        BadTable apiTable = new BadTable();
        m_Api.Load(apiTable);

        BadExecutionContext ctx = m_Options.Build();

        ctx.Scope.AddSingleton(BadTaskRunner.Instance);
        ctx.Scope.DefineVariable(m_Api.Name, apiTable);

        return ctx;
    }


	/// <summary>
	///     Resets the Current Context
	/// </summary>
	public void Reset()
    {
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

        BadExecutionContext ctx = m_Context;
        Reset();
        BadExecutionContext current = m_Context;
        BadSourceParser parser = BadSourceParser.Create(file, BadFileSystem.ReadAllText(file));
        Run(parser.Parse());

        m_Context = ctx;

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
            exprs = expressions.ToArray();
        }

        if (BadNativeOptimizationSettings.Instance.UseConstantExpressionOptimization)
        {
            exprs = BadExpressionOptimizer.Optimize(exprs);
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

        if (m_Context.Scope.Error != null)
        {
            BadConsole.WriteLine("Error: " + m_Context.Scope.Error);
            m_Context.Scope.UnsetError();
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
            exprs = expressions.ToArray();
        }

        if (BadNativeOptimizationSettings.Instance.UseConstantExpressionOptimization)
        {
            exprs = BadExpressionOptimizer.Optimize(exprs);
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

        if (m_Context.Scope.Error != null)
        {
            BadConsole.WriteLine("Error: " + m_Context.Scope.Error);
            m_Context.Scope.UnsetError();
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
}