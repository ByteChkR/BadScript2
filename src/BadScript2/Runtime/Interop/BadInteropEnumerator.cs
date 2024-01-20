using System.Collections;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop;

/// <summary>
///     Implements a simple wrapper for C# IEnumerators to be used in BS2
/// </summary>
public class BadInteropEnumerator : BadObject, IBadEnumerator
{
    /// <summary>
    /// The Prototype for the Interop Enumerator Object
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadInteropEnumerator>(
        "Enumerator",
        (_, _) => throw new BadRuntimeException("Cannot call method on enumerator"),
        BadNativeClassBuilder.Enumerator
    );

    /// <summary>
    ///     Current Function Reference
    /// </summary>
    private readonly BadObjectReference m_Current;

    /// <summary>
    ///     The Internal Enumerator
    /// </summary>
    private readonly IEnumerator<BadObject> m_Enumerator;

    /// <summary>
    ///     GetNext Function Reference
    /// </summary>
    private readonly BadObjectReference m_Next;

    /// <summary>
    ///     Creates a new Interop Enumerator
    /// </summary>
    /// <param name="enumerator">Enumerator to iterate</param>
    public BadInteropEnumerator(IEnumerator<BadObject> enumerator)
    {
        m_Enumerator = enumerator;
        BadDynamicInteropFunction next = new BadDynamicInteropFunction(
            "MoveNext",
            _ => m_Enumerator.MoveNext(),
            BadNativeClassBuilder.GetNative("bool")
        );

        BadDynamicInteropFunction current = new BadDynamicInteropFunction(
            "GetCurrent",
            _ => m_Enumerator.Current,
            BadAnyPrototype.Instance
        );

        m_Next = BadObjectReference.Make("BadArrayEnumerator.MoveNext", () => next);
        m_Current = BadObjectReference.Make("BadArrayEnumerator.GetCurrent", () => current);
    }

    /// <summary>
    /// Moves the Enumerator to the next element
    /// </summary>
    /// <returns>True if the Enumerator moved to the next element</returns>
    public bool MoveNext()
    {
        return m_Enumerator.MoveNext();
    }

    /// <summary>
    /// Resets the Enumerator
    /// </summary>
    public void Reset()
    {
        m_Enumerator.Reset();
    }

    /// <summary>
    /// The Current Element
    /// </summary>
    public BadObject Current => m_Enumerator.Current!;

    /// <summary>
    /// The Current Element
    /// </summary>
    object IEnumerator.Current => m_Enumerator.Current!;

    /// <summary>
    /// Disposes the Enumerator
    /// </summary>
    public void Dispose()
    {
        m_Enumerator.Dispose();
    }

    /// <inheritdoc/>
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc/>
    public override bool HasProperty(BadObject propName, BadScope? caller = null)
    {
        return propName is IBadString str &&
               str.Value is "MoveNext" or "GetCurrent" ||
               base.HasProperty(propName, caller);
    }

    /// <inheritdoc/>
    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        if (propName is not IBadString str)
        {
            return base.GetProperty(propName, caller);
        }

        switch (str.Value)
        {
            case "MoveNext":
                return m_Next;
            case "GetCurrent":
                return m_Current;
        }

        return base.GetProperty(propName, caller);
    }

    /// <inheritdoc/>
    public override string ToSafeString(List<BadObject> done)
    {
        return "InteropEnumerator";
    }
}