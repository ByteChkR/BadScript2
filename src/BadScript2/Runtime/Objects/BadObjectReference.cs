using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects
{
    /// <summary>
    ///     Implements the base functionality for a BadScript Reference
    /// </summary>
    public abstract class BadObjectReference : BadObject
    {
        /// <summary>
        ///     Returns the Referenced Object
        /// </summary>
        /// <returns>Referenced Object</returns>
        public abstract BadObject Resolve();

        /// <summary>
        ///     Sets the Referenced Object to a new Value
        /// </summary>
        /// <param name="obj">New Value</param>
        /// <param name="info">(Optional) Property Info</param>
        public abstract void Set(BadObject obj, BadPropertyInfo? info = null);

        /// <summary>
        ///     Creates a new Reference Object
        /// </summary>
        /// <param name="refText">Text used for debugging</param>
        /// <param name="getter">Getter of the Property</param>
        /// <param name="setter">(optional) Setter of the Property</param>
        /// <returns>Reference Instance</returns>
        public static BadObjectReference Make(
            string refText,
            Func<BadObject> getter,
            Action<BadObject, BadPropertyInfo?>? setter = null)
        {
            return new BadObjectReferenceImpl(refText, getter, setter);
        }

        /// <summary>
        ///     Implements a Reference Object
        /// </summary>
        private class BadObjectReferenceImpl : BadObjectReference
        {
            /// <summary>
            ///     The Getter of the Reference
            /// </summary>
            private readonly Func<BadObject> m_Getter;

            /// <summary>
            ///     The Debug Text
            /// </summary>
            private readonly string m_RefText;

            /// <summary>
            ///     The Setter of the Reference
            /// </summary>
            private readonly Action<BadObject, BadPropertyInfo?>? m_Setter;

            /// <summary>
            ///     Creates a new Reference Object
            /// </summary>
            /// <param name="refText">The Reference Debug Text</param>
            /// <param name="getter">Getter of the Reference</param>
            /// <param name="setter">Setter of the Reference</param>
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
}