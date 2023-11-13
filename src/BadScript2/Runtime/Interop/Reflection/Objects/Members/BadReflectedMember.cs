using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

public abstract class BadReflectedMember
{
	protected BadReflectedMember(string name)
	{
		Name = name;
	}

	public abstract bool IsReadOnly { get; }

	protected string Name { get; }

	public abstract BadObject Get(object instance);

	public abstract void Set(object instance, BadObject o);

	protected BadObject Wrap(object? o)
	{
		if (BadObject.CanWrap(o))
		{
			return BadObject.Wrap(o);
		}

		return new BadReflectedObject(o!);
	}
}
