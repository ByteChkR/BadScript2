using System.Reflection;

using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

public class BadReflectedField : BadReflectedMember
{
    private readonly FieldInfo m_Info;

    public BadReflectedField(FieldInfo field) : base(field.Name)
    {
        m_Info = field;
    }

    public override BadObject Get(object instance)
    {
        return Wrap(m_Info.GetValue(instance));
    }
}