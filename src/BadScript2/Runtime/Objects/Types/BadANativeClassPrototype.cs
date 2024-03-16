using BadScript2.Parser;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Implements a Native Class Prototype
/// </summary>
public abstract class BadANativeClassPrototype : BadClassPrototype
{
	/// <summary>
	///     The Constructor for the Class
	/// </summary>
	private readonly Func<BadExecutionContext, BadObject[], BadObject> m_Func;

	/// <summary>
	///     Creates a new Native Class Prototype
	/// </summary>
	/// <param name="name">Class Name</param>
	/// <param name="func">Class Constructor</param>
	/// <param name="interfaces">The Implemented Interfaces</param>
	/// <param name="meta">The Metadata of the Class</param>
	protected BadANativeClassPrototype(
        string name,
        Func<BadExecutionContext, BadObject[], BadObject> func,
        BadMetaData? meta = null) : base(name, meta)
    {
        m_Func = func;
    }

	protected override BadClassPrototype? BaseClass { get; } = BadAnyPrototype.Instance;

	/// <summary>
	///     Creates an instance of the Class
	/// </summary>
	/// <param name="caller">Caller Context</param>
	/// <param name="args">Constructor Arguments</param>
	/// <returns>Enumeration of BadObjects that were created by the exeuction of the constructor</returns>
	public IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, BadObject[] args)
    {
        yield return m_Func(caller, args);
    }

    /// <inheritdoc />
    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        return CreateInstance(caller, Array.Empty<BadObject>());
    }
}