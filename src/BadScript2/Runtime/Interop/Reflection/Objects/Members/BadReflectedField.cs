using System.Reflection;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

/// <summary>
///     Implements a Reflected Field Member
/// </summary>
public class BadReflectedField : BadReflectedMember
{
    /// <summary>
    /// The Reflected Field
    /// </summary>
    private readonly FieldInfo m_Info;

    /// <summary>
    /// Creates a new BadReflectedField
    /// </summary>
    /// <param name="field">The Reflected Field</param>
    public BadReflectedField(FieldInfo field) : base(field.Name)
    {
        m_Info = field;
    }

    /// <inheritdoc/>
    public override bool IsReadOnly => m_Info.IsInitOnly;

    /// <inheritdoc/>
    public override BadObject Get(object instance)
    {
        return Wrap(m_Info.GetValue(instance));
    }

    /// <inheritdoc/>
    public override void Set(object instance, BadObject o)
    {
        if (o is not IBadNative native || !m_Info.FieldType.IsAssignableFrom(native.Type))
        {
            throw new BadRuntimeException("Invalid Reflection Set");
        }

        m_Info.SetValue(instance, native.Value);
    }
}