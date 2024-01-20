using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.ControlFlow;

/// <summary>
///     Implements the Break Expression that is used to skip a loop iteraion
/// </summary>
public class BadContinueExpression : BadExpression
{
	/// <summary>
	///     Constructor of the Continue Expression
	/// </summary>
	/// <param name="position">Source Position of the Expression</param>
	public BadContinueExpression(BadSourcePosition position) : base(false, position) { }

	/// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        context.Scope.SetContinue();

        yield return BadObject.Null;
    }

	/// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        yield break;
    }
}