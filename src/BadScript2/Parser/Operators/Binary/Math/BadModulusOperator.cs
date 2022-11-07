using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Parser.Operators.Binary.Math;

/// <summary>
///     Implements the Modulus Operator
/// </summary>
public class BadModulusOperator : BadBinaryOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    public BadModulusOperator() : base(5, "%") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadModulusExpression(left, right, left.Position.Combine(right.Position));
    }
}