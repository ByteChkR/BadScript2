using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic;
namespace BadScript2.Parser.Operators.Binary.Logic;

/// <summary>
///     Implements the Logic Or Operator
/// </summary>
public class BadLogicOrOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadLogicOrOperator() : base(15, "||") { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicOrExpression(left, right, left.Position.Combine(right.Position));
    }
}