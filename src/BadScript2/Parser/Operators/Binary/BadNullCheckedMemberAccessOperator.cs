using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Reader;
using BadScript2.Reader.Token;

namespace BadScript2.Parser.Operators.Binary;

/// <summary>
///     Implements the Null-Checked Member Access Operator
/// </summary>
public class BadNullCheckedMemberAccessOperator : BadBinaryOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    public BadNullCheckedMemberAccessOperator() : base(2, "?.") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadWordToken right = parser.Reader.ParseWord();

		return new BadMemberAccessExpression(left, right, left.Position.Combine(right.SourcePosition), true);
	}
}
