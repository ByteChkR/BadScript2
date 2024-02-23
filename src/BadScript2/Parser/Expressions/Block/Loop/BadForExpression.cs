using BadScript2.Common;
using BadScript2.Optimizations.Folding;
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
    private readonly List<BadExpression> m_Body;

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
        IEnumerable<BadExpression> body,
        BadSourcePosition position) : base(false, position)
    {
        VarDef = varDef;
        Condition = condition;
        VarIncrement = varIncrement;
        m_Body = body.ToList();
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

    /// <summary>
    ///     Sets the Body of the Loop
    /// </summary>
    /// <param name="body">The new Body of the Loop</param>
    public void SetBody(IEnumerable<BadExpression> body)
    {
        m_Body.Clear();
        m_Body.AddRange(body);
    }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        Condition = BadConstantFoldingOptimizer.Optimize(Condition);
        VarDef = BadConstantFoldingOptimizer.Optimize(VarDef);
        VarIncrement = BadConstantFoldingOptimizer.Optimize(VarIncrement);

        for (int i = 0; i < m_Body.Count; i++)
        {
            m_Body[i] = BadConstantFoldingOptimizer.Optimize(m_Body[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression? vDef in VarDef.GetDescendantsAndSelf())
        {
            yield return vDef;
        }

        foreach (BadExpression? vInc in VarIncrement.GetDescendantsAndSelf())
        {
            yield return vInc;
        }

        foreach (BadExpression? cond in Condition.GetDescendantsAndSelf())
        {
            yield return cond;
        }

        foreach (BadExpression descendant in m_Body.SelectMany(expression => expression.GetDescendantsAndSelf()))
        {
            yield return descendant;
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        using BadExecutionContext loopCtx =
            new BadExecutionContext(context.Scope.CreateChild("ForLoop", context.Scope, null));

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

        if (context.Scope.IsError)
        {
            yield break;
        }

        IBadBoolean bRet = cond.Dereference() as IBadBoolean ??
                           throw new BadRuntimeException("While Condition is not a boolean", Position);

        while (bRet.Value)
        {
            using BadExecutionContext loopContext = new BadExecutionContext(
                loopCtx.Scope.CreateChild(
                    "InnerForLoop",
                    loopCtx.Scope,
                    null,
                    BadScopeFlags.Breakable | BadScopeFlags.Continuable
                )
            );

            if (m_Body.Count != 0)
            {
                foreach (BadObject o in loopContext.Execute(m_Body))
                {
                    yield return o;
                }

                if (loopContext.Scope.IsBreak || loopContext.Scope.ReturnValue != null || loopContext.Scope.IsError)
                {
                    break;
                }
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

            if (loopContext.Scope.IsError)
            {
                yield break;
            }

            bRet = cond.Dereference() as IBadBoolean ??
                   throw new BadRuntimeException("While Condition is not a boolean", Position);
        }

        yield return BadObject.Null;
    }
}