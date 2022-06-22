using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Logic
{
    public class BadLogicXOrExpression : BadBinaryExpression
    {
        public BadLogicXOrExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
            left,
            right,
            position
        ) { }

        public static BadObject XOr(BadObject left, BadObject right, BadSourcePosition pos)
        {
            if (left is IBadBoolean lBool && right is IBadBoolean rBool)
            {
                return lBool.Value ^ rBool.Value ? BadObject.True : BadObject.False;
            }

            throw new BadRuntimeException($"Can not apply operator '^' to {left} and {right}", pos);
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

            yield return XOr(left, right, Position);
        }

        protected override string GetSymbol()
        {
            return "^";
        }
    }
}