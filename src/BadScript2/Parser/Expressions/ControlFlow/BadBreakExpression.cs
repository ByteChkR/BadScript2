using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
/// <summary>
/// Contains the Controlflow Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions.ControlFlow;

/// <summary>
///     Implements the Break Expression that is used to prematurely exit a loop
/// </summary>
public class BadBreakExpression : BadExpression
{
	/// <summary>
	///     Constructor of the Break Expression
	/// </summary>
	/// <param name="position">Source Position of the Expression</param>
	public BadBreakExpression(BadSourcePosition position) : base(false, position) { }

	/// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        context.Scope.SetBreak();

        yield return BadObject.Null;
    }

	/// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        yield break;
    }
}