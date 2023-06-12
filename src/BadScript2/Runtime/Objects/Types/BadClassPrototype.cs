using BadScript2.Parser;
using BadScript2.Runtime.Error;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Implements a Class Prototype for the BadScript Language
/// </summary>
public abstract class BadClassPrototype : BadObject
{
	/// <summary>
	///     The Prototype of the ClassPrototype(can not be used)
	/// </summary>
	public static readonly BadClassPrototype Prototype = new BadNativeClassPrototype<BadClassPrototype>("Class",
		(_, _) => throw new BadRuntimeException("Classes cannot be extended"));

	/// <summary>
	///     The Base Class of the Prototype
	/// </summary>
	protected readonly BadClassPrototype? BaseClass;

	public readonly BadMetaData MetaData;

	/// <summary>
	///     Creates a new Class Prototype
	/// </summary>
	/// <param name="name">Class Name</param>
	/// <param name="baseClass">Base Class Prototype</param>
	protected BadClassPrototype(string name, BadClassPrototype? baseClass, BadMetaData? metaData)
	{
		Name = name;
		BaseClass = baseClass;
		MetaData = metaData ?? BadMetaData.Empty;
	}

	/// <summary>
	///     The Name of the Type
	/// </summary>
	public string Name { get; }

	public BadClassPrototype? GetBaseClass()
	{
		return BaseClass;
	}

	public override BadClassPrototype GetPrototype()
	{
		return Prototype;
	}

	/// <summary>
	///     Creates an Instance of the Class
	/// </summary>
	/// <param name="caller">The Caller Context</param>
	/// <param name="setThis">
	///     Indicates if the Creation Function should set the "this" instance to the resulting instance of
	///     the class.
	/// </param>
	/// <returns></returns>
	public abstract IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true);

	/// <summary>
	///     Returns true if the Class is a Subclass of the given Class
	/// </summary>
	/// <param name="proto">The Other Class</param>
	/// <returns>true if the Class is a Subclass of the given Class</returns>
	public bool IsSuperClassOf(BadClassPrototype proto)
	{
		return proto == this || (BaseClass?.IsSuperClassOf(proto) ?? false);
	}

	/// <summary>
	///     Returns true if the provided object is an instance of this class or a subclass of this class
	/// </summary>
	/// <param name="obj">The Other Class</param>
	/// <returns>true if the object is a Subclass of this Class</returns>
	public virtual bool IsAssignableFrom(BadObject obj)
	{
		if (obj == Null)
		{
			return true;
		}

		if (obj is BadClass cls)
		{
			return cls.InheritsFrom(this);
		}

		return false;
	}

	public override string ToSafeString(List<BadObject> done)
	{
		return $"prototype {Name}";
	}
}
