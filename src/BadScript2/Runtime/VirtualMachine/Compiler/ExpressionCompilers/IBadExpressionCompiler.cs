using BadScript2.Common;
using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

public readonly struct BadExpressionCompileContext
{
    public BadCompiler Compiler { get; }

    private readonly List<BadInstruction> m_Instructions = new List<BadInstruction>();

    public int InstructionCount => m_Instructions.Count;

    private readonly List<int> m_EmptyInstructions = new List<int>();

    public BadInstruction[] GetInstructions()
    {
        if (m_EmptyInstructions.Count > 0)
        {
            throw new BadCompilerException("Unresolved Empty Instructions!");
        }

        return m_Instructions.ToArray();
    }

    public BadExpressionCompileContext(BadCompiler compiler)
    {
        Compiler = compiler;
    }

    public void Compile(BadExpression expr)
    {
        Compiler.Compile(this, expr);
    }

    public void Compile(IEnumerable<BadExpression> exprs, bool clearStack = true)
    {
        Compiler.Compile(this, exprs, clearStack);
    }

    public void Emit(BadInstruction instruction)
    {
        m_Instructions.Add(instruction);
    }

    public int EmitEmpty()
    {
        int i = m_Instructions.Count;
        m_EmptyInstructions.Add(i);
        m_Instructions.Add(new BadInstruction());

        return i;
    }

    public void ResolveEmpty(int index, BadOpCode code, BadSourcePosition position, params object[] args)
    {
        if (m_EmptyInstructions.Contains(index))
        {
            m_Instructions[index] = new BadInstruction(code, position, args);
            m_EmptyInstructions.Remove(index);
        }
        else
        {
            throw new BadCompilerException("Invalid Empty Instruction Index!");
        }
    }

    public void EmitRange(IEnumerable<BadInstruction> instructions)
    {
        m_Instructions.AddRange(instructions);
    }

    public void Emit(BadOpCode code, BadSourcePosition position, params object[] args)
    {
        Emit(new BadInstruction(code, position, args));
    }
}

/// <summary>
///     Defines a Compiler for a specific <see cref="BadExpression" />.
/// </summary>
public interface IBadExpressionCompiler
{
    /// <summary>
    ///     Compiles the given <see cref="BadExpression" /> into a set of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="context">The Context of the Compilation</param>
    /// <param name="expression">The <see cref="BadExpression" /> to compile.</param>
    /// <returns>List of <see cref="BadInstruction" />s.</returns>
    void Compile(BadExpressionCompileContext context, BadExpression expression);
}