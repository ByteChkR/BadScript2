using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.ControlFlow;

/// <summary>
///     Implements the Break Expression that is used to prematurely exit a loop
/// </summary>
public class BadBreakExpression : BadExpression
{
    /// <summary>
    ///     Constructor of the Break Expression
    /// </summary>
    /// <param name="position">Source Position of the Expression</param>
    public BadBreakExpression(BadSourcePosition position) : base(false, position) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        context.Scope.SetBreak();

        yield return BadObject.Null;
    }
}