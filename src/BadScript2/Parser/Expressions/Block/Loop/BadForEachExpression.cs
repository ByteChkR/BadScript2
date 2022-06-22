using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Block.Loop
{
    public class BadForEachExpression : BadExpression
    {
        private readonly BadExpression[] m_Body;
        private readonly BadWordToken m_LoopVariable;
        private BadExpression m_Target;

        public BadForEachExpression(
            BadExpression target,
            BadWordToken loopVariable,
            BadExpression[] body,
            BadSourcePosition position) : base(
            false,
            false,
            position
        )
        {
            m_Target = target;
            m_LoopVariable = loopVariable;
            m_Body = body;
        }

        public override void Optimize()
        {
            m_Target = BadExpressionOptimizer.Optimize(m_Target);
            for (int i = 0; i < m_Body.Length; i++)
            {
                m_Body[i] = BadExpressionOptimizer.Optimize(m_Body[i]);
            }
        }

        private (BadFunction moveNext, BadFunction getCurrent) FindEnumerator(BadObject target)
        {
            if (target.HasProperty("MoveNext"))
            {
                BadFunction? moveNext = target.GetProperty("MoveNext").Dereference() as BadFunction;
                if (moveNext != null)
                {
                    if (target.HasProperty("GetCurrent"))
                    {
                        BadFunction? getCurrent = target.GetProperty("GetCurrent").Dereference() as BadFunction;
                        if (getCurrent != null)
                        {
                            return (moveNext, getCurrent);
                        }
                    }
                }
            }

            throw new BadRuntimeException("Invalid enumerator", Position);
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject target = BadObject.Null;
            foreach (BadObject o in m_Target.Execute(context))
            {
                target = o;

                yield return o;
            }

            target = target.Dereference();

            if (target.HasProperty("GetEnumerator"))
            {
                BadFunction? getEnumerator = target.GetProperty("GetEnumerator").Dereference() as BadFunction;
                if (getEnumerator == null)
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
                    throw new BadRuntimeException($"Invalid enumerator: {target}", Position);
                }

                target = newTarget.Dereference();
            }

            (BadFunction moveNext, BadFunction getCurrent) = FindEnumerator(target);

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
                BadExecutionContext loopContext = new BadExecutionContext(
                    context.Scope.CreateChild(
                        "ForEachLoop",
                        context.Scope,
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

                current = current.Dereference();

                loopContext.Scope.DefineVariable(m_LoopVariable.Text, current);

                foreach (BadObject o in loopContext.Execute(m_Body))
                {
                    yield return o;
                }

                if (loopContext.Scope.IsBreak || loopContext.Scope.ReturnValue != null || loopContext.Scope.IsError)
                {
                    break;
                }

                foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), loopContext))
                {
                    cond = o;

                    yield return o;
                }

                bRet = cond.Dereference() as IBadBoolean ??
                       throw new BadRuntimeException("Enumerator MoveNext did not return a boolean", Position);
            }
        }
    }
}