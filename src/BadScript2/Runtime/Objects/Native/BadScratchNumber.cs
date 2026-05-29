using System.Globalization;

using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Phase C2 – Escape Analysis.
///     A mutable <see cref="BadNumber"/>-like object owned by a single
///     <c>BadRuntimeVirtualMachine</c> instance and reused for arithmetic
///     results whose <c>TransientResult</c> instruction flag is set.
///     The VM guarantees that the scratch object is consumed by the very next
///     arithmetic or comparison instruction and never stored in a variable,
///     returned, or passed to an external call.
/// </summary>
internal sealed class BadScratchNumber : BadObject, IBadNumber
{
    private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("num");

    /// <summary>The mutable backing value.</summary>
    internal decimal ScratchValue;

    internal BadScratchNumber(decimal initial)
    {
        ScratchValue = initial;
    }

    // IBadNumber
    decimal IBadNumber.Value => ScratchValue;

    public override BadClassPrototype GetPrototype() => s_Prototype;

    public override string ToSafeString(List<BadObject> done) =>
        ScratchValue.ToString(CultureInfo.InvariantCulture);

    public override bool HasProperty(string propName, BadScope? caller = null)
        => caller != null && caller.Provider.HasObject<decimal>(propName);

    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
        => BadObjectReference.Make($"BadScratchNumber.{propName}",
               p => caller != null
                   ? caller.Provider.GetObject<decimal>(propName, this, caller, p)
                   : throw new InvalidOperationException($"No property '{propName}' on scratch number"));

    // IBadNative explicit implementations
    object IBadNative.Value => ScratchValue;
    Type IBadNative.Type => typeof(decimal);

    bool IEquatable<IBadNative>.Equals(IBadNative? other)
        => other is not null && other.Value.Equals((object)ScratchValue);
}
