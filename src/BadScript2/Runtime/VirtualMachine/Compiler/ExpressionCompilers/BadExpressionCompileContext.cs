using BadScript2.Common;
using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
/// Compilation Context. Allows to compile <see cref="BadExpression" />s into <see cref="BadInstruction" />s.
/// </summary>
public readonly struct BadExpressionCompileContext
{
    /// <summary>
    /// The Compiler Implementation
    /// </summary>
    public BadCompiler Compiler { get; }

    /// <summary>
    /// The List of already compiled <see cref="BadInstruction" />s.
    /// </summary>
    private readonly List<BadInstruction> m_Instructions = new List<BadInstruction>();

    /// <summary>
    /// Count of <see cref="BadInstruction" />s.
    /// </summary>
    public int InstructionCount => m_Instructions.Count;

    /// <summary>
    /// Index lists of instructions that are empty and need to be resolved.
    /// </summary>
    private readonly List<int> m_EmptyInstructions = new List<int>();

    /// <summary>
    /// Returns the list of compiled <see cref="BadInstruction" />s.
    /// </summary>
    /// <returns>List of <see cref="BadInstruction" />s.</returns>
    /// <exception cref="BadCompilerException">Gets raised when there are unresolved empty instructions.</exception>
    public BadInstruction[] GetInstructions()
    {
        if (m_EmptyInstructions.Count > 0)
        {
            throw new BadCompilerException("Unresolved Empty Instructions!");
        }

        return m_Instructions.ToArray();
    }

    /// <summary>
    /// Creates a new <see cref="BadExpressionCompileContext" /> instance.
    /// </summary>
    /// <param name="compiler"></param>
    public BadExpressionCompileContext(BadCompiler compiler)
    {
        Compiler = compiler;
    }

    /// <summary>
    /// Compiles the given <see cref="BadExpression" /> into a set of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="expr">The <see cref="BadExpression" /> to compile.</param>
    public void Compile(BadExpression expr)
    {
        Compiler.Compile(this, expr);
    }

    /// <summary>
    /// Compiles the given <see cref="BadExpression" />s into a set of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="exprs">The <see cref="BadExpression" />s to compile.</param>
    /// <param name="clearStack">Indicates if the stack should be cleared after compilation.(i.e. the remaining values should be popped from the stack)</param>
    public void Compile(IEnumerable<BadExpression> exprs, bool clearStack = true)
    {
        Compiler.Compile(this, exprs, clearStack);
    }

    /// <summary>
    /// Emits a <see cref="BadInstruction" />.
    /// </summary>
    /// <param name="instruction">The <see cref="BadInstruction" /> to emit.</param>
    public void Emit(BadInstruction instruction)
    {
        m_Instructions.Add(instruction);
    }

    /// <summary>
    /// Emits an empty <see cref="BadInstruction" />. This is used to create a placeholder for an instruction that will be resolved later.
    /// </summary>
    /// <returns>The index of the empty instruction.</returns>
    public int EmitEmpty()
    {
        int i = m_Instructions.Count;
        m_EmptyInstructions.Add(i);
        m_Instructions.Add(new BadInstruction());

        return i;
    }

    /// <summary>
    /// Resolves an empty <see cref="BadInstruction" />. This is used to replace a placeholder with the actual instruction.
    /// </summary>
    /// <param name="index">The Index of the empty instruction.</param>
    /// <param name="code">The OpCode of the instruction.</param>
    /// <param name="position">The Source Position of the instruction.</param>
    /// <param name="args">The Instruction Arguments.</param>
    /// <exception cref="BadCompilerException">Gets raised when the index is invalid.</exception>
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

    /// <summary>
    /// Emits a range of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="instructions">The <see cref="BadInstruction" />s to emit.</param>
    public void EmitRange(IEnumerable<BadInstruction> instructions)
    {
        m_Instructions.AddRange(instructions);
    }

    /// <summary>
    /// Emits a <see cref="BadInstruction" /> with the given <see cref="BadOpCode" /> and <see cref="BadSourcePosition" />.
    /// </summary>
    /// <param name="code">The OpCode of the instruction.</param>
    /// <param name="position">The Source Position of the instruction.</param>
    /// <param name="args">The Instruction Arguments.</param>
    public void Emit(BadOpCode code, BadSourcePosition position, params object[] args)
    {
        Emit(new BadInstruction(code, position, args));
    }
}