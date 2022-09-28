using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Block
{
    /// <summary>
    ///     Implements the Try Catch Statement Expression
    /// </summary>
    public class BadTryCatchExpression : BadExpression
    {
        /// <summary>
        ///     The Catch Block
        /// </summary>
        private readonly BadExpression[] m_CatchExpressions;

        /// <summary>
        ///     The Variable name of the Exception inside the catch block
        /// </summary>
        private readonly string m_ErrorName;

        /// <summary>
        ///     The Try Block
        /// </summary>
        private readonly BadExpression[] m_Expressions;

        /// <summary>
        ///     Constructor for the Try Catch Expression
        /// </summary>
        /// <param name="position">Source position of the Expression</param>
        /// <param name="expressions">The Try Block</param>
        /// <param name="catchExpressions">The Catch Block</param>
        /// <param name="errorName">The Variable name of the Exception inside the Catch block</param>
        public BadTryCatchExpression(
            BadSourcePosition position,
            BadExpression[] expressions,
            BadExpression[] catchExpressions,
            string errorName) : base(false, position)
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