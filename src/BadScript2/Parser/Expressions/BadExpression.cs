using BadScript2.Common;
using BadScript2.Debugging;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Expressions
{
    /// <summary>
    ///     Base Implementation for all Expressions used inside the Script
    /// </summary>
    public abstract class BadExpression
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="isConstant">Indicates if the expression stays constant at all times</param>
        /// <param name="position">The source Position of the Expression</param>
        protected BadExpression(bool isConstant, BadSourcePosition position)
        {
            IsConstant = isConstant;
            Position = position;
        }

        /// <summary>
        ///     Indicates if the expression stays constant at all times.
        /// </summary>
        public bool IsConstant { get; }

        /// <summary>
        ///     The source Position of the Expression
        /// </summary>
        public BadSourcePosition Position { get; }

        /// <summary>
        ///     Optimizes the Expression
        /// </summary>
        public virtual void Optimize() { }

        /// <summary>
        ///     Is used to evaluate the Expression
        /// </summary>
        /// <param name="context">The current Execution context the expression is evaluated in</param>
        /// <returns>Enumerable of BadObject. The Last element returned is the result of the current expression.</returns>
        protected abstract IEnumerable<BadObject> InnerExecute(BadExecutionContext context);

        /// <summary>
        ///     Evaluates the Expression within the current Execution Context.
        /// </summary>
        /// <param name="context">The current Execution context the expression is evaluated in</param>
        /// <returns>Enumerable of BadObject. The Last element returned is the result of the current expression.</returns>
        public IEnumerable<BadObject> Execute(BadExecutionContext context)
        {
            if (BadDebugger.IsAttached)
            {
                BadDebugger.Step(new BadDebuggerStep(context, Position, this));
            }

            foreach (BadObject o in InnerExecute(context))
            {
                yield return o;
            }
        }

        /// <summary>
        ///     Helper function that executes an operator override function if implemented.
        /// </summary>
        /// <param name="left">Left Expression Part</param>
        /// <param name="right">Right Expression Part</param>
        /// <param name="context">The current Execution context the expression is evaluated in</param>
        /// <param name="name">The name of the operator override function</param>
        /// <returns>Enumerable of BadObject. The Last element returned is the result of the current expression.</returns>
        /// <exception cref="BadRuntimeException">Gets thrown if the override function does not exist or is not of type BadFunction</exception>
        protected IEnumerable<BadObject> ExecuteOperatorOverride(
            BadObject left,
            BadObject right,
            BadExecutionContext context,
            string name)
        {
            BadFunction? func = left.GetProperty(name).Dereference() as BadFunction;

            if (func == null)
            {
                throw new BadRuntimeException(
                    $"{left.GetType().Name} has no {name} property",
                    Position
                );
            }

            foreach (BadObject o in func.Invoke(new[] { right }, context))
            {
                yield return o;
            }
        }
    }
}