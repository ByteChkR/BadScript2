using System.Runtime.ExceptionServices;

using BadScript2.Common;
using BadScript2.Debugging;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Settings;

namespace BadScript2.Parser.Expressions;

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
    public BadSourcePosition Position { get; private set; }

    /// <summary>
    ///     Uses the Constant Folding Optimizer to optimize the expression
    /// </summary>
    public virtual void Optimize() { }

    /// <summary>
    ///     Returns all Descendants of the Expression
    /// </summary>
    /// <returns>Enumeration of all Descendants</returns>
    public abstract IEnumerable<BadExpression> GetDescendants();

    /// <summary>
    ///     Returns all Descendants of the Expression and the Expression itself
    /// </summary>
    /// <returns>Enumeration of all Descendants and the Expression itself</returns>
    public IEnumerable<BadExpression> GetDescendantsAndSelf()
    {
        yield return this;

        foreach (BadExpression? descendant in GetDescendants())
        {
            yield return descendant;
        }
    }

    /// <summary>
    ///     Sets the Source Position of the Expression
    /// </summary>
    /// <param name="pos"></param>
    public void SetPosition(BadSourcePosition pos)
    {
        Position = pos;
    }

    /// <summary>
    ///     Is used to evaluate the Expression
    /// </summary>
    /// <param name="context">The current Execution context the expression is evaluated in</param>
    /// <returns>Enumerable of BadObject. The Last element returned is the result of the current expression.</returns>
    protected abstract IEnumerable<BadObject> InnerExecute(BadExecutionContext context);

    private IEnumerable<BadObject> ExecuteWithCatch(BadExecutionContext context)
    {
        using IEnumerator<BadObject> e = InnerExecute(context).GetEnumerator();

        while (true)
        {
            try
            {
                if (!e.MoveNext())
                {
                    break;
                }
            }
            catch (BadRuntimeErrorException err)
            {
                ExceptionDispatchInfo.Capture(err).Throw();
            }
            catch (Exception exception)
            {
                throw new BadRuntimeErrorException(BadRuntimeError.FromException(exception, context.Scope.GetStackTrace()));
            }

            yield return e.Current ?? BadObject.Null;
        }
    }
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

        if (BadRuntimeSettings.Instance.CatchRuntimeExceptions)
        {
            return ExecuteWithCatch(context);
        }
        return InnerExecute(context);
    }

    /// <summary>
    ///     Helper function that executes an operator override function if implemented.
    /// </summary>
    /// <param name="left">Left Expression Part</param>
    /// <param name="right">Right Expression Part</param>
    /// <param name="context">The current Execution context the expression is evaluated in</param>
    /// <param name="name">The name of the operator override function</param>
    /// <param name="position">The Source Position used when throwing an error</param>
    /// <returns>Enumerable of BadObject. The Last element returned is the result of the current expression.</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the override function does not exist or is not of type BadFunction</exception>
    protected static IEnumerable<BadObject> ExecuteOperatorOverride(
        BadObject left,
        BadObject right,
        BadExecutionContext context,
        string name,
        BadSourcePosition position)
    {
        if (left.GetProperty(name, context.Scope).Dereference() is not BadFunction func)
        {
            throw new BadRuntimeException(
                $"{left.GetType().Name} has no {name} property",
                position
            );
        }

        foreach (BadObject o in func.Invoke(
                     new[]
                     {
                         right,
                     },
                     context
                 ))
        {
            yield return o;
        }
    }

    /// <summary>
    ///     Executes an operator override function if implemented.
    /// </summary>
    /// <param name="left">Left Expression Part</param>
    /// <param name="context">The current Execution context the expression is evaluated in</param>
    /// <param name="name">The name of the operator override function</param>
    /// <param name="position">The Source Position used when throwing an error</param>
    /// <returns>Result of the operator override function</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the override function does not exist or is not of type BadFunction</exception>
    protected static IEnumerable<BadObject> ExecuteOperatorOverride(
        BadObject left,
        BadExecutionContext context,
        string name,
        BadSourcePosition position)
    {
        if (left.GetProperty(name, context.Scope).Dereference() is not BadFunction func)
        {
            throw new BadRuntimeException(
                $"{left.GetType().Name} has no {name} property",
                position
            );
        }

        foreach (BadObject o in func.Invoke(Array.Empty<BadObject>(), context))
        {
            yield return o;
        }
    }
}