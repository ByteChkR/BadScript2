using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

/// <summary>
///     Base class for all binary operators
/// </summary>
public abstract class BadBinaryOperator : BadOperator
{
    /// <summary>
    ///     Constructor for a binary operator
    /// </summary>
    /// <param name="precedence">The operator Precedence</param>
    /// <param name="symbol">The Operator Symbol</param>
    /// <param name="isLeftAssociative">Set to true if the Operator needs to be evaluated left to right</param>
    protected BadBinaryOperator(int precedence, string symbol, bool isLeftAssociative = true) : base(precedence,
		symbol,
		isLeftAssociative) { }

    /// <summary>
    ///     Parses the operator
    /// </summary>
    /// <param name="left">Left side of the binary expression</param>
    /// <param name="parser">The parser instance</param>
    /// <returns>The resulting parsed expression</returns>
    public abstract BadExpression Parse(BadExpression left, BadSourceParser parser);
}
