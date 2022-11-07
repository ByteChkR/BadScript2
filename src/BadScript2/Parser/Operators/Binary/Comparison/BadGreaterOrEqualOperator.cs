using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Parser.Operators.Binary.Comparison;

/// <summary>
///     Implements the Greater or Equal Operator
/// </summary>
public class BadGreaterOrEqualOperator : BadBinaryOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    public BadGreaterOrEqualOperator() : base(8, ">=") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadGreaterOrEqualExpression(left, right, left.Position.Combine(right.Position));
    }
}