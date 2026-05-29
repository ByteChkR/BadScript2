using BadScript2.Parser.Expressions.Types;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Template for creating class prototypes in VM execution without Eval fallback.
/// </summary>
public sealed class BadCompiledClassTemplate
{
    private readonly BadClassPrototypeExpression m_Expression;

    public BadCompiledClassTemplate(BadClassPrototypeExpression expression)
    {
        m_Expression = expression;
        InstanceMembers = expression.Body.Select(x => new BadCompiledClassMemberTemplate(x, false))
                                    .ToArray();
        StaticMembers = expression.StaticBody.Select(x => new BadCompiledClassMemberTemplate(x, true))
                                  .ToArray();
    }

    /// <summary>
    /// Structured templates for instance members.
    /// </summary>
    public IReadOnlyList<BadCompiledClassMemberTemplate> InstanceMembers { get; }

    /// <summary>
    /// Structured templates for static members.
    /// </summary>
    public IReadOnlyList<BadCompiledClassMemberTemplate> StaticMembers { get; }

    public IEnumerable<BadObject> Instantiate(BadExecutionContext context)
    {
        return m_Expression.ExecuteAsClassDefinition(context, InstanceMembers, StaticMembers);
    }
}

