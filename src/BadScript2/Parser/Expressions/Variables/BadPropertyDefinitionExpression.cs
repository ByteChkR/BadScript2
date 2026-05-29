using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.VirtualMachine.Compiler;

namespace BadScript2.Parser.Expressions.Variables;

/// <summary>
/// Implements a Property Definition Expression
/// </summary>
public class BadPropertyDefinitionExpression : BadExpression
{
    /// <summary>
    /// Constructor of the Property Definition Expression
    /// </summary>
    /// <param name="name">The Name of the Property</param>
    /// <param name="position">The Source Position of the Expression</param>
    /// <param name="getExpression">The Get Expression</param>
    /// <param name="typeExpression">The (optional) Type of the Property</param>
    /// <param name="setExpression">The optional Set Expression</param>
    /// <param name="isReadOnly">Indicates if the Property will be declared as Read-Only</param>
    /// <param name="getCompileLevel">Compile level hint for the getter accessor</param>
    /// <param name="setCompileLevel">Compile level hint for the setter accessor</param>
    public BadPropertyDefinitionExpression(BadWordToken name,
        BadSourcePosition position,
        BadExpression getExpression,
        BadExpression? typeExpression = null,
        BadExpression? setExpression = null,
        bool isReadOnly = false,
        BadFunctionCompileLevel getCompileLevel = BadFunctionCompileLevel.None,
        BadFunctionCompileLevel setCompileLevel = BadFunctionCompileLevel.None) : base(false, position)
    {
        Name = name;
        IsReadOnly = isReadOnly;
        TypeExpression = typeExpression;
        GetExpression = getExpression;
        SetExpression = setExpression;
        GetCompileLevel = getCompileLevel;
        SetCompileLevel = setCompileLevel;
    }

    /// <summary>
    /// The Name of the Property
    /// </summary>
    public BadWordToken Name { get; }

    /// <summary>
    /// Indicates if the Property will be declared as Read-Only
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// The (optional) Type of the Property
    /// </summary>
    public BadExpression? TypeExpression { get; }

    /// <summary>
    /// The Get Expression
    /// </summary>
    public BadExpression GetExpression { get; }

    /// <summary>
    /// Compile level hint for the getter accessor.
    /// </summary>
    public BadFunctionCompileLevel GetCompileLevel { get; }

    /// <summary>
    /// The optional Set Expression
    /// </summary>
    public BadExpression? SetExpression { get; }

    /// <summary>
    /// Compile level hint for the setter accessor.
    /// </summary>
    public BadFunctionCompileLevel SetCompileLevel { get; }

    /// <inheritdoc />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        yield return GetExpression;

        if (TypeExpression != null)
        {
            yield return TypeExpression;
        }

        if (SetExpression != null)
        {
            yield return SetExpression;
        }
    }

    /// <summary>
    /// Executes this property definition in the current context and returns the resulting execution stream.
    /// </summary>
    public IEnumerable<BadObject> ExecuteAsPropertyDefinition(BadExecutionContext context,
                                                              BadCompiledPropertyTemplate? template = null)
    {
        BadCompiledPropertyTemplate effectiveTemplate = template ?? new BadCompiledPropertyTemplate(this);

        foreach (BadObject o in effectiveTemplate.Define(context))
        {
            yield return o;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        foreach (BadObject o in ExecuteAsPropertyDefinition(context))
        {
            yield return o;
        }
    }
}