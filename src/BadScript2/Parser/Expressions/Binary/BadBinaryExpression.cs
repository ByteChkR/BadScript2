using BadScript2.Common;
using BadScript2.Optimizations;

namespace BadScript2.Parser.Expressions.Binary
{
    public abstract class BadBinaryExpression : BadExpression
    {
        protected BadBinaryExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
            left.IsConstant && right.IsConstant,
            false,
            position
        )
        {
            Left = left;
            Right = right;
        }

        public BadExpression Left { get; private set; }
        public BadExpression Right { get; private set; }

        public override void Optimize()
        {
            Left = BadExpressionOptimizer.Optimize(Left);
            Right = BadExpressionOptimizer.Optimize(Right);
        }

        protected abstract string GetSymbol();

        public override string ToString()
        {
            return $"({Left} {GetSymbol()} {Right})";
        }
    }
}