using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

/// <summary>
///     Base class for all Unary Prefix Operators
/// </summary>
public abstract class BadUnaryPrefixOperator : BadOperator
{
    /// <summary>
    ///     Constructor for the Unary Prefix Operator
    /// </summary>
    /// <param name="precedence">The Operator precedence</param>
    /// <param name="symbol">The Operator Symbol</param>
    protected BadUnaryPrefixOperator(int precedence, string symbol) : base(precedence, symbol) { }

    /// <summary>
    ///     Parses the Operator and returns the resulting Expression
    /// </summary>
    /// <param name="parser">The Parser Instance</param>
    /// <returns>The Resulting Expression</returns>
    public abstract BadExpression Parse(BadSourceParser parser);
}
