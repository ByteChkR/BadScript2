using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Access
{
    public class BadTernaryExpression : BadExpression
    {
        public BadTernaryExpression(
            BadExpression left,
            BadExpression trueRet,
            BadExpression falseRet,
            BadSourcePosition position) : base(left.IsConstant, false, position)
        {
            Left = left;
            TrueRet = trueRet;
            FalseRet = falseRet;
        }

        public BadExpression FalseRet { get; private set; }
        public BadExpression Left { get; private set; }
        public BadExpression TrueRet { get; private set; }

        public override void Optimize()
        {
            FalseRet = BadExpressionOptimizer.Optimize(FalseRet);
            Left = BadExpressionOptimizer.Optimize(Left);
            TrueRet = BadExpressionOptimizer.Optimize(TrueRet);
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject left = BadObject.Null;
            foreach (BadObject o in Left.Execute(context))
            {
                left = o;
            }

            left = left.Dereference();

            if (left is not IBadBoolean lBool)
            {
                throw new BadRuntimeException("Ternary operator requires a boolean value on the left side.", Position);
            }

            if (lBool.Value)
            {
                foreach (BadObject o in TrueRet.Execute(context))
                {
                    yield return o;
                }
            }
            else
            {
                foreach (BadObject o in FalseRet.Execute(context))
                {
                    yield return o;
                }
            }
        }

        public override string ToString()
        {
            return $"({Left} ? {TrueRet} : {FalseRet})";
        }
    }
}