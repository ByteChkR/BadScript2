using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Block;

/// <summary>
///     Implements the Try Catch Statement Expression
/// </summary>
public class BadTryCatchExpression : BadExpression
{
    /// <summary>
    ///     The Variable name of the Exception inside the catch block
    /// </summary>
    public readonly string ErrorName;

    /// <summary>
    ///     The Catch Block
    /// </summary>
    private readonly BadExpression[] m_CatchExpressions;

    /// <summary>
    ///     The Try Block
    /// </summary>
    private readonly BadExpression[] m_Expressions;

    /// <summary>
    ///     Constructor for the Try Catch Expression
    /// </summary>
    /// <param name="position">Source position of the Expression</param>
    /// <param name="expressions">The Try Block</param>
    /// <param name="catchExpressions">The Catch Block</param>
    /// <param name="errorName">The Variable name of the Exception inside the Catch block</param>
    public BadTryCatchExpression(
		BadSourcePosition position,
		BadExpression[] expressions,
		BadExpression[] catchExpressions,
		string errorName) : base(false, position)
	{
		m_Expressions = expressions;
		m_CatchExpressions = catchExpressions;
		ErrorName = errorName;
	}

	public IEnumerable<BadExpression> CatchExpressions => m_CatchExpressions;

	public IEnumerable<BadExpression> TryExpressions => m_Expressions;

	public override void Optimize()
	{
		for (int i = 0; i < m_CatchExpressions.Length; i++)
		{
			m_CatchExpressions[i] = BadConstantFoldingOptimizer.Optimize(m_CatchExpressions[i]);
		}

		for (int i = 0; i < m_Expressions.Length; i++)
		{
			m_Expressions[i] = BadConstantFoldingOptimizer.Optimize(m_Expressions[i]);
		}
	}

	public override IEnumerable<BadExpression> GetDescendants()
	{
		foreach (BadExpression expression in m_Expressions)
		{
			foreach (BadExpression e in expression.GetDescendantsAndSelf())
			{
				yield return e;
			}
		}

		foreach (BadExpression expression in m_CatchExpressions)
		{
			foreach (BadExpression e in expression.GetDescendantsAndSelf())
			{
				yield return e;
			}
		}
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadExecutionContext tryContext = new BadExecutionContext(
			context.Scope.CreateChild("TryBlock", context.Scope, null, BadScopeFlags.CaptureThrow));

		foreach (BadObject o in tryContext.Execute(m_Expressions))
		{
			yield return o;
		}

		if (tryContext.Scope.Error != null)
		{
			BadExecutionContext catchContext = new BadExecutionContext(
				context.Scope.CreateChild("CatchBlock", context.Scope, null));
			catchContext.Scope.DefineVariable(ErrorName, tryContext.Scope.Error);

			foreach (BadObject e in catchContext.Execute(m_CatchExpressions))
			{
				yield return e;
			}
		}
	}
}
