using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Assign;
/// <summary>
/// Contains the Self-Assignung Math Operators for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Operators.Binary.Math.Assign;

/// <summary>
///     Implements the Add Assign Operator
/// </summary>
public class BadAddAssignOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadAddAssignOperator() : base(16, "+=", false) { }

	/// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadAddAssignExpression(left, right, left.Position.Combine(right.Position));
    }
}