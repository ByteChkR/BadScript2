using System.Reflection;

using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

public class BadReflectedField : BadReflectedMember
{
    public readonly FieldInfo FieldInfo;

    public BadReflectedField(FieldInfo field) : base(field.Name)
    {
        FieldInfo = field;
    }

    public override BadObject Get(object instance)
    {
        return Wrap(FieldInfo.GetValue(instance));
    }
}