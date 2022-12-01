using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Reflection.Objects;

internal class BadReflectedObjectPrototype : BadANativeClassPrototype
{
    internal BadReflectedObjectPrototype() : base("BadReflectedObject", (_, _) => throw new BadRuntimeException("Can not create a BadReflectedObject inside the Script")) { }
}