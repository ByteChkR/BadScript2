using System.Text;

using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Block.Loop;

/// <summary>
///     Implements the While Expression
/// </summary>
public class BadWhileExpression : BadExpression
{
    /// <summary>
    ///     The Loop Body
    /// </summary>
    private readonly List<BadExpression> m_Body;

    /// <summary>
    ///     Constructor of the While Expression
    /// </summary>
    /// <param name="condition">The condition of the loop</param>
    /// <param name="block">The Loop Body</param>
    /// <param name="position">Source position of the Expression</param>
    public BadWhileExpression(BadExpression condition, List<BadExpression> block, BadSourcePosition position) : base(
        false,
        position
    )
    {
        Condition = condition;
        m_Body = block;
    }

    /// <summary>
    ///     The Loop Body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;

    /// <summary>
    ///     The Loop Condition
    /// </summary>
    public BadExpression Condition { get; private set; }

    /// <summary>
    ///     Sets the Body of the Loop
    /// </summary>
    /// <param name="body">The new Body</param>
    public void SetBody(IEnumerable<BadExpression> body)
    {
        m_Body.Clear();
        m_Body.AddRange(body);
    }


    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        Condition = BadConstantFoldingOptimizer.Optimize(Condition);

        for (int i = 0; i < m_Body.Count; i++)
        {
            m_Body[i] = BadConstantFoldingOptimizer.Optimize(m_Body[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression expression in Condition.GetDescendantsAndSelf())
        {
            yield return expression;
        }

        foreach (BadExpression descendant in m_Body.SelectMany(expression => expression.GetDescendantsAndSelf()))
        {
            yield return descendant;
        }
    }

    /// <summary>
    ///     Returns a human readable representation of the Expression
    /// </summary>
    /// <returns>String Representation of the Expression</returns>
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

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject cond = BadObject.Null;

        foreach (BadObject o in Condition.Execute(context))
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
                context.Scope.CreateChild(
                    "WhileLoop",
                    context.Scope,
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

            foreach (BadObject o in Condition.Execute(context))
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