using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant;

/// <summary>
///     Implements the String Expression
/// </summary>
public class BadStringExpression : BadConstantExpression<string>
{
    /// <summary>
    ///     Constructor of the String Expression
    /// </summary>
    /// <param name="value">The String Value of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadStringExpression(string value, BadSourcePosition position) : base(value, position) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        yield return BadObject.Wrap(Value.Substring(1, Value.Length - 2));
    }
}