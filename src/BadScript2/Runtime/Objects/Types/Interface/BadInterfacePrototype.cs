using BadScript2.Parser;
using BadScript2.Runtime.Error;

namespace BadScript2.Runtime.Objects.Types;

public abstract class BadInterfaceConstraint
{
	public abstract void Validate(BadClass obj, List<BadInterfaceValidatorError> errors);
}
public class BadInterfacePrototype : BadClassPrototype
{
	private BadInterfaceConstraint[]? m_Constraints;
	private readonly Func<BadInterfaceConstraint[]> m_ConstraintsFunc;
	public IReadOnlyCollection<BadInterfaceConstraint> Constraints => m_Constraints ??= m_ConstraintsFunc();
	public override BadClassPrototype GetPrototype()
	{
		return new BadNativeClassPrototype<BadClassPrototype>("Interface",
			(_, _) => throw new BadRuntimeException("Interfaces cannot be extended"));
	}

	public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
	{
		throw BadRuntimeException.Create(caller.Scope, "Interfaces cannot be instantiated");
	}

	public BadInterfacePrototype(string name, BadInterfacePrototype[] interfaces, BadMetaData? metaData, Func<BadInterfaceConstraint[]> constraints) : base(name, null, interfaces, metaData)
	{
		m_ConstraintsFunc = constraints;
	}

	public override bool IsSuperClassOf(BadClassPrototype proto)
	{
		return base.IsSuperClassOf(proto) || proto.Interfaces.Any(IsSuperClassOf);
	}
	
	
	public override string ToSafeString(List<BadObject> done)
	{
		return $"interface {Name}";
	}
}