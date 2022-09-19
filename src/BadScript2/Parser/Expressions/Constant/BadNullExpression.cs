using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant;

/// <summary>
/// Implements the Null Expression
/// </summary>
public class BadNullExpression : BadExpression, IBadNativeExpression
{
    /// <summary>
    /// Constructor for the null expression
    /// </summary>
    /// <param name="position">Source Position of the Expression</param>
    public BadNullExpression(BadSourcePosition position) : base(true, position) { }

    /// <summary>
    /// The Raw Value of the Expression
    /// </summary>
    public object Value => null!;

    /// <summary>
    /// String Representation of the Expression
    /// </summary>
    /// <returns>String Representation of the Expression</returns>
    public override string ToString()
    {
        return BadStaticKeys.Null;
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        yield return BadObject.Null;
    }
}