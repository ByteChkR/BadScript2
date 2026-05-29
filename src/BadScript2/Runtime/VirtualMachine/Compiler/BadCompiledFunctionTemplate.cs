using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Template for creating function objects in VM execution without Eval fallback.
/// </summary>
public sealed class BadCompiledFunctionTemplate
{
    private readonly BadInstruction[]? m_CompiledInstructions;
    private readonly BadFunctionExpression m_Expression;
    private readonly bool m_RequiresClosureScopeMaterialization;
    private readonly bool? m_UseOverrides;
    private readonly BadSymbolTable? m_SymbolTable;

    public BadCompiledFunctionTemplate(BadFunctionExpression expression,
                                       BadInstruction[]? compiledInstructions,
                                       bool? useOverrides,
                                       BadSymbolTable? symbolTable = null,
                                       bool requiresClosureScopeMaterialization = false)
    {
        m_Expression = expression;
        m_CompiledInstructions = compiledInstructions;
        m_RequiresClosureScopeMaterialization = requiresClosureScopeMaterialization;
        m_UseOverrides = useOverrides;
        m_SymbolTable = symbolTable;
    }

    /// <summary>
    /// Original function expression.
    /// </summary>
    public BadFunctionExpression Expression => m_Expression;

    /// <summary>
    /// Optional precompiled instruction payload.
    /// </summary>
    public IReadOnlyList<BadInstruction>? CompiledInstructions => m_CompiledInstructions;

    /// <summary>
    /// Optional override mode for compiled execution.
    /// </summary>
    public bool? UseOverrides => m_UseOverrides;

    /// <summary>
    /// Symbol table for slot-based local variable access (Phase 5 optimization).
    /// </summary>
    public BadSymbolTable? SymbolTable => m_SymbolTable;

    /// <summary>
    /// Indicates that slot-backed writes must still be mirrored into scope for closures.
    /// </summary>
    public bool RequiresClosureScopeMaterialization => m_RequiresClosureScopeMaterialization;

    public IEnumerable<BadObject> Instantiate(BadExecutionContext context)
    {
        return m_Expression.ExecuteAsFunctionDefinition(context,
                                                        m_CompiledInstructions,
                                                        m_UseOverrides,
                                                        m_SymbolTable,
                                                        m_RequiresClosureScopeMaterialization);
    }
}
