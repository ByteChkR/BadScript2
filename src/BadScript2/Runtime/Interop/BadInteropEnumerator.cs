using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop;

public class BadInteropEnumerator : BadObject
{
    private readonly BadObjectReference m_Current;
    private readonly IEnumerator<BadObject> m_Enumerator;
    private readonly BadObjectReference m_Next;

    public BadInteropEnumerator(IEnumerator<BadObject> enumerator)
    {
        m_Enumerator = enumerator;
        BadDynamicInteropFunction next = new BadDynamicInteropFunction(
            "MoveNext",
            _ => m_Enumerator.MoveNext()
        );

        BadDynamicInteropFunction current = new BadDynamicInteropFunction(
            "GetCurrent",
            _ => m_Enumerator.Current
        );

        m_Next = BadObjectReference.Make("BadArrayEnumerator.MoveNext", () => next);
        m_Current = BadObjectReference.Make("BadArrayEnumerator.GetCurrent", () => current);
    }

    public override BadClassPrototype GetPrototype()
    {
        return new BadNativeClassPrototype<BadInteropEnumerator>(
            "Enumerator",
            (_, _) => throw new BadRuntimeException("Cannot call method on enumerator")
        );
    }

    public override bool HasProperty(BadObject propName)
    {
        return propName is IBadString str &&
               (str.Value == "MoveNext" || str.Value == "GetCurrent") ||
               base.HasProperty(propName);
    }

    public override BadObjectReference GetProperty(BadObject propName)
    {
        if (propName is IBadString str)
        {
            if (str.Value == "MoveNext")
            {
                return m_Next;
            }

            if (str.Value == "GetCurrent")
            {
                return m_Current;
            }
        }

        return base.GetProperty(propName);
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "InteropEnumerator";
    }
}