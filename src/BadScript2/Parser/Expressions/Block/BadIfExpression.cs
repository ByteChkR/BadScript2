using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Block;

/// <summary>
///     Implements the If Statement Expression
/// </summary>
public class BadIfExpression : BadExpression
{
	/// <summary>
	///     The Conditional Branches
	/// </summary>
	private readonly Dictionary<BadExpression, BadExpression[]> m_ConditionalBranches;

	/// <summary>
	///     The (optional) Else Branch
	/// </summary>
	private List<BadExpression>? m_ElseBranch;

	/// <summary>
	///     Constructor of the If Expression
	/// </summary>
	/// <param name="branches">The Conditional Branches</param>
	/// <param name="elseBranch">The (optional) Else Branch</param>
	/// <param name="position">Source Position of the Expression</param>
	public BadIfExpression(
        Dictionary<BadExpression, BadExpression[]> branches,
        IEnumerable<BadExpression>? elseBranch,
        BadSourcePosition position) : base(false, position)
    {
        m_ConditionalBranches = branches;
        m_ElseBranch = elseBranch?.ToList();
    }

	/// <summary>
	///     The Conditional Branches
	/// </summary>
	public IDictionary<BadExpression, BadExpression[]> ConditionalBranches => m_ConditionalBranches;

	/// <summary>
	///     The (optional) Else Branch
	/// </summary>
	public IEnumerable<BadExpression>? ElseBranch => m_ElseBranch;

    public void SetElseBranch(IEnumerable<BadExpression>? branch)
    {
        if (branch == null)
        {
            m_ElseBranch = null;
        }
        else if (m_ElseBranch != null)
        {
            m_ElseBranch.Clear();
            m_ElseBranch.AddRange(branch);
        }
        else
        {
            m_ElseBranch = new List<BadExpression>(branch);
        }
    }

    public override void Optimize()
    {
        KeyValuePair<BadExpression, BadExpression[]>[] branches = m_ConditionalBranches.ToArray();
        m_ConditionalBranches.Clear();

        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in branches)
        {
            m_ConditionalBranches[BadConstantFoldingOptimizer.Optimize(branch.Key)] =
                BadConstantFoldingOptimizer.Optimize(branch.Value).ToArray();
        }

        if (m_ElseBranch == null)
        {
            return;
        }

        for (int i = 0; i < m_ElseBranch.Count; i++)
        {
            m_ElseBranch[i] = BadConstantFoldingOptimizer.Optimize(m_ElseBranch[i]);
        }
    }

    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in m_ConditionalBranches)
        {
            foreach (BadExpression keyExpr in branch.Key.GetDescendantsAndSelf())
            {
                yield return keyExpr;
            }

            foreach (BadExpression valueExpr in branch.Value)
            {
                foreach (BadExpression expr in valueExpr.GetDescendantsAndSelf())
                {
                    yield return expr;
                }
            }
        }

        if (m_ElseBranch == null)
        {
            yield break;
        }

        foreach (BadExpression e in m_ElseBranch.SelectMany(expr => expr.GetDescendantsAndSelf()))
            {
                yield return e;
            }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        foreach (KeyValuePair<BadExpression, BadExpression[]> keyValuePair in m_ConditionalBranches)
        {
            BadObject conditionResult = BadObject.Null;

            foreach (BadObject o in keyValuePair.Key.Execute(context))
            {
                conditionResult = o;

                yield return o;
            }

            conditionResult = conditionResult.Dereference();


            if (context.Scope.IsError)
            {
                yield break;
            }

            if (conditionResult is not IBadBoolean cBool)
            {
                throw new BadRuntimeException("Condition must be a boolean", Position);
            }

            if (!cBool.Value)
            {
                continue;
            }

            {
                BadExecutionContext branchContext = new BadExecutionContext(
                    context.Scope.CreateChild(
                        "IfBranch",
                        context.Scope,
                        null
                    )
                );

                foreach (BadObject o in branchContext.Execute(keyValuePair.Value))
                {
                    yield return o;
                }

                yield break;
            }
        }

        if (m_ElseBranch is null)
        {
            yield break;
        }

        {
            BadExecutionContext elseContext = new BadExecutionContext(
                context.Scope.CreateChild(
                    "IfBranch",
                    context.Scope,
                    null
                )
            );

            foreach (BadObject o in elseContext.Execute(m_ElseBranch))
            {
                yield return o;
            }
        }
    }
}