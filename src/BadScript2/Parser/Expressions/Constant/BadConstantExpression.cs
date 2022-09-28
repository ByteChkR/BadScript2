using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant
{
    /// <summary>
    ///     Base Class of all Constant Expressions
    /// </summary>
    public class BadConstantExpression : BadExpression, IBadNativeExpression
    {
        /// <summary>
        ///     Constructor of the Constant Expression
        /// </summary>
        /// <param name="position">Source Position of the Expression</param>
        /// <param name="value">The Constant Value of the Expression</param>
        public BadConstantExpression(BadSourcePosition position, BadObject value) : base(true, position)
        {
            Value = value;
        }

        /// <summary>
        ///     The Constant Value of the Expression
        /// </summary>
        private BadObject Value { get; }

        /// <summary>
        ///     Private Implementation of IBadNativeExpression.Value
        /// </summary>
        object IBadNativeExpression.Value => Value;

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            yield return Value;
        }

        /// <summary>
        ///     String Representation of the Expression
        /// </summary>
        /// <returns>String Representation</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    ///     Base class of all Constant Expressions
    /// </summary>
    /// <typeparam name="T">Type of the Constant Value</typeparam>
    public abstract class BadConstantExpression<T> : BadExpression, IBadNativeExpression
    {
        /// <summary>
        ///     Constructor of the Constant Expression
        /// </summary>
        /// <param name="value">Constant Value of the Expression</param>
        /// <param name="position">Source Position of the Expression</param>
        protected BadConstantExpression(T value, BadSourcePosition position) : base(true, position)
        {
            Value = value;
        }


        /// <summary>
        ///     The Raw Valud of the Expression
        /// </summary>
        public T Value { get; }

        /// <summary>
        ///     Private implementation of IBadNativeExpression.Value
        /// </summary>
        object IBadNativeExpression.Value => Value!;


        /// <summary>
        ///     String Representation of the Expression
        /// </summary>
        /// <returns>String Representation of the Expression</returns>
        public override string ToString()
        {
            return Value!.ToString()!;
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            yield return BadObject.Wrap(Value);
        }
    }
}