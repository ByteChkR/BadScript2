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
    public BadForEachExpression(BadExpression target,
                                BadWordToken loopVariable,
                                IEnumerable<BadExpression> body,
                                BadSourcePosition position) : base(false,
                                                                   position
                                                                  )
    {
        Target = target;
        LoopVariable = loopVariable;
        m_Body = body.ToList();
    }

    /// <summary>
    ///     The Loop Body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;

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
    public static (BadFunction moveNext, BadFunction getCurrent) FindEnumerator(
        BadObject target,
        BadExecutionContext context,
        BadSourcePosition position)
    {
        if (!target.HasProperty("MoveNext", context.Scope))
        {
            throw new BadRuntimeException("Invalid enumerator", position);
        }

        if (target.GetProperty("MoveNext", context.Scope)
                  .Dereference(position) is not BadFunction moveNext)
        {
            throw new BadRuntimeException("Invalid enumerator", position);
        }

        if (!target.HasProperty("GetCurrent", context.Scope))
        {
            throw new BadRuntimeException("Invalid enumerator", position);
        }

        if (target.GetProperty("GetCurrent", context.Scope)
                  .Dereference(position) is BadFunction getCurrent)
        {
            return (moveNext, getCurrent);
        }

        throw new BadRuntimeException("Invalid enumerator", position);
    }

    public static IEnumerable<BadObject> Enumerate(BadExecutionContext context,
                                                   BadObject target,
                                                   BadSourcePosition position,
                                                   Func<Action, BadExecutionContext, BadObject, BadSourcePosition, IEnumerable<BadObject>> action)
    {
        if (target.HasProperty("GetEnumerator", context.Scope))
        {
            if (target.GetProperty("GetEnumerator", context.Scope)
                      .Dereference(position) is not BadFunction getEnumerator)
            {
                throw new BadRuntimeException("Invalid enumerator", position);
            }

            BadObject newTarget = BadObject.Null;

            foreach (BadObject o in getEnumerator.Invoke(Array.Empty<BadObject>(), context))
            {
                yield return o;

                newTarget = o;
            }

            if (newTarget == BadObject.Null)
            {
                throw BadRuntimeException.Create(context.Scope, $"Invalid enumerator: {target}", position);
            }

            target = newTarget.Dereference(position);
        }

        (BadFunction moveNext, BadFunction getCurrent) = FindEnumerator(target, context, position);

        BadObject cond = BadObject.Null;

        foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), context))
        {
            cond = o;

            yield return o;
        }

        IBadBoolean bRet = cond.Dereference(position) as IBadBoolean ??
                           throw new BadRuntimeException("While Condition is not a boolean", position);

        while (bRet.Value)
        {
            using BadExecutionContext loopContext = new BadExecutionContext(context.Scope.CreateChild("ForEachLoop",
                                                                                 context.Scope,
                                                                                 null,
                                                                                 BadScopeFlags.Breakable |
                                                                                 BadScopeFlags.Continuable
                                                                                )
                                                                           );

            BadObject current = BadObject.Null;

            foreach (BadObject o in getCurrent.Invoke(Array.Empty<BadObject>(), loopContext))
            {
                current = o;

                yield return o;
            }

            bool bBreak = false;

            foreach (BadObject? o in action(() => bBreak = true, loopContext, current.Dereference(position), position))
            {
                yield return o;
            }

            if (bBreak)
            {
                break;
            }

            foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), loopContext))
            {
                cond = o;

                yield return o;
            }

            bRet = cond.Dereference(position) as IBadBoolean ??
                   throw BadRuntimeException.Create(context.Scope,
                                                    "Enumerator MoveNext did not return a boolean",
                                                    position
                                                   );
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject target = BadObject.Null;

        foreach (BadObject o in Target.Execute(context))
        {
            target = o;

            yield return o;
        }

        target = target.Dereference(Position);

        foreach (BadObject? o in Enumerate(context, target, Position, LoopBody))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> LoopBody(Action breakLoop, BadExecutionContext loopContext, BadObject current, BadSourcePosition position)
    {
        if (m_Body.Count != 0)
        {
            current = current.Dereference(position);
            loopContext.Scope.DefineVariable(LoopVariable.Text, current);

            foreach (BadObject o in loopContext.Execute(m_Body))
            {
                yield return o;
            }

            if (loopContext.Scope.IsBreak || loopContext.Scope.ReturnValue != null /*|| loopContext.Scope.IsError*/)
            {
                breakLoop();
            }
        }
    }
}