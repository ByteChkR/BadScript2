using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Reflection.Objects
{
    internal class BadReflectedObjectPrototype : BadNativeClassPrototype
    {
        internal BadReflectedObjectPrototype() : base("BadReflectedObject", Constructor) { }

        private static BadObject Constructor(BadExecutionContext arg1, BadObject[] arg2)
        {
            throw new BadRuntimeException("Can not create a BadReflectedObject inside the Script");
        }
    }
}