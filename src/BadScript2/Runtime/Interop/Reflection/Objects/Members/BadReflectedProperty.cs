using System.Reflection;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

public class BadReflectedProperty : BadReflectedMember
{
    private readonly PropertyInfo Property;

    public BadReflectedProperty(PropertyInfo property) : base(property.Name)
    {
        Property = property;
    }

    public override bool IsReadOnly => !Property.CanWrite;

    public override BadObject Get(object instance)
    {
        return Wrap(Property.GetValue(instance));
    }

    public override void Set(object instance, BadObject o)
    {
        if (o is not IBadNative native || !Property.PropertyType.IsAssignableFrom(native.Type))
        {
            throw new BadRuntimeException("Invalid Reflection Set");
        }
        Property.SetValue(instance, native.Value);
    }
}