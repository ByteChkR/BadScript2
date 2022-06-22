using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Logic.Assign
{
    public class BadLogicAssignAndExpression : BadBinaryExpression
    {
        public BadLogicAssignAndExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
            left,
            right,
            position
        ) { }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            bool hasReturn = false;
            BadObject left = BadObject.Null;
            foreach (BadObject o in Left.Execute(context))
            {
                left = o;

                yield return o;
            }

            if (left is not BadObjectReference leftRef)
            {
                throw new BadRuntimeException($"Left side of {GetSymbol()} must be a reference", Position);
            }

            left = left.Dereference();

            BadObject right = BadObject.Null;
            foreach (BadObject o in Right.Execute(context))
            {
                right = o;

                yield return o;
            }

            right = right.Dereference();

            if (left is IBadBoolean lBool && right is IBadBoolean rBool)
            {
                hasReturn = true;

                BadObject r = BadObject.Wrap(lBool.Value && rBool.Value);
                leftRef.Set(r);

                yield return r;
            }

            if (!hasReturn)
            {
                throw new BadRuntimeException($"Can not apply operator '{GetSymbol()}' to {left} and {right}", Position);
            }
        }

        protected override string GetSymbol()
        {
            return "&=";
        }
    }
}