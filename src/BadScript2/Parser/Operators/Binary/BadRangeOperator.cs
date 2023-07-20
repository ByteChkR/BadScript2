using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Reader;

namespace BadScript2.Parser.Operators.Binary;

public class BadBinaryUnpackOperator : BadBinaryOperator
{
	public BadBinaryUnpackOperator() : base(3, "...") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		parser.Reader.SkipNonToken();
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadBinaryUnpackExpression(left, right, left.Position.Combine(right.Position));
	}
}

public class BadUnaryUnpackOperator : BadUnaryPrefixOperator
{
	public BadUnaryUnpackOperator() : base(20, "...") { }

	public override BadExpression Parse(BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadUnaryUnpackExpression(right);
	}
}

/// <summary>
///     Implements the Range Operator
/// </summary>
public class BadRangeOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadRangeOperator() : base(15, "..") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadRangeExpression(left, right, left.Position.Combine(right.Position));
	}
}
