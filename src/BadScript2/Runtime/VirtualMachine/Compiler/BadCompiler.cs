using System.Collections.Generic;
using System.Linq;
using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.Parser.Expressions.Binary.Logic.Assign;
using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Module;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Function;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Module;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <summary>
/// Contains the Compiler for the BadVirtualMachine.
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
///     Implements the Compile for the BadVirtualMachine.
/// </summary>
public class BadCompiler
{
    /// <summary>
    ///     The Default Compiler Instance.
    /// </summary>
    public static readonly BadCompiler Instance = new BadCompiler();

    /// <summary>
    ///     Indicates whether or not the Compiler should allow Eval Instructions.
    /// </summary>
    public readonly bool AllowEval;
    
    public void AddCompiler(Type t, IBadExpressionCompiler compiler) =>
        m_Compilers[t] = compiler;
    public void AddCompiler<T>(BadExpressionCompiler<T> compiler) where T : BadExpression =>
        m_Compilers[typeof(T)] = compiler;
    public void AddCompiler<T, TE>() where T : BadExpressionCompiler<TE>, new()
        where TE : BadExpression
    {
        AddCompiler(new T());
    }

    /// <summary>
    ///     The Dictionary of Compilers for the different <see cref="BadExpression" /> types.
    /// </summary>
    private readonly Dictionary<Type, IBadExpressionCompiler> m_Compilers = new Dictionary<Type, IBadExpressionCompiler>
    {
        { typeof(BadVariableExpression), new BadVariableExpressionCompiler() },
        { typeof(BadVariableDefinitionExpression), new BadVariableDefinitionExpressionCompiler() },
        { typeof(BadPropertyDefinitionExpression), new BadPropertyDefinitionExpressionCompiler() },
        { typeof(BadMemberAccessExpression), new BadMemberAccessExpressionCompiler() },
        { typeof(BadEqualityExpression), new BadEqualityExpressionCompiler() },
        { typeof(BadInequalityExpression), new BadInequalityExpressionCompiler() },
        { typeof(BadLessThanExpression), new BadLessExpressionCompiler() },
        { typeof(BadLessOrEqualExpression), new BadLessOrEqualExpressionCompiler() },
        { typeof(BadGreaterOrEqualExpression), new BadGreaterOrEqualExpressionCompiler() },
        { typeof(BadGreaterThanExpression), new BadGreaterExpressionCompiler() },
        { typeof(BadLogicAndExpression), new BadLogicAndExpressionCompiler() },
        { typeof(BadLogicOrExpression), new BadLogicOrExpressionCompiler() },
        { typeof(BadLogicXOrExpression), new BadLogicXOrExpressionCompiler() },
        { typeof(BadLogicNotExpression), new BadLogicNotExpressionCompiler() },
        { typeof(BadLogicAssignAndExpression), new BadLogicAssignAndExpressionCompiler() },
        { typeof(BadLogicAssignOrExpression), new BadLogicAssignOrExpressionCompiler() },
        { typeof(BadLogicAssignXOrExpression), new BadLogicAssignXOrExpressionCompiler() },
        { typeof(BadAddExpression), new BadAddExpressionCompiler() },
        { typeof(BadSubtractExpression), new BadSubtractExpressionCompiler() },
        { typeof(BadMultiplyExpression), new BadMultiplyExpressionCompiler() },
        { typeof(BadExponentiationExpression), new BadExponentiationExpressionCompiler() },
        { typeof(BadNegationExpression), new BadNegateExpressionCompiler() },
        { typeof(BadDeleteExpression), new BadDeleteExpressionCompiler() },
        { typeof(BadInstanceOfExpression), new BadInstanceOfExpressionCompiler() },
        { typeof(BadTypeOfExpression), new BadTypeOfExpressionCompiler() },
        { typeof(BadDivideExpression), new BadDivideExpressionCompiler() },
        { typeof(BadModulusExpression), new BadModulusExpressionCompiler() },
        { typeof(BadAddAssignExpression), new BadAddAssignExpressionCompiler() },
        { typeof(BadSubtractAssignExpression), new BadSubtractAssignExpressionCompiler() },
        { typeof(BadExponentiationAssignExpression), new BadExponentiationAssignExpressionCompiler() },
        { typeof(BadMultiplyAssignExpression), new BadMultiplyAssignExpressionCompiler() },
        { typeof(BadDivideAssignExpression), new BadDivideAssignExpressionCompiler() },
        { typeof(BadModulusAssignExpression), new BadModulusAssignExpressionCompiler() },
        { typeof(BadPostDecrementExpression), new BadPostDecrementExpressionCompiler() },
        { typeof(BadPostIncrementExpression), new BadPostIncrementExpressionCompiler() },
        { typeof(BadPreDecrementExpression), new BadPreDecrementExpressionCompiler() },
        { typeof(BadPreIncrementExpression), new BadPreIncrementExpressionCompiler() },
        { typeof(BadAssignExpression), new BadAssignExpressionCompiler() },
        { typeof(BadBinaryUnpackExpression), new BadBinaryUnpackExpressionCompiler() },
        { typeof(BadUnaryUnpackExpression), new BadUnaryUnpackExpressionCompiler() },
        { typeof(BadBooleanExpression), new BadBooleanExpressionCompiler() },
        { typeof(BadNumberExpression), new BadNumberExpressionCompiler() },
        { typeof(BadStringExpression), new BadStringExpressionCompiler() },
        { typeof(BadNullExpression), new BadNullExpressionCompiler() },
        { typeof(BadIfExpression), new BadIfExpressionCompiler() },
        { typeof(BadSwitchExpression), new BadSwitchExpressionCompiler() },
        { typeof(BadReturnExpression), new BadReturnExpressionCompiler() },
        { typeof(BadContinueExpression), new BadContinueExpressionCompiler() },
        { typeof(BadBreakExpression), new BadBreakExpressionCompiler() },
        { typeof(BadThrowExpression), new BadThrowExpressionCompiler() },
        { typeof(BadWhileExpression), new BadWhileExpressionCompiler() },
        { typeof(BadForExpression), new BadForExpressionCompiler() },
        { typeof(BadFormattedStringExpression), new BadFormattedStringExpressionCompiler() },
        { typeof(BadConstantExpression), new BadConstantExpressionCompiler() },
        { typeof(BadArrayAccessExpression), new BadArrayAccessExpressionCompiler() },
        { typeof(BadArrayAccessReverseExpression), new BadArrayAccessReverseExpressionCompiler() },
        { typeof(BadTernaryExpression), new BadTernaryExpressionCompiler() },
        { typeof(BadNullCoalescingExpression), new BadNullCoalescingExpressionCompiler() },
        { typeof(BadNullCoalescingAssignExpression), new BadNullCoalescingAssignExpressionCompiler() },
        { typeof(BadLockExpression), new BadLockExpressionCompiler() },
        { typeof(BadArrayExpression), new BadArrayExpressionCompiler() },
        { typeof(BadTableExpression), new BadTableExpressionCompiler() },
        { typeof(BadInvocationExpression), new BadInvocationExpressionCompiler() },
        { typeof(BadForEachExpression), new BadForEachExpressionCompiler() },
        { typeof(BadNewExpression), new BadNewExpressionCompiler() },
        { typeof(BadRangeExpression), new BadRangeExpressionCompiler() },
        { typeof(BadTryCatchExpression), new BadTryCatchExpressionCompiler() },
        { typeof(BadInExpression), new BadInExpressionCompiler() },
        { typeof(BadFunctionExpression), new BadFunctionExpressionCompiler() },
        { typeof(BadClassPrototypeExpression), new BadClassPrototypeExpressionCompiler() },
        { typeof(BadInterfacePrototypeExpression), new BadInterfacePrototypeExpressionCompiler() },
        { typeof(BadUsingExpression), new BadUsingExpressionCompiler() },
        { typeof(BadUsingStatementExpression), new BadUsingStatementExpressionCompiler() },
        { typeof(BadNamedExportExpression), new BadNamedExportExpressionCompiler() },
        { typeof(BadDefaultExportExpression), new BadDefaultExportExpressionCompiler() },
        { typeof(BadImportExpression), new BadImportExpressionCompiler() },
    };

