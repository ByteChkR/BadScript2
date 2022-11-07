using System.Collections;

using BadScript2.Common;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop;

public class BadRuntimeEnumerable : BadObject, IBadEnumerable
{
    private readonly BadExecutionContext m_Caller;
    private readonly BadFunction m_GetCurrent;
    private readonly BadFunction m_MoveNext;
    private readonly BadSourcePosition m_Position;

    public BadRuntimeEnumerable(BadFunction moveNext, BadFunction getCurrent, BadExecutionContext caller, BadSourcePosition position)
    {
        m_MoveNext = moveNext;
        m_GetCurrent = getCurrent;
        m_Caller = caller;
        m_Position = position;
    }

    public IEnumerator<BadObject> GetEnumerator()
    {
        return new BadRuntimeEnumerator(m_Caller, m_MoveNext, m_GetCurrent, m_Position);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override BadClassPrototype GetPrototype()
    {
        return new BadNativeClassPrototype<BadRuntimeEnumerable>(
            "Enumerable",
            (_, _) => throw new BadRuntimeException("Cannot call method")
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        throw new NotImplementedException();
    }
}