using System.Runtime.ExceptionServices;

using BadScript2.Common;
using BadScript2.Debugging;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
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
        Attributes = [];
    }

    public IEnumerable<BadExpression> Attributes { get; private set; }

    /// <summary>
    ///     Indicates if the expression stays constant at all times.
    /// </summary>
    public bool IsConstant { get; }

    /// <summary>
    ///     The source Position of the Expression
    /// </summary>
    public BadSourcePosition Position { get; private set; }

    public void SetAttributes(IEnumerable<BadExpression> attributes)
    {
        Attributes = attributes;
    }
    protected IEnumerable<BadObject> ComputeAttributes(BadExecutionContext ctx, List<BadObject> attributes)
    {
        foreach (BadExpression? attribute in Attributes)
        {
            BadObject? obj = BadObject.Null;
            BadExpression? attrib = null;
            BadExpression access;
            IEnumerable<BadExpression> args;

            if (attribute is BadInvocationExpression invoc)
            {
                access = invoc.Left;
                args = invoc.Arguments;
            }
            else if (attribute is BadMemberAccessExpression || attribute is BadVariableExpression)
            {
                access = attribute;
                args = Array.Empty<BadExpression>();
            }
            else
            {
                throw BadRuntimeException.Create(ctx.Scope, "Attribute Expression not Supported.", attribute.Position);
            }

            BadClassPrototype? attribClass = null;
            if (access is BadVariableExpression varExpr)
            {
                //Check if the variable exists and is a class.
                if (ctx.Scope.HasVariable(varExpr.Name, ctx.Scope)) // Try to get the variable
                {
                    BadObject? attribObj = ctx.Scope.GetVariable(varExpr.Name, ctx.Scope).Dereference();

                    //Check if the variable is a class and inherits from IAttribute
                    if (attribObj is BadClassPrototype cls && BadNativeClassBuilder.Attribute.IsSuperClassOf(cls))
                    {
                        attribClass = cls;
                    }
                }

                //If the variable does not exist, check if the variable name + "Attribute" exists and is a class.
                if (attribClass == null && !varExpr.Name.EndsWith("Attribute") && ctx.Scope.HasVariable(varExpr.Name + "Attribute", ctx.Scope))
                {
                    BadObject? attribObj = ctx.Scope.GetVariable(varExpr.Name + "Attribute", ctx.Scope).Dereference();


                    //Check if the variable is a class and inherits from IAttribute
                    if (attribObj is BadClassPrototype cls && BadNativeClassBuilder.Attribute.IsSuperClassOf(cls))
                    {
                        attribClass = cls;
                    }
                }
            }
            else if (access is BadMemberAccessExpression mac)
            {
                //evaluate left side of the member access
                foreach (BadObject? o in mac.Left.Execute(ctx))
                {
                    obj = o;
                }
                BadObject? parent = obj.Dereference();
                //Check if parent has property
                if (parent.HasProperty(mac.Right.Text, ctx.Scope))
                {
                    BadObject? attribObj = parent.GetProperty(mac.Right.Text, ctx.Scope).Dereference();
                    //Check if the property is a class and inherits from IAttribute
                    if (attribObj is BadClassPrototype cls && BadNativeClassBuilder.Attribute.IsSuperClassOf(cls))
                    {
                        attribClass = cls;
                    }
                }
                if (parent.HasProperty(mac.Right.Text + "Attribute", ctx.Scope))
                {
                    BadObject? attribObj = parent.GetProperty(mac.Right.Text + "Attribute", ctx.Scope).Dereference();
                    //Check if the property is a class and inherits from IAttribute
                    if (attribObj is BadClassPrototype cls && BadNativeClassBuilder.Attribute.IsSuperClassOf(cls))
                    {
                        attribClass = cls;
                    }
                }
            }

            if (attribClass == null)
            {
                throw BadRuntimeException.Create(ctx.Scope, "Attribute must be a class", attribute.Position);
            }

            attrib = new BadNewExpression(
                new BadInvocationExpression(
                    new BadConstantExpression(attribute.Position, attribClass),
                    args,
                    attribute.Position
                ),
                attribute.Position
            );

            foreach (BadObject? o in attrib.Execute(ctx))
            {
                yield return o;
                obj = o;
            }

            BadObject? a = obj.Dereference();
            if (a is not BadClass c)
            {
                throw BadRuntimeException.Create(ctx.Scope, "Attribute must be a class", attrib.Position);
            }

            if (!c.InheritsFrom(BadNativeClassBuilder.Attribute))
            {
                throw BadRuntimeException.Create(ctx.Scope, "Attribute must inherit from IAttribute", attrib.Position);
            }
            attributes.Add(a);
        }
    }

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