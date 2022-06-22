using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Variables;

public class BadFormattedStringExpression : BadStringExpression
{
    private readonly BadExpression[] m_Expressions;

    public BadFormattedStringExpression(BadExpression[] exprs, string str, BadSourcePosition position) : base(
        str,
        position
    )
    {
        m_Expressions = exprs;
    }

    public override void Optimize()
    {
        for (int i = 0; i < m_Expressions.Length; i++)
        {
            m_Expressions[i] = BadExpressionOptimizer.Optimize(m_Expressions[i]);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        List<BadObject> objs = new List<BadObject>();
        foreach (BadExpression expr in m_Expressions)
        {
            BadObject obj = BadObject.Null;
            foreach (BadObject o in expr.Execute(context))
            {
                if (context.Scope.IsError)
                {
                    yield break;
                }

                obj = o;

                yield return o;
            }

            objs.Add(obj.Dereference());
        }

        yield return string.Format(Value, objs.Cast<object?>().ToArray());
    }
}