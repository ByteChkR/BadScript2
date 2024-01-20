using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic;
/// <summary>
/// Contains the Logic Operators for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Operators.Binary.Logic;

/// <summary>
///     Implements the Logic And Operator
/// </summary>
public class BadLogicAndOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadLogicAndOperator() : base(14, "&&") { }

	/// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicAndExpression(left, right, left.Position.Combine(right.Position));
    }
}