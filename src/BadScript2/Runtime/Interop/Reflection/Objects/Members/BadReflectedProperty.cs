using System.Reflection;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

/// <summary>
///     Implements a Reflected Property Member
/// </summary>
public class BadReflectedProperty : BadReflectedMember
{
    /// <summary>
    /// The Reflected Property
    /// </summary>
    private readonly PropertyInfo m_Property;

    /// <summary>
    /// Creates a new BadReflectedProperty
    /// </summary>
    /// <param name="property">The Reflected Property</param>
    public BadReflectedProperty(PropertyInfo property) : base(property.Name)
    {
        m_Property = property;
    }

    /// <inheritdoc/>
    public override bool IsReadOnly => !m_Property.CanWrite;

    /// <inheritdoc/>
    public override BadObject Get(object instance)
    {
        return Wrap(m_Property.GetValue(instance));
    }

    /// <inheritdoc/>
    public override void Set(object instance, BadObject o)
    {
        if (o is not IBadNative native || !m_Property.PropertyType.IsAssignableFrom(native.Type))
        {
            throw new BadRuntimeException("Invalid Reflection Set");
        }

        m_Property.SetValue(instance, native.Value);
    }
}