using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Block.Lock
{
    /// <summary>
    ///     Implements the Lock Expression
    /// </summary>
    public class BadLockExpression : BadExpression
    {
        /// <summary>
        ///     The Block Body
        /// </summary>
        private readonly BadExpression[] m_Block;

        /// <summary>
        ///     The Expression to lock on
        /// </summary>
        private readonly BadExpression m_LockExpression;

        /// <summary>
        ///     Constructor of the Lock Expression
        /// </summary>
        /// <param name="position">Source Position of the Expression</param>
        /// <param name="lockExpression">The expression to lock on</param>
        /// <param name="block">The Block Body</param>
        public BadLockExpression(BadSourcePosition position, BadExpression lockExpression, BadExpression[] block) : base(false, position)
        {
            m_LockExpression = lockExpression;
            m_Block = block;
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject lockObj = BadObject.Null;
            foreach (BadObject o in m_LockExpression.Execute(context))
            {
                lockObj = o;

                yield return o;
            }

            lockObj = lockObj.Dereference();

            if (lockObj is not BadArray && lockObj is not BadTable && lockObj is BadClass)
            {
                throw new BadRuntimeException("Lock object must be of type Array, Object or Class", Position);
            }

            while (!BadLockList.Instance.TryAquire(lockObj))
            {
                yield return BadObject.Null;
            }

            foreach (BadObject o in context.Execute(m_Block))
            {
                yield return o;
            }

            BadLockList.Instance.Release(lockObj);
        }
    }
}