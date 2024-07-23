using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Reader;
using BadScript2.Reader.Token;
namespace BadScript2.Parser.Operators.Binary;

/// <summary>
///     Implements the Member Access Operator
/// </summary>
public class BadMemberAccessOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadMemberAccessOperator() : base(2, ".") { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadWordToken right = parser.Reader.ParseWord();

        List<BadExpression>? args = parser.ParseGenericArguments();

        return new BadMemberAccessExpression(left, right, left.Position.Combine(right.SourcePosition), args);
    }
}