    /// <summary>
    ///     Creates a new BadCompiler instance.
    /// </summary>
    /// <param name="allowEval">Indicates whether or not the Compiler should allow Eval Instructions.</param>
    public BadCompiler(bool allowEval = false)
    {
        AllowEval = allowEval;
    }

    /// <summary>
    ///     Compiles the given <see cref="BadExpression" /> into a set of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="expression">The <see cref="BadExpression" /> to compile.</param>
    /// <returns>List of <see cref="BadInstruction" />s.</returns>
    /// <exception cref="BadCompilerException">
    ///     If no Compiler for the given <see cref="BadExpression" /> type exists and
    ///     AllowEval is set to false.
    /// </exception>
    public void Compile(BadExpressionCompileContext context, BadExpression expression)
    {
        Type t = expression.GetType();

        if (m_Compilers.TryGetValue(t, out IBadExpressionCompiler compiler))
        {
            compiler.Compile(context, expression);
        }
        else if (!AllowEval)
        {
            throw new BadCompilerException("No Compiler for Expression Type " +
                                           expression.GetType()
                                                     .Name
                                          );
        }
        else
        {
            context.Emit(BadOpCode.Eval, expression.Position, expression);
        }
    }

    /// <summary>
    ///     Compiles the given <see cref="BadExpression" />s into a set of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="expressions">The <see cref="BadExpression" />s to compile.</param>
    /// <param name="clearStack">Indicates whether or not the Stack should be cleared after each expression.</param>
    /// <returns>List of <see cref="BadInstruction" />s.</returns>
    public void Compile(BadExpressionCompileContext context,
                        IEnumerable<BadExpression> expressions,
                        bool clearStack = true)
    {
        foreach (BadExpression expression in expressions)
        {
            BadSourcePosition position = expression.Position;

            Compile(context, expression);

            if (clearStack)
            {
                context.Emit(BadOpCode.ClearStack, position);
            }
        }
    }

