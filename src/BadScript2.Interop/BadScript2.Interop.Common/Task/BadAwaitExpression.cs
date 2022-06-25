using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Task;

public class BadAwaitExpression : BadExpression
{
    public readonly BadExpression TaskExpr;

    public BadAwaitExpression(BadExpression expr, BadSourcePosition position) : base(false, false, position)
    {
        TaskExpr = expr;
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject obj = BadObject.Null;
        foreach (BadObject o in TaskExpr.Execute(context))
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

        //Run Task
        //Add current to continuation
        task.ContinuationTasks.Add(
            BadTaskRunner.Instance.Current ?? throw new BadRuntimeException("Current task is null", Position)
        );
        BadTaskRunner.Instance.Current?.Pause();
        if (task.IsInactive)
        {
            BadTaskRunner.Instance.AddTask(task);
        }

        yield return BadObject.Null; //Should pause Here

        yield return task.Runnable.GetReturn();
    }
}