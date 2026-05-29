using BadScript2.Parser.Expressions.Types;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Template for creating interface prototypes in VM execution without Eval fallback.
/// </summary>
public sealed class BadCompiledInterfaceTemplate
{
    private readonly BadInterfacePrototypeExpression m_Expression;

    public BadCompiledInterfaceTemplate(BadInterfacePrototypeExpression expression)
    {
        m_Expression = expression;
    }

    public IEnumerable<BadObject> Instantiate(BadExecutionContext context)
    {
        return m_Expression.ExecuteAsInterfaceDefinition(context);
    }
}

