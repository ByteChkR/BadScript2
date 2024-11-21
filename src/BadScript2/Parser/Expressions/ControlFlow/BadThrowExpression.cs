using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.ControlFlow;

/// <summary>
///     Implements the Throw Expression that is used to raise errors inside the Script
/// </summary>
public class BadThrowExpression : BadExpression
{
    /// <summary>
    ///     Constructor of the Throw Expression
    /// </summary>
    /// <param name="right">The Error Object that is thrown</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadThrowExpression(BadExpression right, BadSourcePosition position) : base(false,
                                                                                      position
                                                                                     )
    {
        Right = right;
    }

    /// <summary>
    ///     The Error Object that is thrown
    /// </summary>
    public BadExpression Right { get; set; }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        Right = BadConstantFoldingOptimizer.Optimize(Right);
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Right.GetDescendantsAndSelf();
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject value = BadObject.Null;

        foreach (BadObject obj in Right.Execute(context))
        {
            value = obj;

            yield return obj;
        }

        value = value.Dereference(Position);

        BadRuntimeError? error = new BadRuntimeError(null, value, context.Scope.GetStackTrace());

        throw new BadRuntimeErrorException(error);
    }
}