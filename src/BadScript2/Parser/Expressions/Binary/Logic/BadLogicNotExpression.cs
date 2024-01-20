using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Logic;

/// <summary>
///     Implements the Logic Not Exression
/// </summary>
public class BadLogicNotExpression : BadExpression
{
	/// <summary>
	///     Constructor for the Logic Not Expression
	/// </summary>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">Source position of the Expression</param>
	public BadLogicNotExpression(BadExpression right, BadSourcePosition position) : base(
        right.IsConstant,
        position
    )
    {
        Right = right;
    }

	/// <summary>
	///     Right side of the Expression
	/// </summary>
	public BadExpression Right { get; }

    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression? expression in Right.GetDescendantsAndSelf())
        {
            yield return expression;
        }
    }

    /// <summary>
    ///     Returns true if the Input is false
    ///     Returns false if the Input is true
    /// </summary>
    /// <param name="left">The Input of the Expression</param>
    /// <param name="pos">Source Position that is used to generate an error if left is not inheriting IBadBoolean</param>
    /// <returns>The negation of Left</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if left is not an IBadBoolean</exception>
    public static BadObject Not(BadObject left, BadSourcePosition pos)
    {
        if (left is IBadBoolean rBool)
        {
            return rBool.Value ? BadObject.False : BadObject.True;
        }

        throw new BadRuntimeException(
            $"Cannot apply '!' to object '{left}'",
            pos
        );
    }

    public static IEnumerable<BadObject> NotWithOverride(
        BadExecutionContext? context,
        BadObject left,
        BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.NotOperatorName, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(left, context!, BadStaticKeys.NotOperatorName, position))
            {
                yield return o;
            }
        }
        else
        {
            yield return Not(left, position);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject r = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            r = o;
        }

        if (context?.Scope.IsError ?? false)
        {
            yield break;
        }

        r = r.Dereference();

        foreach (BadObject? o in NotWithOverride(context!, r, Position))
        {
            yield return o;
        }
    }

    public override string ToString()
    {
        return $"!{Right}";
    }
}