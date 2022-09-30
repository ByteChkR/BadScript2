using System.Collections;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Utility;

namespace BadScript2.Runtime.Interop
{
    public class BadInteropEnumerable : BadObject, IBadEnumerable
    {
        private readonly IEnumerable<BadObject> m_Enumerable;
        private readonly BadFunction m_Func;
        public BadInteropEnumerable(IEnumerable<BadObject> enumerable)
        {
            m_Enumerable = enumerable;

            m_Func = new BadDynamicInteropFunction("GetEnumerator",
                c => new BadInteropEnumerator(m_Enumerable.GetEnumerator())
            );
        }

        public override BadClassPrototype GetPrototype()
        {
            return new BadNativeClassPrototype<BadInteropEnumerator>(
                "Enumerable",
                (_, _) => throw new BadRuntimeException("Cannot call method")
            );
        }

        public override bool HasProperty(BadObject propName)
        {
            return propName is IBadString { Value: "GetEnumerator" } || base.HasProperty(propName);
        }

        public override BadObjectReference GetProperty(BadObject propName)
        {
            if (propName is IBadString { Value: "GetEnumerator" })
            {
                return BadObjectReference.Make("GetEnumerator", () => m_Func);
            }
            return base.GetProperty(propName);
        }

        public override string ToSafeString(List<BadObject> done)
        {
            return "BadInteropEnumerable";
        }

        public IEnumerator<BadObject> GetEnumerator()
        {
            return m_Enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_Enumerable).GetEnumerator();
        }
    }
}