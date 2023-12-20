using System.Collections;
using System.Reflection;

using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

/// <summary>
///     Implements a Reflected 'GetEnumerator' Method
/// </summary>
public class BadReflectedEnumeratorMethod : BadReflectedMethod
{
	public BadReflectedEnumeratorMethod(MethodInfo method) : base(method) { }

	private IEnumerable<BadObject> GetEnumerable(object instance)
	{
		IEnumerable o = (IEnumerable)instance;

		foreach (object obj in o)
		{
			yield return Wrap(obj);
		}
	}

	public override BadObject Get(object? instance)
	{
		return new BadInteropFunction("GetEnumerator",
			_ => new BadInteropEnumerator(GetEnumerable(instance!).GetEnumerator()),
			false,
			BadAnyPrototype.Instance);
	}
}
