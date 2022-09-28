using System.Reflection;

using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members
{
    public class BadReflectedProperty : BadReflectedMember
    {
        public readonly PropertyInfo Property;

        public BadReflectedProperty(PropertyInfo property) : base(property.Name)
        {
            Property = property;
        }

        public override BadObject Get(object instance)
        {
            return Wrap(Property.GetValue(instance));
        }
    }
}