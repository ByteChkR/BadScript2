using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Module;

/// <summary>
///     Exports the Default Value of the current execution
/// </summary>
public class BadDefaultExportExpression : BadExpression
{
    /// <summary>
    ///     Creates a new Default Export Expression
    /// </summary>
    /// <param name="expression">The Expression to export</param>
    /// <param name="position">The Source Position of the Expression</param>
    public BadDefaultExportExpression(BadExpression expression, BadSourcePosition position) : base(false, position)
    {
        Expression = expression;
    }

    /// <summary>
    ///     The Expression to export
    /// </summary>
    public BadExpression Expression { get; private set; }

    /// <inheritdoc />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Expression.GetDescendantsAndSelf();
    }

    /// <inheritdoc />
    public override void Optimize()
    {
        Expression = BadConstantFoldingOptimizer.Optimize(Expression);
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject? result = BadObject.Null;

        foreach (BadObject o in Expression.Execute(context))
        {
            result = o;

            yield return o;
        }

        result = result.Dereference(Position);

        context.Scope.SetExports(context, result);

        yield return result;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "export default " + Expression;
    }
}