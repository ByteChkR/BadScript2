using System.Reflection;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

public class BadReflectedProperty : BadReflectedMember
{
    private readonly PropertyInfo m_Property;

    public BadReflectedProperty(PropertyInfo property) : base(property.Name)
    {
        m_Property = property;
    }

    public override bool IsReadOnly => !m_Property.CanWrite;

    public override BadObject Get(object instance)
    {
        return Wrap(m_Property.GetValue(instance));
    }

    public override void Set(object instance, BadObject o)
    {
        if (o is not IBadNative native || !m_Property.PropertyType.IsAssignableFrom(native.Type))
        {
            throw new BadRuntimeException("Invalid Reflection Set");
        }

        m_Property.SetValue(instance, native.Value);
    }
}