using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic;
namespace BadScript2.Parser.Operators.Binary.Logic;

/// <summary>
///     Implements the Logic Exclusive Or Operator
/// </summary>
public class BadLogicXOrOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadLogicXOrOperator() : base(12, "^") { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicXOrExpression(left, right, left.Position.Combine(right.Position));
    }
}