    /// <summary>
    ///     Compiles the given source into a set of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="src">The source to compile.</param>
    /// <returns>List of <see cref="BadInstruction" />s.</returns>
    public static IEnumerable<BadInstruction> Compile(string src)
    {
        BadSourceParser parser = BadSourceParser.Create("<nofile>", src);

        return Compile(parser.Parse());
    }

    /// <summary>
    /// Compiles the given <see cref="BadExpression" />s into a set of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="expressions">The <see cref="BadExpression" />s to compile.</param>
    /// <returns>The list of <see cref="BadInstruction" />s.</returns>
    public static IEnumerable<BadInstruction> Compile(IEnumerable<BadExpression> expressions)
    {
        BadExpressionCompileContext ctx = new BadExpressionCompileContext(Instance);
        Instance.Compile(ctx, expressions);
        BadInstruction[] instructions = ctx.GetInstructions().ToArray();
        RunEscapeAnalysis(instructions);

        return instructions;
    }

    // -------------------------------------------------------------------------
    // Phase C2 – Escape Analysis pass
    // -------------------------------------------------------------------------

    /// <summary>
    ///     Opcodes that produce a single numeric/boolean value on the stack
    ///     by consuming two operands (binary arithmetic / comparison).
    /// </summary>
    private static readonly HashSet<BadOpCode> s_ArithmeticOpcodes = new()
    {
        BadOpCode.Add,
        BadOpCode.Sub,
        BadOpCode.Mul,
        BadOpCode.Div,
        BadOpCode.Mod,
        BadOpCode.Exp,
        BadOpCode.Neg,
    };

    /// <summary>
    ///     Opcodes that CONSUME a value from the stack and produce a boolean
    ///     (comparisons). A transient arithmetic result flowing into these is safe.
    /// </summary>
    private static readonly HashSet<BadOpCode> s_ConsumerOpcodes = new()
    {
        BadOpCode.Add,
        BadOpCode.Sub,
        BadOpCode.Mul,
        BadOpCode.Div,
        BadOpCode.Mod,
        BadOpCode.Exp,
        BadOpCode.Neg,
        BadOpCode.Equals,
        BadOpCode.NotEquals,
        BadOpCode.Greater,
        BadOpCode.GreaterEquals,
        BadOpCode.Less,
        BadOpCode.LessEquals,
        BadOpCode.Not,
    };

    /// <summary>
    ///     Scans <paramref name="instructions"/> and marks arithmetic instructions
    ///     whose result is immediately consumed by the next instruction (no branch
    ///     can jump into the consumer from elsewhere) with
    ///     <see cref="BadInstructionFlags.TransientResult"/>.
    ///     The VM can then use a pre-allocated scratch <c>BadNumber</c> instead of
    ///     heap-allocating a new one.
    /// </summary>
    private static void RunEscapeAnalysis(BadInstruction[] instructions)
    {
        if (instructions.Length < 2)
        {
            return;
        }

        // Step 1: collect all branch-target indices so we know which instructions
        //         are reachable via a jump from another location.
        HashSet<int> jumpTargets = new HashSet<int>();

        for (int i = 0; i < instructions.Length; i++)
        {
            BadOpCode op = instructions[i].OpCode;

            if (op is BadOpCode.JumpRelative
                    or BadOpCode.JumpRelativeIfFalse
                    or BadOpCode.JumpRelativeIfTrue
                    or BadOpCode.JumpRelativeIfNotNull
                    or BadOpCode.JumpRelativeIfNull
                    or BadOpCode.SetBreakPointer
                    or BadOpCode.SetContinuePointer
                    or BadOpCode.SetThrowPointer)
            {
                if (instructions[i].Arguments.Length > 0 &&
                    instructions[i].Arguments[0] is int relativeOffset)
                {
                    int target = i + relativeOffset;

                    if (target >= 0 && target < instructions.Length)
                    {
                        jumpTargets.Add(target);
                    }
                }
            }
        }

        // Step 2: for each arithmetic instruction, check whether the NEXT
        //         instruction immediately consumes the result without any branch
        //         being able to enter the consumer from a different path.
        for (int i = 0; i < instructions.Length - 1; i++)
        {
            if (!s_ArithmeticOpcodes.Contains(instructions[i].OpCode))
            {
                continue;
            }

            int nextIdx = i + 1;

            // The consumer must not be a jump target (i.e. no other code path
            // can reach it with a different stack state).
            if (jumpTargets.Contains(nextIdx))
            {
                continue;
            }

            if (s_ConsumerOpcodes.Contains(instructions[nextIdx].OpCode))
            {
                instructions[i].Flags |= BadInstructionFlags.TransientResult;
            }
        }
    }
}
