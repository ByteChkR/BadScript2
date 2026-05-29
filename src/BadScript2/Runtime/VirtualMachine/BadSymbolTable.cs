using BadScript2.Common;

namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
/// Represents information about a single symbol (variable, parameter, or capture).
/// </summary>
public sealed class BadSlotInfo
{
    /// <summary>
    /// The name of the symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The slot index where this symbol is stored (in the locals array or parent frame).
    /// </summary>
    public int SlotIndex { get; }

    /// <summary>
    /// Indicates if this slot holds a captured variable from an outer scope.
    /// </summary>
    public bool IsCapture { get; }

    /// <summary>
    /// The source position where this symbol was defined.
    /// </summary>
    public BadSourcePosition? Position { get; }

    /// <summary>
    /// Creates a new SlotInfo instance.
    /// </summary>
    public BadSlotInfo(string name, int slotIndex, bool isCapture = false, BadSourcePosition? position = null)
    {
        Name = name;
        SlotIndex = slotIndex;
        IsCapture = isCapture;
        Position = position;
    }

    /// <inheritdoc />
    public override string ToString() => $"Slot[{SlotIndex}] {Name}{(IsCapture ? " (capture)" : "")}";
}

/// <summary>
/// Symbol table for compiled function scopes.
/// Maps variable names to their slot indices.
/// </summary>
public sealed class BadSymbolTable
{
    private readonly Dictionary<string, BadSlotInfo> m_Symbols = new Dictionary<string, BadSlotInfo>();
    private readonly List<BadSlotInfo> m_Parameters = new List<BadSlotInfo>();
    private readonly List<BadSlotInfo> m_Locals = new List<BadSlotInfo>();
    private readonly List<BadSlotInfo> m_Captures = new List<BadSlotInfo>();
    private BadSlotInfo[]? m_ParametersArray;
    private BadSlotInfo[]? m_LocalsArray;
    private BadSlotInfo[]? m_CapturesArray;
    private Dictionary<int, BadSlotInfo>? m_SlotIndexMap;
    private int m_NextSlotIndex;

    /// <summary>
    /// The number of parameters in this function.
    /// </summary>
    public int ParameterCount { get; private set; }

    /// <summary>
    /// The number of local variables (excluding parameters and captures).
    /// </summary>
    public int LocalCount => m_NextSlotIndex - ParameterCount - CaptureCount;

    /// <summary>
    /// The number of captured variables.
    /// </summary>
    public int CaptureCount { get; private set; }

    /// <summary>
    /// Total slot count required.
    /// </summary>
    public int TotalSlotCount => m_NextSlotIndex;

    /// <summary>
    /// Registers a parameter in the symbol table.
    /// Parameters are allocated first (slots 0..N-1).
    /// </summary>
    public BadSlotInfo RegisterParameter(string name, BadSourcePosition? position = null)
    {
        if (m_Symbols.ContainsKey(name))
        {
            throw new InvalidOperationException($"Symbol '{name}' already registered");
        }

        var info = new BadSlotInfo(name, m_NextSlotIndex, isCapture: false, position);
        m_Symbols[name] = info;
        m_Parameters.Add(info);
        m_ParametersArray = null;
        m_SlotIndexMap = null;
        m_NextSlotIndex++;
        ParameterCount++;
        return info;
    }

    /// <summary>
    /// Registers a local variable in the symbol table.
    /// Locals are allocated after parameters.
    /// </summary>
    public BadSlotInfo RegisterLocal(string name, BadSourcePosition? position = null)
    {
        if (m_Symbols.ContainsKey(name))
        {
            throw new InvalidOperationException($"Symbol '{name}' already registered");
        }

        var info = new BadSlotInfo(name, m_NextSlotIndex, isCapture: false, position);
        m_Symbols[name] = info;
        m_Locals.Add(info);
        m_LocalsArray = null;
        m_SlotIndexMap = null;
        m_NextSlotIndex++;
        return info;
    }

    /// <summary>
    /// Registers a captured variable from an outer scope.
    /// Captures are allocated after locals.
    /// </summary>
    public BadSlotInfo RegisterCapture(string name, BadSourcePosition? position = null)
    {
        if (m_Symbols.ContainsKey(name))
        {
            throw new InvalidOperationException($"Symbol '{name}' already registered");
        }

        var info = new BadSlotInfo(name, m_NextSlotIndex, isCapture: true, position);
        m_Symbols[name] = info;
        m_Captures.Add(info);
        m_CapturesArray = null;
        m_SlotIndexMap = null;
        m_NextSlotIndex++;
        CaptureCount++;
        return info;
    }

    /// <summary>
    /// Tries to get a symbol by name.
    /// </summary>
    public bool TryGetSymbol(string name, out BadSlotInfo? info)
    {
        return m_Symbols.TryGetValue(name, out info);
    }

    /// <summary>
    /// Gets a symbol by name. Throws if not found.
    /// </summary>
    public BadSlotInfo GetSymbol(string name)
    {
        if (!m_Symbols.TryGetValue(name, out var info))
        {
            throw new KeyNotFoundException($"Symbol '{name}' not found in symbol table");
        }

        return info;
    }

    /// <summary>
    /// Tries to get a symbol by slot index (optimized for hot path).
    /// </summary>
    public bool TryGetSymbolByIndex(int slotIndex, out BadSlotInfo? info)
    {
        info = null;

        if (m_SlotIndexMap == null)
        {
            m_SlotIndexMap = new Dictionary<int, BadSlotInfo>();
            foreach (BadSlotInfo symbol in m_Symbols.Values)
            {
                m_SlotIndexMap[symbol.SlotIndex] = symbol;
            }
        }

        return m_SlotIndexMap.TryGetValue(slotIndex, out info);
    }

    /// <summary>
    /// Gets all registered symbols.
    /// </summary>
    public IEnumerable<BadSlotInfo> AllSymbols => m_Symbols.Values;

    /// <summary>
    /// Gets all parameter symbols.
    /// </summary>
    public IEnumerable<BadSlotInfo> Parameters
    {
        get
        {
            if (m_ParametersArray == null)
            {
                m_ParametersArray = m_Parameters.ToArray();
            }

            return m_ParametersArray;
        }
    }

    /// <summary>
    /// Gets all local symbols (excluding captures).
    /// </summary>
    public IEnumerable<BadSlotInfo> Locals
    {
        get
        {
            if (m_LocalsArray == null)
            {
                m_LocalsArray = m_Locals.ToArray();
            }

            return m_LocalsArray;
        }
    }

    /// <summary>
    /// Gets all captured symbols.
    /// </summary>
    public IEnumerable<BadSlotInfo> Captures
    {
        get
        {
            if (m_CapturesArray == null)
            {
                m_CapturesArray = m_Captures.ToArray();
            }

            return m_CapturesArray;
        }
    }

    /// <inheritdoc />
    public override string ToString() => $"SymbolTable[Params:{ParameterCount}, Locals:{LocalCount}, Captures:{CaptureCount}]";
}


