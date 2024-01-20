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
    private readonly FieldInfo m_Info;

    public BadReflectedField(FieldInfo field) : base(field.Name)
    {
        m_Info = field;
    }

    public override bool IsReadOnly => m_Info.IsInitOnly;

    public override BadObject Get(object instance)
    {
        return Wrap(m_Info.GetValue(instance));
    }

    public override void Set(object instance, BadObject o)
    {
        if (o is not IBadNative native || !m_Info.FieldType.IsAssignableFrom(native.Type))
        {
            throw new BadRuntimeException("Invalid Reflection Set");
        }

        m_Info.SetValue(instance, native.Value);
    }
}