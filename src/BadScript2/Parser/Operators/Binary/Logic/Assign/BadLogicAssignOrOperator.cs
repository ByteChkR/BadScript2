using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Parser.Operators.Binary.Logic.Assign;

/// <summary>
///     Implements the Logic Or Assign Operator
/// </summary>
public class BadLogicAssignOrOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadLogicAssignOrOperator() : base(16, "|=", false) { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadLogicAssignOrExpression(left, right, left.Position.Combine(right.Position));
	}
}
