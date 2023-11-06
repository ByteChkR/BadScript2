using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary;

/// <summary>
///     Implements the Assign Expression
///     <Left> = <Right>
/// </summary>
public class BadAssignExpression : BadExpression
{
	/// <summary>
	///     Constructor of the Assign Expression
	/// </summary>
	/// <param name="left">Left side that the right side will be assigned to</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">Source position of the Expression</param>
	public BadAssignExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        false,
        position
    )
    {
        Left = left;
        Right = right;
    }

	/// <summary>
	///     Left side that the right side will be assigned to
	/// </summary>
	public BadExpression Left { get; set; }

	/// <summary>
	///     Right side of the Expression
	/// </summary>
	public BadExpression Right { get; set; }

    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression expression in Left.GetDescendantsAndSelf())
        {
            yield return expression;
        }

        foreach (BadExpression? expression in Right.GetDescendantsAndSelf())
        {
            yield return expression;
        }
    }

    public override void Optimize()
    {
        Left = BadConstantFoldingOptimizer.Optimize(Left);
        Right = BadConstantFoldingOptimizer.Optimize(Right);
    }

    public override string ToString()
    {
        return $"({Left} = {Right})";
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        if (context.Scope.IsError)
        {
            yield break;
        }

        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        if (context.Scope.IsError)
        {
            yield break;
        }

        right = right.Dereference();

        if (left is BadObjectReference reference)
        {
            reference.Set(right);
        }
        else
        {
            throw new BadRuntimeException($"Left handside of {this} is not a reference", Position);
        }

        yield return left;
    }
}