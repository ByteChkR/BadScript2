using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Types;

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
