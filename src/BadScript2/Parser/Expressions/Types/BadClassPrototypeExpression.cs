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
    /// <summary>
    ///     The Class Body
    /// </summary>
    private readonly BadExpression[] m_Body;

	private readonly BadMetaData? m_MetaData;

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
		BadExpression? baseClass,
		BadSourcePosition position,
		BadMetaData? metaData) : base(false, position)
	{
		Name = name;
		m_Body = body;
		BaseClass = baseClass;
		m_MetaData = metaData;
	}

    /// <summary>
    ///     The (optional) Base Class
    /// </summary>
    public BadExpression? BaseClass { get; }

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
	}

	public override IEnumerable<BadExpression> GetDescendants()
	{
		if (BaseClass != null)
		{
			foreach (BadExpression baseExpr in BaseClass.GetDescendantsAndSelf())
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
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadClassPrototype? basePrototype = null;

		if (BaseClass != null)
		{
			BadObject obj = BadObject.Null;

			foreach (BadObject o in BaseClass.Execute(context))
			{
				obj = o;

				yield return o;
			}

			obj = obj.Dereference();

			if (obj is not BadClassPrototype cls)
			{
				throw new BadRuntimeException("Base class must be a class prototype", Position);
			}

			basePrototype = cls;
		}

		BadClassPrototype p = new BadExpressionClassPrototype(Name, context.Scope, m_Body, basePrototype, m_MetaData);
		context.Scope.DefineVariable(Name, p);

		yield return p;
	}
}
