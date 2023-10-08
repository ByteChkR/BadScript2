using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime;

/// <summary>
///     The Execution Context.
///     Every execution of a script needs a context the script is running in.
///     It is responsible for defining the global scope and the global variables.
/// </summary>
public class BadExecutionContext
{
    /// <summary>
    ///     Creates a new Execution Context
    /// </summary>
    /// <param name="scope">The Root Scope</param>
    public BadExecutionContext(BadScope scope)
    {
        Scope = scope;
    }

    /// <summary>
    ///     The Root Scope of the Context
    /// </summary>
    public BadScope Scope { get; }

    /// <summary>
    ///     Creates a new Execution Context with an empty scope
    /// </summary>
    /// <returns>New (empty) instance.</returns>
    public static BadExecutionContext Create()
    {
        return new BadExecutionContext(new BadScope("<root>"));
    }


    /// <summary>
    ///     Executes an enumeration of expressions.
    /// </summary>
    /// <param name="expressions">Expression Enumeration</param>
    /// <returns>The Return Value. Null if no return value was set.</returns>
    public BadObject? Run(IEnumerable<BadExpression> expressions)
    {
        foreach (BadObject o in Execute(expressions))
        {
            //Execute
        }

        if (Scope.ReturnValue != null)
        {
            return Scope.ReturnValue;
        }

        return null;
    }

    public BadObject ExecuteScript(IEnumerable<BadExpression> expressions)
    {
        BadObject result = BadObject.Null;

        foreach (BadObject o in Execute(expressions))
        {
            result = o;
        }

        return result;
    }


    public BadObject ExecuteScript(BadExpression expression)
    {
        BadObject result = BadObject.Null;

        foreach (BadObject o in Execute(expression))
        {
            result = o;
        }

        return result;
    }

    /// <summary>
    ///     Executes an enumeration of expressions.
    /// </summary>
    /// <param name="expressions">Expression Enumeration</param>
    /// <returns>Enumeration of the resulting objects</returns>
    public IEnumerable<BadObject> Execute(IEnumerable<BadExpression> expressions)
    {
        foreach (BadExpression expression in expressions)
        {
            foreach (BadObject o in Execute(expression))
            {
                yield return o;
            }
        }
    }

    public IEnumerable<BadObject> Execute(BadExpression expression)
    {
        foreach (BadObject o in expression.Execute(this))
        {
            yield return o;

            if (Scope.ReturnValue != null ||
                Scope.IsBreak ||
                Scope.IsContinue ||
                Scope.IsError)
            {
                yield break;
            }
        }
    }
}