using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Comparison
{
    public class BadGreaterThanExpression : BadBinaryExpression
    {
        public BadGreaterThanExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
            left,
            right,
            position
        ) { }

        public static BadObject GreaterThan(BadObject left, BadObject right, BadSourcePosition pos)
        {
            if (left is IBadNumber lNum)
            {
                if (right is IBadNumber rNum)
                {
                    if (lNum.Value > rNum.Value)
                    {
                        return BadObject.True;
                    }

                    return BadObject.False;
                }
            }

            throw new BadRuntimeException($"Can not apply operator '>' to {left} and {right}", pos);
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            bool hasReturn = false;
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

            if (left is IBadNumber lNum)
            {
                if (right is IBadNumber rNum)
                {
                    hasReturn = true;

                    if (lNum.Value > rNum.Value)
                    {
                        yield return BadObject.True;
                    }
                    else
                    {
                        yield return BadObject.False;
                    }
                }
            }

            if (!hasReturn)
            {
                throw new BadRuntimeException($"Can not apply operator '{GetSymbol()}' to {left} and {right}", Position);
            }
        }

        protected override string GetSymbol()
        {
            return ">";
        }
    }
}