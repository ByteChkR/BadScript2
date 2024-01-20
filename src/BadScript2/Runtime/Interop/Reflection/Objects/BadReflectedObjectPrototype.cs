using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Reflection.Objects;

/// <summary>
///     Implements a Reflected Object Prototype(not usable, just to satisfy the prototype chain)
/// </summary>
internal class BadReflectedObjectPrototype : BadANativeClassPrototype
{
    internal BadReflectedObjectPrototype() : base(
        "BadReflectedObject",
        (_, _) => throw new BadRuntimeException("Can not create a BadReflectedObject inside the Script")
    ) { }

    public override bool IsAbstract { get; } = true;
}