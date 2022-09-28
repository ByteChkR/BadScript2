using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math
{
    /// <summary>
    ///     Implements the Modulus Expression
    /// </summary>
    public class BadModulusExpression : BadBinaryExpression
    {
        /// <summary>
        ///     Constructor of the Modulus Expression
        /// </summary>
        /// <param name="left">Left side of the Expression</param>
        /// <param name="right">Right side of the Expression</param>
        /// <param name="position">Source Position of the Expression</param>
        public BadModulusExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
            left,
            right,
            position
        ) { }

        protected override string GetSymbol()
        {
            return "%";
        }

        /// <summary>
        ///     Performs the Modulus Operation on left and right
        /// </summary>
        /// <param name="left">Left side of the Expression</param>
        /// <param name="right">Right side of the Expression</param>
        /// <param name="pos">Source position that is used to generate an Exception if left or right are not a number</param>
        /// <returns>The result of the modulus operation of left by right</returns>
        /// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side are not inheriting from IBadNumber</exception>
        public static BadObject Mod(BadObject left, BadObject right, BadSourcePosition pos)
        {
            if (left is IBadNumber lNum && right is IBadNumber rNum)
            {
                return BadObject.Wrap(lNum.Value % rNum.Value);
            }

            throw new BadRuntimeException($"Can not apply operator '%' to {left} and {right}", pos);
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject left = BadObject.Null;
            foreach (BadObject o in Left.Execute(context))
            {
                left = o;

                yield return o;
            }

            left = left.Dereference();
            BadObject right = BadObject.Null;
            foreach (BadObject o in Right.Execute(context))
            {
                right = o;

                yield return o;
            }

            right = right.Dereference();

            yield return Mod(left, right, Position);
        }
    }
}