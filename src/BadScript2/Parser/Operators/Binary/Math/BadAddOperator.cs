using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Parser.Operators.Binary.Math
{
    /// <summary>
    ///     Implements the Add Operator
    /// </summary>
    public class BadAddOperator : BadBinaryOperator
    {
        /// <summary>
        ///     Constructor of the Operator
        /// </summary>
        public BadAddOperator() : base(6, "+") { }

        public override BadExpression Parse(BadExpression left, BadSourceParser parser)
        {
            BadExpression right = parser.ParseExpression(null, Precedence);

            return new BadAddExpression(left, right, left.Position.Combine(right.Position));
        }
    }
}