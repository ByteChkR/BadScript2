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

public class BadInteractiveConsole
{
    private readonly BadInteractiveConsoleApi m_Api;
    private readonly BadExecutionContextOptions m_Options;
    private readonly BadTaskRunner m_Runner;
    private BadExecutionContext? m_Context;

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

    public BadScope? CurrentScope => m_Context?.Scope;

    public bool CatchErrors { get; set; } = false;
    public bool PreParse { get; set; } = false;

    private BadExecutionContext CreateContext()
    {
        BadTable apiTable = new BadTable();
        m_Api.Load(apiTable);

        BadExecutionContext ctx = m_Options.Build();
        ctx.Scope.DefineVariable(m_Api.Name, apiTable);

        return ctx;
    }


    public void Reset()
    {
        m_Context = CreateContext();
    }


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

    public void Load(string file)
    {
        BadSourceParser parser = BadSourceParser.Create(file, BadFileSystem.ReadAllText(file));
        Run(parser.Parse());
    }

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
            Console.WriteLine("Error: " + m_Context.Scope.Error);
            m_Context.Scope.UnsetError();
        }
    }

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
                Console.WriteLine(e.Message);
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
            Console.WriteLine("Error: " + m_Context.Scope.Error);
            m_Context.Scope.UnsetError();
        }
    }

    public IEnumerable<object?> RunIsolatedRoutine(string code)
    {
        if (m_Context == null)
        {
            throw new BadRuntimeException("Context is not initialized");
        }

        BadExecutionContext ctx = m_Context;
        Reset();
        BadExecutionContext current = m_Context;
        BadSourceParser parser = BadSourceParser.Create("<stdin>", code);
        foreach (object? o in RunRoutine(parser.Parse()))
        {
            yield return o;
        }

        m_Context = ctx;
    }

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

    public void Run(string code)
    {
        BadSourceParser parser = BadSourceParser.Create("<stdin>", code);
        Run(parser.Parse());
    }
}