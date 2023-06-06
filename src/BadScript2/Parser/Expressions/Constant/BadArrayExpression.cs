using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant;

/// <summary>
///     Implements the Array Expression
/// </summary>
public class BadArrayExpression : BadExpression
{
    /// <summary>
    ///     The Initializer List
    /// </summary>
    private readonly BadExpression[] m_InitExpressions;

    /// <summary>
    ///     Constructor of the Array Expression
    /// </summary>
    /// <param name="initExpressions">The initializer list of the Array</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadArrayExpression(BadExpression[] initExpressions, BadSourcePosition position) : base(false,
		position)
	{
		m_InitExpressions = initExpressions;
	}

	public int Length => m_InitExpressions.Length;

    /// <summary>
    ///     The Initializer List
    /// </summary>
    public IEnumerable<BadExpression> InitExpressions => m_InitExpressions;

	public override void Optimize()
	{
		for (int i = 0; i < m_InitExpressions.Length; i++)
		{
			m_InitExpressions[i] = BadExpressionOptimizer.Optimize(m_InitExpressions[i]);
		}
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		List<BadObject> array = new List<BadObject>();

		foreach (BadExpression expression in m_InitExpressions)
		{
			BadObject o = BadObject.Null;

			foreach (BadObject obj in expression.Execute(context))
			{
				o = obj;

				yield return o;
			}

			array.Add(o.Dereference());
		}

		yield return new BadArray(array);
	}
}
