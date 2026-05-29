using BadScript2.Common;
using BadScript2.Runtime.VirtualMachine;

namespace BadScript2.Tests;

/// <summary>
/// Tests for Phase 5: Slot-based local variables implementation.
/// </summary>
public class BadSymbolTableTests
{
    [Test]
    public void SymbolTable_CanRegisterParameters()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Act
        var param1 = symbolTable.RegisterParameter("x", pos);
        var param2 = symbolTable.RegisterParameter("y", pos);

        // Assert
        Assert.That(param1.SlotIndex, Is.EqualTo(0));
        Assert.That(param2.SlotIndex, Is.EqualTo(1));
        Assert.That(param1.IsCapture, Is.False);
        Assert.That(symbolTable.ParameterCount, Is.EqualTo(2));
        Assert.That(symbolTable.TotalSlotCount, Is.EqualTo(2));
    }

    [Test]
    public void SymbolTable_CanRegisterLocals()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Act
        symbolTable.RegisterParameter("x", pos);
        var local1 = symbolTable.RegisterLocal("a", pos);
        var local2 = symbolTable.RegisterLocal("b", pos);

        // Assert
        Assert.That(local1.SlotIndex, Is.EqualTo(1));
        Assert.That(local2.SlotIndex, Is.EqualTo(2));
        Assert.That(local1.IsCapture, Is.False);
        Assert.That(symbolTable.LocalCount, Is.EqualTo(2));
        Assert.That(symbolTable.TotalSlotCount, Is.EqualTo(3));
    }

    [Test]
    public void SymbolTable_CanRegisterCaptures()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Act
        symbolTable.RegisterParameter("x", pos);
        symbolTable.RegisterLocal("a", pos);
        var capture1 = symbolTable.RegisterCapture("outer1", pos);
        var capture2 = symbolTable.RegisterCapture("outer2", pos);

        // Assert
        Assert.That(capture1.SlotIndex, Is.EqualTo(2));
        Assert.That(capture2.SlotIndex, Is.EqualTo(3));
        Assert.That(capture1.IsCapture, Is.True);
        Assert.That(symbolTable.CaptureCount, Is.EqualTo(2));
        Assert.That(symbolTable.TotalSlotCount, Is.EqualTo(4));
    }

    [Test]
    public void SymbolTable_CanRetrieveSymbols()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Act
        symbolTable.RegisterParameter("x", pos);
        symbolTable.RegisterLocal("a", pos);
        symbolTable.RegisterCapture("outer", pos);

        // Assert
        Assert.That(symbolTable.TryGetSymbol("x", out var xInfo), Is.True);
        Assert.That(xInfo.Name, Is.EqualTo("x"));
        Assert.That(xInfo.SlotIndex, Is.EqualTo(0));

        Assert.That(symbolTable.TryGetSymbol("a", out var aInfo), Is.True);
        Assert.That(aInfo.Name, Is.EqualTo("a"));
        Assert.That(aInfo.SlotIndex, Is.EqualTo(1));

        Assert.That(symbolTable.TryGetSymbol("outer", out var outerInfo), Is.True);
        Assert.That(outerInfo.Name, Is.EqualTo("outer"));
        Assert.That(outerInfo.IsCapture, Is.True);
    }

    [Test]
    public void SymbolTable_ThrowsOnDuplicateRegistration()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Act & Assert
        symbolTable.RegisterParameter("x", pos);
        Assert.Throws<InvalidOperationException>(() => symbolTable.RegisterParameter("x", pos));
    }

    [Test]
    public void SymbolTable_ThrowsOnUnknownSymbol()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => symbolTable.GetSymbol("unknown"));
    }

    [Test]
    public void SymbolTable_CanEnumerateSymbols()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Act
        symbolTable.RegisterParameter("x", pos);
        symbolTable.RegisterParameter("y", pos);
        symbolTable.RegisterLocal("a", pos);
        symbolTable.RegisterLocal("b", pos);
        symbolTable.RegisterCapture("outer1", pos);

        // Assert
        var allSymbols = symbolTable.AllSymbols.ToList();
        Assert.That(allSymbols.Count, Is.EqualTo(5));

        var parameters = symbolTable.Parameters.ToList();
        Assert.That(parameters.Count, Is.EqualTo(2));

        var locals = symbolTable.Locals.ToList();
        Assert.That(locals.Count, Is.EqualTo(2));

        var captures = symbolTable.Captures.ToList();
        Assert.That(captures.Count, Is.EqualTo(1));
    }

    [Test]
    public void SymbolTable_ToStringWorks()
    {
        // Arrange
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Act
        symbolTable.RegisterParameter("x", pos);
        symbolTable.RegisterLocal("a", pos);
        symbolTable.RegisterCapture("outer", pos);
        var str = symbolTable.ToString();

        // Assert
        Assert.That(str, Does.Contain("Params:1"));
        Assert.That(str, Does.Contain("Locals:1"));
        Assert.That(str, Does.Contain("Captures:1"));
    }

    [Test]
    public void SlotInfo_ToStringWorks()
    {
        // Arrange
        var pos = BadSourcePosition.FromSource("test", 0, 4);

        // Act
        var regularSlot = new BadSlotInfo("x", 0, false, pos);
        var captureSlot = new BadSlotInfo("outer", 2, true, pos);

        // Assert
        Assert.That(regularSlot.ToString(), Does.Contain("Slot[0]"));
        Assert.That(regularSlot.ToString(), Does.Contain("x"));
        Assert.That(captureSlot.ToString(), Does.Contain("capture"));
    }

    [Test]
    public void SymbolTable_EmptyTableHasZeroCounts()
    {
        var symbolTable = new BadSymbolTable();

        Assert.That(symbolTable.ParameterCount, Is.EqualTo(0));
        Assert.That(symbolTable.LocalCount, Is.EqualTo(0));
        Assert.That(symbolTable.CaptureCount, Is.EqualTo(0));
        Assert.That(symbolTable.TotalSlotCount, Is.EqualTo(0));
        Assert.That(symbolTable.AllSymbols, Is.Empty);
    }

    [Test]
    public void SymbolTable_TryGetSymbol_ReturnsFalseForUnknown()
    {
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        symbolTable.RegisterParameter("x", pos);

        bool found = symbolTable.TryGetSymbol("notExisting", out BadSlotInfo? info);

        Assert.That(found, Is.False);
        Assert.That(info, Is.Null);
    }

    [Test]
    public void SymbolTable_DuplicateLocalThrows()
    {
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        symbolTable.RegisterLocal("a", pos);

        Assert.Throws<InvalidOperationException>(() => symbolTable.RegisterLocal("a", pos));
    }

    [Test]
    public void SymbolTable_DuplicateCaptureThrows()
    {
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        symbolTable.RegisterCapture("outer", pos);

        Assert.Throws<InvalidOperationException>(() => symbolTable.RegisterCapture("outer", pos));
    }

    [Test]
    public void SymbolTable_CrossTypeDuplicate_LocalAfterParameterThrows()
    {
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        symbolTable.RegisterParameter("x", pos);

        Assert.Throws<InvalidOperationException>(() => symbolTable.RegisterLocal("x", pos));
    }

    [Test]
    public void SymbolTable_LocalCountWithoutParameters()
    {
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        symbolTable.RegisterLocal("a", pos);
        symbolTable.RegisterLocal("b", pos);

        Assert.That(symbolTable.ParameterCount, Is.EqualTo(0));
        Assert.That(symbolTable.LocalCount, Is.EqualTo(2));
        Assert.That(symbolTable.TotalSlotCount, Is.EqualTo(2));
    }

    [Test]
    public void SymbolTable_SlotIndicesAreContiguous()
    {
        var symbolTable = new BadSymbolTable();
        var pos = BadSourcePosition.FromSource("test", 0, 4);
        symbolTable.RegisterParameter("x", pos);
        symbolTable.RegisterLocal("a", pos);
        symbolTable.RegisterLocal("b", pos);
        symbolTable.RegisterCapture("outer", pos);

        int total = symbolTable.TotalSlotCount;
        var indices = symbolTable.AllSymbols.Select(s => s.SlotIndex).OrderBy(i => i).ToList();

        Assert.That(indices, Is.EqualTo(Enumerable.Range(0, total).ToList()));
    }

    [Test]
    public void SlotInfo_WithNullPosition_DoesNotThrow()
    {
        var slot = new BadSlotInfo("x", 0, false, null);

        Assert.That(slot.Position, Is.Null);
        Assert.That(slot.ToString(), Does.Contain("x"));
    }
}


