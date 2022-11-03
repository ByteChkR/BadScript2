using System.Collections;

using BadScript2.Common;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop
{
    public class BadRuntimeEnumerator : BadObject, IBadEnumerator
    {
        private readonly BadExecutionContext m_Caller;
        private readonly BadFunction m_GetCurrent;
        private readonly BadFunction m_MoveNext;
        private readonly BadSourcePosition m_Position;

        public BadRuntimeEnumerator(BadExecutionContext caller, BadFunction moveNext, BadFunction getCurrent, BadSourcePosition position)
        {
            m_MoveNext = moveNext;
            m_GetCurrent = getCurrent;
            m_Caller = caller;
            m_Position = position;
        }

        public bool MoveNext()
        {
            BadObject cond = Null;
            foreach (BadObject o in m_MoveNext.Invoke(Array.Empty<BadObject>(), m_Caller))
            {
                cond = o;
            }

            if (m_Caller.Scope.IsError)
            {
                throw new BadRuntimeErrorException(m_Caller.Scope.Error);
            }

            IBadBoolean bRet = cond.Dereference() as IBadBoolean ??
                               throw new BadRuntimeException("While Condition is not a boolean", m_Position);

            return bRet.Value;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public BadObject Current
        {
            get
            {
                BadObject current = Null;
                foreach (BadObject o in m_GetCurrent.Invoke(Array.Empty<BadObject>(), m_Caller))
                {
                    current = o;
                }

                if (m_Caller.Scope.IsError)
                {
                    throw new BadRuntimeErrorException(m_Caller.Scope.Error);
                }

                current = current.Dereference();

                return current;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            //Nothing to dispose
        }

        public override BadClassPrototype GetPrototype()
        {
            return new BadNativeClassPrototype<BadRuntimeEnumerator>(
                "Enumerator",
                (_, _) => throw new BadRuntimeException("Cannot call method")
            );
        }

        public override string ToSafeString(List<BadObject> done)
        {
            return "BadRuntimeEnumerator";
        }
    }
}