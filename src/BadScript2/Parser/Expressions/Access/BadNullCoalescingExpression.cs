using BadScript2.Common;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Access;

/// <summary>
///     Implements the Null Coalescing Expression
///     LEFT ?? RIGHT
/// </summary>
public class BadNullCoalescingExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Null Coalescing Expression
    /// </summary>
    /// <param name="left">Left side of the expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Position inside the source code</param>
    public BadNullCoalescingExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
                                                                                                                   right,
                                                                                                                   position
                                                                                                                  ) { }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;
        }

        left = left.Dereference(Position);

        if (left == BadObject.Null)
        {
            foreach (BadObject o in Right.Execute(context))
            {
                yield return o;
            }
        }
        else
        {
            yield return left;
        }
    }

    /// <inheritdoc />
    protected override string GetSymbol()
    {
        return "??";
    }
}