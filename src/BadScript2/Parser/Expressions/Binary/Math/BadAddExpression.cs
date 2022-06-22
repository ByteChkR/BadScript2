using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math
{
    public class BadAddExpression : BadBinaryExpression
    {
        public BadAddExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
            left,
            right,
            position
        ) { }

        protected override string GetSymbol()
        {
            return "+";
        }

        public static BadObject Add(BadObject left, BadObject right, BadSourcePosition pos)
        {
            if (left is IBadString lStr)
            {
                if (right is IBadNative rNative)
                {
                    return BadObject.Wrap(lStr.Value + rNative.Value);
                }

                return BadObject.Wrap(lStr.Value + right);
            }

            if (left is IBadNumber lNum)
            {
                if (right is IBadString rStr)
                {
                    return BadObject.Wrap(lNum.Value + rStr.Value);
                }

                if (right is IBadNumber rNum)
                {
                    return BadObject.Wrap(lNum.Value + rNum.Value);
                }
            }
            else if (left is IBadBoolean lBool)
            {
                if (right is IBadString rStr)
                {
                    return BadObject.Wrap(lBool.Value + rStr.Value);
                }
            }
            else if (right is IBadString rStr)
            {
                return BadObject.Wrap(left + rStr.Value);
            }

            throw new BadRuntimeException($"Can not apply operator '+' to {left} and {right}", pos);
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

            yield return Add(left, right, Position);
        }
    }
}