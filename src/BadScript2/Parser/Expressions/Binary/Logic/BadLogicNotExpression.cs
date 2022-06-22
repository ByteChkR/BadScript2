using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Logic
{
    public class BadLogicNotExpression : BadExpression
    {
        public BadLogicNotExpression(BadExpression right, BadSourcePosition position) : base(right.IsConstant, false, position)
        {
            Right = right;
        }

        public BadExpression Right { get; }

        public static BadObject Not(BadObject left, BadSourcePosition pos)
        {
            if (left is IBadBoolean rBool)
            {
                return rBool.Value ? BadObject.False : BadObject.True;
            }

            throw new BadRuntimeException(
                $"Cannot apply '!' to object '{left}'",
                pos
            );
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject r = BadObject.Null;
            foreach (BadObject o in Right.Execute(context))
            {
                r = o;
            }

            r = r.Dereference();

            yield return Not(r, Position);
        }
    }
}