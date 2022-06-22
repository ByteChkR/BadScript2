using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Block
{
    public class BadTryCatchExpression : BadExpression
    {
        private readonly BadExpression[] m_CatchExpressions;
        private readonly string m_ErrorName;
        private readonly BadExpression[] m_Expressions;

        public BadTryCatchExpression(
            BadSourcePosition position,
            BadExpression[] expressions,
            BadExpression[] catchExpressions,
            string errorName) : base(false, false, position)
        {
            m_Expressions = expressions;
            m_CatchExpressions = catchExpressions;
            m_ErrorName = errorName;
        }

        public override void Optimize()
        {
            for (int i = 0; i < m_CatchExpressions.Length; i++)
            {
                m_CatchExpressions[i] = BadExpressionOptimizer.Optimize(m_CatchExpressions[i]);
            }

            for (int i = 0; i < m_Expressions.Length; i++)
            {
                m_Expressions[i] = BadExpressionOptimizer.Optimize(m_Expressions[i]);
            }
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadExecutionContext tryContext = new BadExecutionContext(
                context.Scope.CreateChild("TryBlock", context.Scope, BadScopeFlags.CaptureThrow)
            );
            foreach (BadObject o in tryContext.Execute(m_Expressions))
            {
                yield return o;
            }

            if (tryContext.Scope.Error != null)
            {
                BadExecutionContext catchContext = new BadExecutionContext(
                    context.Scope.CreateChild("CatchBlock", context.Scope)
                );
                catchContext.Scope.DefineVariable(m_ErrorName, tryContext.Scope.Error);
                foreach (BadObject o in catchContext.Execute(m_CatchExpressions))
                {
                    yield return o;
                }
            }
        }
    }
}