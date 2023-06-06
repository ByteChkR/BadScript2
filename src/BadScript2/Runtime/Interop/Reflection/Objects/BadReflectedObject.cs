using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Reflection.Objects;

public class BadReflectedObject : BadObject
{
	public static readonly BadClassPrototype Prototype = new BadReflectedObjectPrototype();

	public BadReflectedObject(object instance)
	{
		Instance = instance;
		Type = instance.GetType();
		Members = BadReflectedMemberTable.Create(Type);
	}

	public Type Type { get; }

	public object Instance { get; }

	public BadReflectedMemberTable Members { get; }

	public override BadClassPrototype GetPrototype()
	{
		return Prototype;
	}

	public override string ToSafeString(List<BadObject> done)
	{
		return Instance.ToString();
	}

	public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
	{
		if (propName is IBadString s && Members.Contains(s.Value))
		{
			return Members.GetMember(Instance, s.Value);
		}

		return base.GetProperty(propName, caller);
	}

	public override bool HasProperty(BadObject propName)
	{
		if (propName is IBadString s)
		{
			return Members.Contains(s.Value) || base.HasProperty(propName);
		}

		return base.HasProperty(propName);
	}
}
