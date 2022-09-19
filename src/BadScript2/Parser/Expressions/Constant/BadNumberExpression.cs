using BadScript2.Common;

namespace BadScript2.Parser.Expressions.Constant;

/// <summary>
/// Implements the Number Expression
/// </summary>
public class BadNumberExpression : BadConstantExpression<decimal>
{
    /// <summary>
    /// Constructor of the Number Expression
    /// </summary>
    /// <param name="value">Number Value of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadNumberExpression(decimal value, BadSourcePosition position) : base(value, position) { }
}