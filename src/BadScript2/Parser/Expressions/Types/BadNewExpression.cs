using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Types;

/// <summary>
///     Implements the New Expression
/// </summary>
public class BadNewExpression : BadExpression
{
    /// <summary>
    ///     Constructor of the New Expression
    /// </summary>
    /// <param name="right">The Expression that evaluates to a BadClassPrototype that can be created</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadNewExpression(BadInvocationExpression right, BadSourcePosition position) : base(false, position)
    {
        Right = right;
    }

    /// <summary>
    ///     The Constructor Invocation
    /// </summary>
    public BadInvocationExpression Right { get; }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        Right.Optimize();
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Right.GetDescendantsAndSelf();
    }

    /// <summary>
    ///     Creates an Instance of the Specified Class prototype
    /// </summary>
    /// <param name="proto">The Class prototype</param>
    /// <param name="context">The Current Execution Context</param>
    /// <param name="args">The Constructor Arguments</param>
    /// <param name="pos">The Source Position that gets used to Raise an Exception</param>
    /// <returns>The Created Instance of the Class</returns>
    /// <exception cref="BadRuntimeException">
    ///     Gets Raised if the return of the class prototype is not an Instance of BadClass
    ///     or the Constructor of the class is not a BadFunction
    /// </exception>
    public static IEnumerable<BadObject> CreateObject(BadClassPrototype proto,
                                                      BadExecutionContext context,
                                                      BadObject[] args,
                                                      BadSourcePosition pos)
    {
        BadObject obj = BadObject.Null;

        if (proto == BadVoidPrototype.Instance)
        {
            throw BadRuntimeException.Create(context.Scope, "Cannot create instance of void", pos);
        }

        if (proto is BadANativeClassPrototype nativeType)
        {
            foreach (BadObject o in nativeType.CreateInstance(context, args))
            {
                yield return o;
            }

            yield break;
        }

        foreach (BadObject o in proto.CreateInstance(context))
        {
            obj = o;

            yield return o;
        }

        obj = obj.Dereference(pos);

        if (obj is not BadClass cls)
        {
            throw new BadRuntimeException("Cannot create object from non-class type", pos);
        }

        //Call Constructor if exists

        if (cls.HasProperty(BadStaticKeys.CONSTRUCTOR_NAME, cls.Scope))
        {
            BadObject ctor = cls.GetProperty(BadStaticKeys.CONSTRUCTOR_NAME, context.Scope)
                                .Dereference(pos);

            if (ctor is not BadFunction func)
            {
                throw new BadRuntimeException("Cannot create object from non-function type", pos);
            }

            foreach (BadObject o in func.Invoke(args, context))
            {
                yield return o;
            }
        }

        foreach (BadObject o in cls.Scope.InitializeAttributes())
        {
            yield return o;
        }

        yield return cls;
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject obj = BadObject.Null;

        //Get Type from Right
        foreach (BadObject o in Right.Left.Execute(context))
        {
            obj = o;

            yield return o;
        }

        obj = obj.Dereference(Position);

        if (obj is not BadClassPrototype ptype)
        {
            throw new BadRuntimeException("Cannot create object from non-class type", Position);
        }

        List<BadObject> args = new List<BadObject>();

        foreach (BadObject o in Right.GetArgs(context, args))
        {
            yield return o;
        }

        foreach (BadObject o in CreateObject(ptype, context, args.ToArray(), Position))
        {
            yield return o;
        }
    }
}