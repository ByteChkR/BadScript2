using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant;

public class BadArrayExpression : BadExpression
{
    private readonly BadExpression[] m_InitExpressions;

    public BadArrayExpression(BadExpression[] initExpressions, BadSourcePosition position) : base(
        false,
        false,
        position
    )
    {
        m_InitExpressions = initExpressions;
    }

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
            }

            array.Add(o);
        }

        yield return new BadArray(array);
    }
}