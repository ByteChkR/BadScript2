using BadScript2.Common;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Access
{
    public class BadNullCoalescingAssignExpression : BadBinaryExpression
    {
        public BadNullCoalescingAssignExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
            left,
            right,
            position
        ) { }


        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject left = BadObject.Null;
            foreach (BadObject o in Left.Execute(context))
            {
                left = o;
            }

            if (left is not BadObjectReference leftRef)
            {
                throw new BadRuntimeException("Left side of null coalescing assignment must be a reference", Position);
            }

            left = left.Dereference();

            if (left == BadObject.Null)
            {
                BadObject rVal = BadObject.Null;
                foreach (BadObject o in Right.Execute(context))
                {
                    yield return o;
                    rVal = o;
                }

                rVal = rVal.Dereference();
                leftRef.Set(rVal);

                yield return rVal;
            }
            else
            {
                yield return left;
            }
        }

        protected override string GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}