using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Types;

public class BadInterfaceFunctionConstraint : BadInterfaceConstraint
{
	public readonly string Name;
	public readonly BadFunctionParameter[] Parameters;
	public readonly BadExpression? Return;

	public override string ToString()
	{
		return "FunctionConstraint";
	}

	public BadInterfaceFunctionConstraint(string name, BadExpression? @return, BadFunctionParameter[] parameters)
	{
		Name = name;
		Return = @return;
		Parameters = parameters;
	}

	public override void Validate(BadClass obj, List<BadInterfaceValidatorError> errors)
	{
		if (!obj.HasProperty(Name))
		{
			errors.Add(new BadInterfaceValidatorError($"Missing Property. Expected {Name}", this));

			return;
		}

		BadObject o = obj.GetProperty(Name).Dereference();

		if (o is not BadFunction f)
		{
			errors.Add(new BadInterfaceValidatorError($"Property {Name} is not a function", this));

			return;
		}

		if (f.Parameters.Length != Parameters.Length)
		{
			errors.Add(new BadInterfaceValidatorError($"Parameter Count Mismatch. Expected {Parameters.Length} but got {f.Parameters.Length} in {f}", this));

			return;
		}

		for (int i = 0; i < Parameters.Length; i++)
		{
			BadFunctionParameter p = f.Parameters[i];
			BadFunctionParameter p2 = Parameters[i];

			if (p.IsOptional != p2.IsOptional)
			{
				errors.Add(new BadInterfaceValidatorError($"{f}: Parameter Optional Flags are not equal. Implementation: {p}, Expectation: {p2}", this));

				return;
			}

			if (p.IsNullChecked != p2.IsNullChecked)
			{
				errors.Add(new BadInterfaceValidatorError($"{f}: Parameter Null Check Flags are not equal. Implementation: {p}, Expectation: {p2}", this));

				return;
			}

			if (p.IsRestArgs != p2.IsRestArgs)
			{
				errors.Add(new BadInterfaceValidatorError($"{f}: Parameter Rest Args Flags are not equal. Implementation: {p}, Expectation: {p2}", this));


				return;
			}

			if (p.TypeExpr != null)
			{
				if (p2.Type != null && p.Type!=p2.Type)
				{
					errors.Add(new BadInterfaceValidatorError($"{f}: Parameter Types not equal. Implementation: {p}, Expectation: {p2}", this));

					return;
				}
			}
		}
	}
}

public class BadInterfacePrototypeExpression : BadExpression
{
	private readonly BadInterfaceFunctionConstraint[] m_Constraints;
	private readonly BadExpression[] m_Interfaces;
	private readonly BadMetaData? m_MetaData;

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

/// <summary>
///     Implements the Class Prototype Expression
/// </summary>
public class BadClassPrototypeExpression : BadExpression
{
	private readonly BadExpression[] m_BaseClasses;

	/// <summary>
	///     The Class Body
	/// </summary>
	private readonly BadExpression[] m_Body;

	private readonly BadMetaData? m_MetaData;
	private readonly BadExpression[] m_StaticBody;

	/// <summary>
	///     Constructor of the Class Prototype Expression
	/// </summary>
	/// <param name="name">The Class name</param>
	/// <param name="body">The Class Body</param>
	/// <param name="baseClass">The (optional) base class</param>
	/// <param name="position">The Source Position of the Expression</param>
	public BadClassPrototypeExpression(
		string name,
		BadExpression[] body,
		BadExpression[] staticBody,
		BadExpression[] baseClasses,
		BadSourcePosition position,
		BadMetaData? metaData) : base(false, position)
	{
		Name = name;
		m_Body = body;
		m_BaseClasses = baseClasses;
		m_MetaData = metaData;
		m_StaticBody = staticBody;
	}

	/// <summary>
	///     The Class Body
	/// </summary>
	public IEnumerable<BadExpression> Body => m_Body;

	/// <summary>
	///     The Class Name
	/// </summary>
	public string Name { get; }

	public override void Optimize()
	{
		for (int i = 0; i < m_Body.Length; i++)
		{
			m_Body[i] = BadExpressionOptimizer.Optimize(m_Body[i]);
		}

		for (int i = 0; i < m_StaticBody.Length; i++)
		{
			m_StaticBody[i] = BadExpressionOptimizer.Optimize(m_StaticBody[i]);
		}
	}

	public override IEnumerable<BadExpression> GetDescendants()
	{
		foreach (BadExpression baseClass in m_BaseClasses)
		{
			foreach (BadExpression baseExpr in baseClass.GetDescendantsAndSelf())
			{
				yield return baseExpr;
			}
		}

		foreach (BadExpression expression in m_Body)
		{
			foreach (BadExpression e in expression.GetDescendantsAndSelf())
			{
				yield return e;
			}
		}

		foreach (BadExpression expression in m_StaticBody)
		{
			foreach (BadExpression e in expression.GetDescendantsAndSelf())
			{
				yield return e;
			}
		}
	}

	private BadClassPrototype? GetPrototype(
		BadExecutionContext context,
		out BadInterfacePrototype[] interfaces)
	{
		BadClassPrototype? baseClass = null;

		List<BadInterfacePrototype> interfacesList = new List<BadInterfacePrototype>();

		for (int i = 0; i < m_BaseClasses.Length; i++)
		{
			BadExpression baseClassExpr = m_BaseClasses[i];
			BadObject[] baseClassObj = context.Execute(baseClassExpr).ToArray();

			if (baseClassObj.Length != 1)
			{
				throw new BadRuntimeException(
					$"Base Class Expression {baseClassExpr} returned {baseClassObj.Length} Objects. Expected 1.",
					baseClassExpr.Position);
			}

			BadObject o = baseClassObj[0].Dereference();

			if (o is BadInterfacePrototype iface)
			{
				interfacesList.Add(iface);
			}
			else if (o is BadClassPrototype p)
			{
				if (i != 0)
				{
					throw new BadRuntimeException(
						$"Base Class Expression {baseClassExpr} returned a Class Prototype. Expected an Interface Prototype.",
						baseClassExpr.Position);
				}

				baseClass = p;
			}
			else
			{
				throw new BadRuntimeException(
					$"Base Class Expression {baseClassExpr} returned an Object of Type {o}. Expected a Class Prototype or an Interface Prototype.",
					baseClassExpr.Position);
			}
		}

		interfaces = interfacesList.ToArray();

		return baseClass;
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadClassPrototype? basePrototype = GetPrototype(context, out BadInterfacePrototype[] interfaces);


		BadExecutionContext staticContext =
			new BadExecutionContext(context.Scope.CreateChild($"static:{Name}", context.Scope, true));

		foreach (BadObject o in staticContext.Execute(m_StaticBody))
		{
			yield return o;
		}

		BadClassPrototype p = new BadExpressionClassPrototype(Name,
			context.Scope,
			m_Body,
			basePrototype,
			interfaces,
			m_MetaData,
			staticContext.Scope);
		context.Scope.DefineVariable(Name, p);

		yield return p;
	}
}
