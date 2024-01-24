using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
/// <summary>
/// Contains the Loop Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions.Block.Loop;

/// <summary>
///     Implements the For Each Expression
/// </summary>
public class BadForEachExpression : BadExpression
{
    /// <summary>
    ///     The Variable Name of the Current Loop iteration
    /// </summary>
    public readonly BadWordToken LoopVariable;

    /// <summary>
    ///     The Loop Body
    /// </summary>
    private readonly List<BadExpression> m_Body;

    /// <summary>
    ///     The Enumerable/Enumerator Expression of the Loop
    /// </summary>
    public BadExpression Target;

    /// <summary>
    ///     Constructor of the For Each Expression
    /// </summary>
    /// <param name="target">The Enumerable/Enumerator Expression of the Loop</param>
    /// <param name="loopVariable">The Variable Name of the Current Loop iteration</param>
    /// <param name="body">The Loop Body</param>
    /// <param name="position">The Source Position of the Expression</param>
    public BadForEachExpression(
        BadExpression target,
        BadWordToken loopVariable,
        IEnumerable<BadExpression> body,
        BadSourcePosition position) : base(
        false,
        position
    )
    {
        Target = target;
        LoopVariable = loopVariable;
        m_Body = body.ToList();
    }

    /// <summary>
    /// The Loop Body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;

    /// <summary>
    /// Sets the Body of the Loop
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
        Target = BadConstantFoldingOptimizer.Optimize(Target);

        for (int i = 0; i < m_Body.Count; i++)
        {
            m_Body[i] = BadConstantFoldingOptimizer.Optimize(m_Body[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression expression in Target.GetDescendantsAndSelf())
        {
            yield return expression;
        }

        foreach (BadExpression descendant in m_Body.SelectMany(expression => expression.GetDescendantsAndSelf()))
        {
            yield return descendant;
        }
    }

    /// <summary>
    ///     Helper Function that returns the MoveNext/GetCurrent function of the Target
    /// </summary>
    /// <param name="target">The loop target</param>
    /// <param name="context">The Calling Context</param>
    /// <returns>Tuple of Functions used in the for each loop</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the target is not an enumerable object.</exception>
    private (BadFunction moveNext, BadFunction getCurrent) FindEnumerator(BadObject target, BadExecutionContext context)
    {
        if (!target.HasProperty("MoveNext", context.Scope))
        {
            throw new BadRuntimeException("Invalid enumerator", Position);
        }

        if (target.GetProperty("MoveNext", context.Scope).Dereference() is not BadFunction moveNext)
        {
            throw new BadRuntimeException("Invalid enumerator", Position);
        }

        if (!target.HasProperty("GetCurrent", context.Scope))
        {
            throw new BadRuntimeException("Invalid enumerator", Position);
        }

        if (target.GetProperty("GetCurrent", context.Scope).Dereference() is BadFunction getCurrent)
        {
            return (moveNext, getCurrent);
        }

        throw new BadRuntimeException("Invalid enumerator", Position);
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject target = BadObject.Null;

        foreach (BadObject o in Target.Execute(context))
        {
            target = o;

            yield return o;
        }

        if (context.Scope.IsError)
        {
            yield break;
        }

        target = target.Dereference();

        if (target.HasProperty("GetEnumerator", context.Scope))
        {
            if (target.GetProperty("GetEnumerator", context.Scope).Dereference() is not BadFunction getEnumerator)
            {
                throw new BadRuntimeException("Invalid enumerator", Position);
            }

            BadObject newTarget = BadObject.Null;

            foreach (BadObject o in getEnumerator.Invoke(Array.Empty<BadObject>(), context))
            {
                yield return o;

                newTarget = o;
            }

            if (context.Scope.IsError)
            {
                yield break;
            }

            if (newTarget == BadObject.Null)
            {
                throw BadRuntimeException.Create(context.Scope, $"Invalid enumerator: {target}", Position);
            }

            target = newTarget.Dereference();
        }

        (BadFunction moveNext, BadFunction getCurrent) = FindEnumerator(target, context);

        if (context.Scope.IsError)
        {
            yield break;
        }

        BadObject cond = BadObject.Null;

        foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), context))
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
                    "ForEachLoop",
                    context.Scope,
                    null,
                    BadScopeFlags.Breakable | BadScopeFlags.Continuable
                )
            );

            BadObject current = BadObject.Null;

            foreach (BadObject o in getCurrent.Invoke(Array.Empty<BadObject>(), loopContext))
            {
                current = o;

                yield return o;
            }

            if (loopContext.Scope.IsError)
            {
                yield break;
            }


            if (m_Body.Count != 0)
            {
                current = current.Dereference();
                loopContext.Scope.DefineVariable(LoopVariable.Text, current);

                foreach (BadObject o in loopContext.Execute(m_Body))
                {
                    yield return o;
                }

                if (loopContext.Scope.IsBreak || loopContext.Scope.ReturnValue != null || loopContext.Scope.IsError)
                {
                    break;
                }
            }

            foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), loopContext))
            {
                cond = o;

                yield return o;
            }


            if (loopContext.Scope.IsError)
            {
                yield break;
            }

            bRet = cond.Dereference() as IBadBoolean ??
                   throw BadRuntimeException.Create(
                       context.Scope,
                       "Enumerator MoveNext did not return a boolean",
                       Position
                   );
        }
    }
}