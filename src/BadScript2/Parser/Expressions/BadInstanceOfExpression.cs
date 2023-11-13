using BadScript2.Common;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions;

/// <summary>
///     Implements the 'instanceof' operator.
/// </summary>
public class BadInstanceOfExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor for the 'instanceof' operator
    /// </summary>
    /// <param name="left">Left Side of the Expression</param>
    /// <param name="right">Right Side of the Expression</param>
    /// <param name="position">The Source Position</param>
    public BadInstanceOfExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
		right,
		position) { }

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject left = BadObject.Null;

		foreach (BadObject o in Left.Execute(context))
		{
			left = o;

			yield return o;
		}

		left = left.Dereference();
		BadObject right = BadObject.Null;

		foreach (BadObject o in Right.Execute(context))
		{
			right = o;

			yield return o;
		}

		right = right.Dereference();

		if (context.Scope.IsError)
		{
			yield break;
		}

		if (right is not BadClassPrototype type)
		{
			throw BadRuntimeException.Create(context.Scope, "Right side of instanceof must be a class", Position);
		}


		yield return type.IsSuperClassOf(left.GetPrototype());
	}

	protected override string GetSymbol()
	{
		return "instanceof";
	}
}
