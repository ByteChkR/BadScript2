using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Atomic
{
    /// <summary>
    ///     Implements the Post Decrement Expression
    /// </summary>
    public class BadPostDecrementExpression : BadExpression
    {
        /// <summary>
        ///     Left side of the expression
        /// </summary>
        public readonly BadExpression Left;

        /// <summary>
        ///     Constructor of the Post Decrement Expression
        /// </summary>
        /// <param name="left">Left side of the Expression</param>
        /// <param name="position">Source position of the Expression</param>
        public BadPostDecrementExpression(BadExpression left, BadSourcePosition position) : base(
            left.IsConstant,
            position
        )
        {
            Left = left;
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject left = BadObject.Null;
            foreach (BadObject o in Left.Execute(context))
            {
                left = o;

                yield return o;
            }

            if (left is not BadObjectReference leftRef)
            {
                throw new BadRuntimeException("Left side of ++ must be a reference", Position);
            }

            left = left.Dereference();

            if (left is not IBadNumber leftNumber)
            {
                throw new BadRuntimeException("Left side of ++ must be a number", Position);
            }

            leftRef.Set(leftNumber.Value - 1);

            yield return left;
        }
    }
}