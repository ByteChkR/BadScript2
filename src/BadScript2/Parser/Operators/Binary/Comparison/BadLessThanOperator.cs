using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Parser.Operators.Binary.Comparison
{
    /// <summary>
    ///     Implements the Less Than Operator
    /// </summary>
    public class BadLessThanOperator : BadBinaryOperator
    {
        /// <summary>
        ///     Constructor of the Operator
        /// </summary>
        public BadLessThanOperator() : base(8, "<") { }

        public override BadExpression Parse(BadExpression left, BadSourceParser parser)
        {
            BadExpression right = parser.ParseExpression(null, Precedence);

            return new BadLessThanExpression(left, right, left.Position.Combine(right.Position));
        }
    }
}