using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Parser.Operators.Binary.Logic;

/// <summary>
///     Implements the Logic Not Operator
/// </summary>
public class BadLogicNotOperator : BadUnaryPrefixOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    public BadLogicNotOperator() : base(3, "!") { }

    public override BadExpression Parse(BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicNotExpression(right, right.Position);
    }
}