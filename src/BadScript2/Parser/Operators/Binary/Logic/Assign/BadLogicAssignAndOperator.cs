using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic.Assign;

/// <summary>
/// Contains the Self-Assigning Logic Operators for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Operators.Binary.Logic.Assign;

/// <summary>
///     Implements the Logic And Assign Operator
/// </summary>
public class BadLogicAssignAndOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadLogicAssignAndOperator() : base(16, "&=", false) { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicAssignAndExpression(left, right, left.Position.Combine(right.Position));
    }
}