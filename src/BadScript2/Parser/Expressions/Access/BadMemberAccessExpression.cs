using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Access;

/// <summary>
///     Implements the Member Access to set or get properties from an object.
///     LEFT.RIGHT
/// </summary>
public class BadMemberAccessExpression : BadExpression, IBadAccessExpression
{
	/// <summary>
	///     Constructor for the Member Access Expression.
	/// </summary>
	/// <param name="left">Left side of the expression</param>
	/// <param name="right">Right side of the expression</param>
	/// <param name="position">Position inside the source code</param>
	/// <param name="nullChecked">
	///     Null-check the result of the left side of the expression. If null. this expression returns
	///     BadObject.Null
	/// </param>
	public BadMemberAccessExpression(
        BadExpression left,
        BadWordToken right,
        BadSourcePosition position,
        bool nullChecked = false) : base(
        false,
        position
    )
    {
        Left = left;
        Right = right;
        NullChecked = nullChecked;
    }

	/// <summary>
	///     Left side of the expression
	/// </summary>
	public BadExpression Left { get; private set; }

	/// <summary>
	///     Right Side of the expression
	/// </summary>
	public BadWordToken Right { get; }

	/// <summary>
	///     Property that indicates if the result of the left side of the expression should be null-checked.
	/// </summary>
	public bool NullChecked { get; }

    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Left.GetDescendantsAndSelf();
    }

    public override void Optimize()
    {
        Left = BadConstantFoldingOptimizer.Optimize(Left);
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();

        if (NullChecked && !left.HasProperty(Right.Text, context.Scope))
        {
            yield return BadObject.Null;
        }
        else
        {
            BadObject ret = left.GetProperty(BadObject.Wrap(Right.Text), context.Scope);

            yield return ret;
        }
    }


    public override string ToString()
    {
        return $"({Left}{(NullChecked ? "?" : "")}.{Right})";
    }
}