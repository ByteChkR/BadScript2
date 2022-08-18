using System.Text;

using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Block.Loop;

public class BadWhileExpression : BadExpression
{
    private readonly List<BadExpression> m_Body;

    public BadWhileExpression(BadExpression condition, List<BadExpression> block, BadSourcePosition position) : base(
        false,
        false,
        position
    )
    {
        Condition = condition;
        m_Body = block;
    }

    private BadExpression Condition { get; set; }
    

    public override void Optimize()
    {
        Condition = BadExpressionOptimizer.Optimize(Condition);
        for (int i = 0; i < m_Body.Count; i++)
        {
            m_Body[i] = BadExpressionOptimizer.Optimize(m_Body[i]);
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder($"while ({Condition})");
        sb.AppendLine();
        sb.AppendLine("{");
        foreach (BadExpression expression in m_Body)
        {
            sb.AppendLine($"\t{expression}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject cond = BadObject.Null;
        foreach (BadObject o in Condition.Execute(context))
        {
            cond = o;

            yield return o;
        }

        IBadBoolean bRet = cond.Dereference() as IBadBoolean ??
                           throw new BadRuntimeException("While Condition is not a boolean", Position);

        while (bRet.Value)
        {
            BadExecutionContext loopContext = new BadExecutionContext(
                context.Scope.CreateChild(
                    "WhileLoop",
                    context.Scope,
                    BadScopeFlags.Breakable | BadScopeFlags.Continuable
                )
            );
            foreach (BadObject o in loopContext.Execute(m_Body))
            {
                yield return o;
            }

            if (loopContext.Scope.IsBreak || loopContext.Scope.ReturnValue != null || loopContext.Scope.IsError)
            {
                break;
            }

            foreach (BadObject o in Condition.Execute(context))
            {
                cond = o;

                yield return o;
            }

            bRet = cond.Dereference() as IBadBoolean ??
                   throw new BadRuntimeException("While Condition is not a boolean", Position);
        }

        yield return BadObject.Null;
    }
}