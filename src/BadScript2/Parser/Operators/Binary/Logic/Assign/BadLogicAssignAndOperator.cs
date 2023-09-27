using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Parser.Operators.Binary.Logic.Assign;

/// <summary>
///     Implements the Logic And Assign Operator
/// </summary>
public class BadLogicAssignAndOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadLogicAssignAndOperator() : base(15, "&=") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicAssignAndExpression(left, right, left.Position.Combine(right.Position));
    }
}