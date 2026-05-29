using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Structured representation of a field definition used by compiled class templates.
/// </summary>
public sealed class BadCompiledFieldTemplate
{
    public BadCompiledFieldTemplate(BadExpression expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// Original field expression.
    /// </summary>
    public BadExpression Expression { get; }

    /// <summary>
    /// Executes the field expression in the provided context.
    /// </summary>
    public IEnumerable<BadObject> Execute(BadExecutionContext context)
    {
        return Expression.Execute(context);
    }
}


