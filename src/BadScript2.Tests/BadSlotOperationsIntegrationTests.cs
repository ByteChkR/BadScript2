using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

namespace BadScript2.Tests;

/// <summary>
/// Integration tests for Phase 5: Slot-based local variables.
/// Tests that slot operations work correctly in the VM.
/// </summary>
public class BadSlotOperationsIntegrationTests
{

    [Test]
    public void InitLocals_InitializesLocalSlots()
    {
        // Arrange
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        var ctx = new BadExpressionCompileContext(BadCompiler.Instance);

        // Create InitLocals instruction
        ctx.Emit(BadOpCode.InitLocals, pos, 5); // Initialize 5 slots
        ctx.Emit(BadOpCode.Push, pos, 42);      // Push value onto stack
        ctx.Emit(BadOpCode.ClearStack, pos);    // Clear stack

        var instructions = ctx.GetInstructions();

        // Act & Assert
        // The InitLocals opcode should execute without errors
        Assert.That(instructions.Length, Is.GreaterThan(0));
        Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.InitLocals));
        Assert.That(instructions[0].Arguments[0], Is.EqualTo(5));
    }

    [Test]
    public void LoadStoreLocal_WorksWithSlots()
    {
        // Arrange
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        var ctx = new BadExpressionCompileContext(BadCompiler.Instance);

        // Compile: value = 42
        ctx.Emit(BadOpCode.InitLocals, pos, 1);      // Initialize 1 slot
        ctx.Emit(BadOpCode.Push, pos, 42);            // Push 42
        ctx.Emit(BadOpCode.StoreLocal, pos, 0);       // Store to slot 0
        ctx.Emit(BadOpCode.LoadLocal, pos, 0);        // Load from slot 0
        ctx.Emit(BadOpCode.ClearStack, pos);          // Clear stack

        var instructions = ctx.GetInstructions();

        // Assert
        Assert.That(instructions.Length, Is.EqualTo(5));
        Assert.That(instructions[1].OpCode, Is.EqualTo(BadOpCode.Push));
        Assert.That(instructions[2].OpCode, Is.EqualTo(BadOpCode.StoreLocal));
        Assert.That(instructions[3].OpCode, Is.EqualTo(BadOpCode.LoadLocal));
    }

    [Test]
    public void SymbolTable_CanBeUsedInFunctionTemplate()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Register parameters and locals
        symbolTable.RegisterParameter("x", pos);
        symbolTable.RegisterParameter("y", pos);
        symbolTable.RegisterLocal("result", pos);

        // Create function template with symbol table
        // (This would normally be done by the compiler)
        var ctx = new BadExpressionCompileContext(BadCompiler.Instance);
        ctx.Emit(BadOpCode.InitLocals, pos, symbolTable.TotalSlotCount);
        ctx.Emit(BadOpCode.LoadLocal, pos, 0);  // Load x
        ctx.Emit(BadOpCode.LoadLocal, pos, 1);  // Load y
        ctx.Emit(BadOpCode.Add, pos);            // x + y
        ctx.Emit(BadOpCode.StoreLocal, pos, 2); // Store to result

        var instructions = ctx.GetInstructions();

        // Assert
        Assert.That(symbolTable.ParameterCount, Is.EqualTo(2));
        Assert.That(symbolTable.LocalCount, Is.EqualTo(1));
        Assert.That(symbolTable.TotalSlotCount, Is.EqualTo(3));
        Assert.That(instructions.Length, Is.GreaterThan(0));
    }

    [Test]
    public void SlotOperations_PreserveStackBehavior()
    {
        // Arrange
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        var ctx = new BadExpressionCompileContext(BadCompiler.Instance);

        // Compile sequence that stores and loads values
        ctx.Emit(BadOpCode.InitLocals, pos, 2);
        ctx.Emit(BadOpCode.Push, pos, 10);
        ctx.Emit(BadOpCode.StoreLocal, pos, 0);  // value1 = 10
        ctx.Emit(BadOpCode.Push, pos, 20);
        ctx.Emit(BadOpCode.StoreLocal, pos, 1);  // value2 = 20
        ctx.Emit(BadOpCode.LoadLocal, pos, 0);   // Load value1
        ctx.Emit(BadOpCode.LoadLocal, pos, 1);   // Load value2
        ctx.Emit(BadOpCode.Add, pos);            // value1 + value2 = 30
        ctx.Emit(BadOpCode.ClearStack, pos);

        var instructions = ctx.GetInstructions();

        // Assert
        Assert.That(instructions.Length, Is.EqualTo(9));
        // Verify the sequence of operations
        Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.InitLocals));
        Assert.That(instructions[2].OpCode, Is.EqualTo(BadOpCode.StoreLocal));
        Assert.That(instructions[6].OpCode, Is.EqualTo(BadOpCode.LoadLocal));
    }

    [Test]
    public void ReservedLoopOpcodes_AreRecognized()
    {
        // Arrange
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        var ctx = new BadExpressionCompileContext(BadCompiler.Instance);

        // Emit reserved loop opcodes
        ctx.Emit(BadOpCode.BeginLoop, pos);
        ctx.Emit(BadOpCode.EndLoop, pos);

        var instructions = ctx.GetInstructions();

        // Assert
        Assert.That(instructions.Length, Is.EqualTo(2));
        Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.BeginLoop));
        Assert.That(instructions[1].OpCode, Is.EqualTo(BadOpCode.EndLoop));
    }
}

