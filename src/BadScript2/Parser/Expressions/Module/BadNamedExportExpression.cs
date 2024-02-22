using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Module;

/// <summary>
///     Exports a Named Value from the current execution
/// </summary>
public class BadNamedExportExpression : BadExpression
{
    /// <summary>
    ///     Creates a new Named Export Expression
    /// </summary>
    /// <param name="name">Name of the Export</param>
    /// <param name="expression">The Expression to export</param>
    /// <param name="position">The Source Position of the Expression</param>
    public BadNamedExportExpression(string? name, BadExpression expression, BadSourcePosition position) : base(false, position)
    {
        Expression = expression;
        Name = name;
    }

    /// <summary>
    ///     The Expression to export
    /// </summary>
    public BadExpression Expression { get; private set; }

    /// <summary>
    ///     The Name of the Export
    /// </summary>
    public string? Name { get; }

    /// <inheritdoc />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Expression.GetDescendantsAndSelf();
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        if (string.IsNullOrEmpty(Name))
        {
            throw BadRuntimeException.Create(context.Scope, "Exported objects must have a name", Position);
        }

        BadObject? result = BadObject.Null;
        foreach (BadObject o in Expression.Execute(context))
        {
            result = o;

            yield return o;
        }

        result = result.Dereference();

        context.Scope.AddExport(Name!, result);

        yield return result;
    }

    /// <inheritdoc />
    public override void Optimize()
    {
        Expression = BadConstantFoldingOptimizer.Optimize(Expression);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"export {Expression}";
    }
}