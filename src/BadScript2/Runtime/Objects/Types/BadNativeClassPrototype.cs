using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Implements a Native Class Prototype
/// </summary>
/// <typeparam name="T">Native Type</typeparam>
public class BadNativeClassPrototype<T> : BadANativeClassPrototype
    where T : BadObject
{
	/// <summary>
	///     Creates a new Native Class Prototype
	/// </summary>
	/// <param name="name">Class Name</param>
	/// <param name="func">Class Constructor</param>
	public BadNativeClassPrototype(
        string name,
        Func<BadExecutionContext, BadObject[], BadObject> func,
        params BadInterfacePrototype[] interfaces) : base(name, func, null, interfaces) { }

    public override bool IsAbstract { get; } = false;

    public override bool IsAssignableFrom(BadObject obj)
    {
        if (obj == Null)
        {
            return true;
        }

        if (obj is T)
        {
            return true;
        }

        return false;
    }
}