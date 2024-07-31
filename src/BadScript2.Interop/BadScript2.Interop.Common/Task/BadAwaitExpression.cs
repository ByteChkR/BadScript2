using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

///<summary>
///	Contains task/async Extensions and Integrations for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Implements the 'await' expression
/// </summary>
public class BadAwaitExpression : BadExpression
{
    /// <summary>
    ///     The Task Expression
    /// </summary>
    private readonly BadExpression m_TaskExpr;

    /// <summary>
    ///     Constructs a new Await Expression
    /// </summary>
    /// <param name="expr">Task Expression</param>
    /// <param name="position">Source Position</param>
    public BadAwaitExpression(BadExpression expr, BadSourcePosition position) : base(false, position)
    {
        m_TaskExpr = expr;
    }

    /// <inheritdoc />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return m_TaskExpr.GetDescendantsAndSelf();
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject obj = BadObject.Null;

        foreach (BadObject o in m_TaskExpr.Execute(context))
        {
            obj = o;

            yield return o;
        }

        obj = obj.Dereference();

        if (obj is not BadTask task)
        {
            throw new BadRuntimeException("await can only be used on a task", Position);
        }

        if (task.IsFinished)
        {
            yield return task.Runnable.GetReturn();

            yield break;
        }

        BadTaskRunner runner = context.Scope.GetSingleton<BadTaskRunner>() ??
                               throw BadRuntimeException.Create(context.Scope, "Task Runner not found", Position);

        //Run Task
        //Add current to continuation
        task.AddContinuation(runner.Current ?? throw new BadRuntimeException("Current task is null", Position));
        runner.Current?.Pause();

        if (task.IsInactive)
        {
            runner.AddTask(task, true);
        }

        yield return BadObject.Null; //Should pause Here

        if (task.Runnable.Error != null)
        {
            throw new BadRuntimeErrorException(task.Runnable.Error);
        }

        yield return task.Runnable.GetReturn();
    }
}