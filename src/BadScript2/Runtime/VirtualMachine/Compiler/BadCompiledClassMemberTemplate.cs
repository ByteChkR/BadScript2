using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Structured description of a class member used by compiled class templates.
/// </summary>
public sealed class BadCompiledClassMemberTemplate
{
    public BadCompiledClassMemberTemplate(BadExpression expression, bool isStatic)
    {
        Expression = expression;
        IsStatic = isStatic;
        Name = GetMemberName(expression);
        Kind = GetMemberKind(expression);
        Method = expression is BadFunctionExpression functionExpression
                     ? new BadCompiledMethodTemplate(functionExpression, isStatic)
                     : null;
        Property = expression is BadPropertyDefinitionExpression propertyExpression
                       ? new BadCompiledPropertyTemplate(propertyExpression)
                       : null;
        Field = expression is BadVariableDefinitionExpression fieldExpression
                    ? new BadCompiledFieldTemplate(fieldExpression)
                    : null;
    }

    /// <summary>
    /// Original expression that defines the member.
    /// </summary>
    public BadExpression Expression { get; }

    /// <summary>
    /// Member name if available.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Semantic member kind.
    /// </summary>
    public BadCompiledClassMemberKind Kind { get; }

    /// <summary>
    /// Indicates whether the member belongs to the static class body.
    /// </summary>
    public bool IsStatic { get; }

    /// <summary>
    /// Structured method template if this member represents a method.
    /// </summary>
    public BadCompiledMethodTemplate? Method { get; }

    /// <summary>
    /// Structured property template if this member represents a property.
    /// </summary>
    public BadCompiledPropertyTemplate? Property { get; }

    /// <summary>
    /// Structured field template if this member represents a field.
    /// </summary>
    public BadCompiledFieldTemplate? Field { get; }

    /// <summary>
    /// Executes the original member expression.
    /// </summary>
    public IEnumerable<BadObject> Execute(BadExecutionContext context)
    {
        if (Property != null)
        {
            return Property.Define(context);
        }

        if (Method != null)
        {
            return Method.Instantiate(context);
        }

        if (Field != null)
        {
            return Field.Execute(context);
        }

        return Expression.Execute(context);
    }

    private static string? GetMemberName(BadExpression expression)
    {
        return expression switch
        {
            BadPropertyDefinitionExpression property => property.Name.Text,
            IBadNamedExpression named => named.GetName(),
            _ => null,
        };
    }

    private static BadCompiledClassMemberKind GetMemberKind(BadExpression expression)
    {
        return expression switch
        {
            BadPropertyDefinitionExpression => BadCompiledClassMemberKind.Property,
            BadVariableDefinitionExpression => BadCompiledClassMemberKind.Field,
            BadFunctionExpression { Name: { Text: BadStaticKeys.CONSTRUCTOR_NAME } } => BadCompiledClassMemberKind.Constructor,
            BadFunctionExpression => BadCompiledClassMemberKind.Method,
            _ => BadCompiledClassMemberKind.Unknown,
        };
    }
}

