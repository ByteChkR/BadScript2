using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Types;

/// <summary>
/// Implements an Interface Prototype Expression.
/// </summary>
public class BadInterfacePrototypeExpression : BadExpression
{
	/// <summary>
	/// The Constraints for the Interface
	/// </summary>
	private readonly BadInterfaceFunctionConstraint[] m_Constraints;
	/// <summary>
	/// The Inherited Interfaces
	/// </summary>
	private readonly BadExpression[] m_Interfaces;
	/// <summary>
	/// Meta Data for the Interface
	/// </summary>
	private readonly BadMetaData? m_MetaData;

	/// <summary>
	/// Creates a new Interface Prototype Expression
	/// </summary>
	/// <param name="name">Name of the Interface</param>
	/// <param name="constraints">Constraints for the Interface</param>
	/// <param name="interfaces">Inherited Interfaces</param>
	/// <param name="metaData">Meta Data for the Interface</param>
	/// <param name="position">Source Position</param>
	public BadInterfacePrototypeExpression(
		string name,
		BadInterfaceFunctionConstraint[] constraints,
		BadExpression[] interfaces,
		BadMetaData? metaData,
		BadSourcePosition position) : base(false, position)
	{
		m_Interfaces = interfaces;
		m_MetaData = metaData;
		Name = name;
		m_Constraints = constraints;
	}

	/// <summary>
	///     The Interface Name
	/// </summary>
	public string Name { get; }

	public override IEnumerable<BadExpression> GetDescendants()
	{
		yield break;
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		List<BadInterfacePrototype> interfaces = new List<BadInterfacePrototype>();

		foreach (BadExpression interfae in m_Interfaces)
		{
			BadObject obj = BadObject.Null;

			foreach (BadObject o in interfae.Execute(context))
			{
				obj = o;
			}

			if (obj.Dereference() is not BadInterfacePrototype cls)
			{
				throw new BadRuntimeException("Base Class is not a class");
			}

			interfaces.Add(cls);
		}

		BadInterfaceConstraint[] GetConstraints()
		{
			Console.WriteLine($"Get Constraints for : {Name} {Position}");
			BadInterfaceConstraint[] constrainsts = new BadInterfaceConstraint[m_Constraints.Length];

			for (int i = 0; i < m_Constraints.Length; i++)
			{
				BadInterfaceFunctionConstraint c = m_Constraints[i];
				constrainsts[i] = new BadInterfaceFunctionConstraint(c.Name,
					c.Return,
					c.Parameters.Select(x => x.Initialize(context)).ToArray());
			}

			return constrainsts;
		}

		//Create the Interface Prototype
		BadInterfacePrototype intf = new BadInterfacePrototype(Name, interfaces.ToArray(), m_MetaData, GetConstraints);
		context.Scope.DefineVariable(Name, intf, context.Scope, new BadPropertyInfo(intf.GetPrototype(), true));

		yield return intf;
	}
}