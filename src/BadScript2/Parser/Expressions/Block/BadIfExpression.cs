using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Block;

public class BadIfExpression : BadExpression
{
    private readonly Dictionary<BadExpression, BadExpression[]> m_ConditionalBranches;
    private readonly BadExpression[]? m_ElseBranch;

    public BadIfExpression(
        Dictionary<BadExpression, BadExpression[]> branches,
        BadExpression[]? elseBranch,
        BadSourcePosition position) : base(false, false, position)
    {
        m_ConditionalBranches = branches;
        m_ElseBranch = elseBranch;
    }

    public override void Optimize()
    {
        KeyValuePair<BadExpression, BadExpression[]>[] branches = m_ConditionalBranches.ToArray();
        m_ConditionalBranches.Clear();
        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in branches)
        {
            m_ConditionalBranches[BadExpressionOptimizer.Optimize(branch.Key)] =
                BadExpressionOptimizer.Optimize(branch.Value).ToArray();
        }

        if (m_ElseBranch != null)
        {
            for (int i = 0; i < m_ElseBranch.Length; i++)
            {
                m_ElseBranch[i] = BadExpressionOptimizer.Optimize(m_ElseBranch[i]);
            }
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

            if (conditionResult is not IBadBoolean cBool)
            {
                throw new BadRuntimeException("Condition must be a boolean", Position);
            }

            if (cBool.Value)
            {
                BadExecutionContext branchContext = new BadExecutionContext(
                    context.Scope.CreateChild(
                        "IfBranch",
                        context.Scope
                    )
                );
                foreach (BadObject o in branchContext.Execute(keyValuePair.Value))
                {
                    yield return o;
                }

                yield break;
            }
        }

        if (m_ElseBranch is not null)
        {
            BadExecutionContext elseContext = new BadExecutionContext(
                context.Scope.CreateChild(
                    "IfBranch",
                    context.Scope
                )
            );
            foreach (BadObject o in elseContext.Execute(m_ElseBranch))
            {
                yield return o;
            }
        }
    }
}