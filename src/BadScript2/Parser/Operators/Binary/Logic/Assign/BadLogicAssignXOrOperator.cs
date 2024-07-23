using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic.Assign;
namespace BadScript2.Parser.Operators.Binary.Logic.Assign;

/// <summary>
///     Implements the Logic Exclusive Or Assign Operator
/// </summary>
public class BadLogicAssignXOrOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadLogicAssignXOrOperator() : base(16, "^=", false) { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicAssignXOrExpression(left, right, left.Position.Combine(right.Position));
    }
}