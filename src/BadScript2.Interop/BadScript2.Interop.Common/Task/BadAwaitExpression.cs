using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Implements the 'await' expression
/// </summary>
public class BadAwaitExpression : BadExpression
{
	/// <summary>
	///     The Task Expression
	/// </summary>
	private readonly BadExpression TaskExpr;

	/// <summary>
	///     Constructs a new Await Expression
	/// </summary>
	/// <param name="expr">Task Expression</param>
	/// <param name="position">Source Position</param>
	public BadAwaitExpression(BadExpression expr, BadSourcePosition position) : base(false, position)
	{
		TaskExpr = expr;
	}

	public override IEnumerable<BadExpression> GetDescendants()
	{
		foreach (BadExpression expression in TaskExpr.GetDescendantsAndSelf())
		{
			yield return expression;
		}
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

		BadTaskRunner runner = context.Scope.GetSingleton<BadTaskRunner>();

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
			context.Scope.SetError($"Task {task.Name} failed", task.Runnable.Error);

			yield return BadObject.Null;
		}
		else
		{
			yield return task.Runnable.GetReturn();
		}
	}
}
