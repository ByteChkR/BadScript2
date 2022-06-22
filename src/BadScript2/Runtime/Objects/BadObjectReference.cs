using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects;

public abstract class BadObjectReference : BadObject
{
    public abstract BadObject Resolve();
    public abstract void Set(BadObject obj, BadPropertyInfo? info = null);

    public static BadObjectReference Make(
        string refText,
        Func<BadObject> getter,
        Action<BadObject, BadPropertyInfo?>? setter = null)
    {
        return new BadObjectReferenceImpl(refText, getter, setter);
    }

    private class BadObjectReferenceImpl : BadObjectReference
    {
        private readonly Func<BadObject> m_Getter;
        private readonly string m_RefText;
        private readonly Action<BadObject, BadPropertyInfo?>? m_Setter;

        public BadObjectReferenceImpl(
            string refText,
            Func<BadObject> getter,
            Action<BadObject, BadPropertyInfo?>? setter)
        {
            m_Getter = getter;
            m_Setter = setter;
            m_RefText = refText;
        }


        public override BadClassPrototype GetPrototype()
        {
            return m_Getter().GetPrototype();
        }

        public override BadObject Resolve()
        {
            return m_Getter();
        }

        public override void Set(BadObject obj, BadPropertyInfo? info = null)
        {
            if (m_Setter == null)
            {
                throw new BadRuntimeException("Cannot set reference " + m_RefText + " because it is read-only");
            }

            m_Setter(obj, info);
        }

        public override string ToSafeString(List<BadObject> done)
        {
            return m_RefText;
        }
    }
}