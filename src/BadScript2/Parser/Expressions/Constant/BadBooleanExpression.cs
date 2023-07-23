using BadScript2.Common;

namespace BadScript2.Parser.Expressions.Constant;

/// <summary>
///     Implements the Boolean Expression
/// </summary>
public class BadBooleanExpression : BadConstantExpression<bool>
{
    /// <summary>
    ///     Constructor of the Boolean Expression
    /// </summary>
    /// <param name="value">Boolean Value of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadBooleanExpression(bool value, BadSourcePosition position) : base(value, position) { }
}
