using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Block.Loop;

/// <summary>
///     Implements the For Loop Expression
/// </summary>
public class BadForExpression : BadExpression
{
    /// <summary>
    ///     Loop Body
    /// </summary>
    private readonly BadExpression[] m_Body;

    /// <summary>
    ///     Constructor of the For Expression
    /// </summary>
    /// <param name="varDef">The Variable Definition part of the for loop</param>
    /// <param name="condition">The Exit Condition of the For Loop</param>
    /// <param name="varIncrement">The Variable Modifier Expression of the For Loop</param>
    /// <param name="body">The Loop Body</param>
    /// <param name="position">The source position of the Expression</param>
    public BadForExpression(
        BadExpression varDef,
        BadExpression condition,
        BadExpression varIncrement,
        BadExpression[] body,
        BadSourcePosition position) : base(false, position)
    {
        VarDef = varDef;
        Condition = condition;
        VarIncrement = varIncrement;
        m_Body = body;
    }

    /// <summary>
    ///     Loop Body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;

    /// <summary>
    ///     The Exit Condition of the For Loop
    /// </summary>
    public BadExpression Condition { get; private set; }

    /// <summary>
    ///     The Variable Definition part of the for loop
    /// </summary>
    public BadExpression VarDef { get; private set; }

    /// <summary>
    ///     The Variable Modifier Expression of the For Loop
    /// </summary>
    public BadExpression VarIncrement { get; private set; }

    public override void Optimize()
    {
        Condition = BadExpressionOptimizer.Optimize(Condition);
        VarDef = BadExpressionOptimizer.Optimize(VarDef);
        VarIncrement = BadExpressionOptimizer.Optimize(VarIncrement);
        for (int i = 0; i < m_Body.Length; i++)
        {
            m_Body[i] = BadExpressionOptimizer.Optimize(m_Body[i]);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadExecutionContext loopCtx = new BadExecutionContext(context.Scope.CreateChild("ForLoop", context.Scope));

        foreach (BadObject o in VarDef.Execute(loopCtx))
        {
            yield return o;
        }


        BadObject cond = BadObject.Null;
        foreach (BadObject o in Condition.Execute(loopCtx))
        {
            cond = o;

            yield return o;
        }

        IBadBoolean bRet = cond.Dereference() as IBadBoolean ??
                           throw new BadRuntimeException("While Condition is not a boolean", Position);

        while (bRet.Value)
        {
            BadExecutionContext loopContext = new BadExecutionContext(
                loopCtx.Scope.CreateChild(
                    "InnerForLoop",
                    loopCtx.Scope,
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

            foreach (BadObject o in VarIncrement.Execute(loopContext))
            {
                yield return o;
            }

            foreach (BadObject o in Condition.Execute(loopContext))
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