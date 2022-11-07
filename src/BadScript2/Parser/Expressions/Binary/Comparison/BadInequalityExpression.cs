using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary.Comparison;

/// <summary>
///     Implements the Inequality Expression
/// </summary>
public class BadInequalityExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor for the Inequality Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source position of the Expression</param>
    public BadInequalityExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    /// <summary>
    ///     Returns True if the two Objects are not equal
    /// </summary>
    /// <param name="left">Left Object</param>
    /// <param name="right">Right Object</param>
    /// <returns>True if the Objects are not Equal. False Otherwise</returns>
    public static BadObject NotEqual(BadObject left, BadObject right)
    {
        if (!left.Equals(right))
        {
            return BadObject.True;
        }

        return BadObject.False;
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
        BadObject right = BadObject.Null;
        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        yield return NotEqual(left, right);
    }

    protected override string GetSymbol()
    {
        return "!=";
    }
}