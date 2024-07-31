using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Expressions.Function;

/// <summary>
///     Implements the Invocation Expression
/// </summary>
public class BadInvocationExpression : BadExpression
{
    /// <summary>
    ///     The Invocation Arguments
    /// </summary>
    private readonly List<BadExpression> m_Arguments;

    /// <summary>
    ///     Constructor of the Invocation Expression
    /// </summary>
    /// <param name="left">Left Side of the Invocation</param>
    /// <param name="args">The Invocation Arguments</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadInvocationExpression(BadExpression left, IEnumerable<BadExpression> args, BadSourcePosition position) :
        base(false,
             position
            )
    {
        Left = left;
        m_Arguments = args.ToList();
    }

    /// <summary>
    ///     Argument Count of the Invocation
    /// </summary>
    public int ArgumentCount => m_Arguments.Count;

    /// <summary>
    ///     The Arguments of the Invocation
    /// </summary>
    public IEnumerable<BadExpression> Arguments => m_Arguments;

    /// <summary>
    ///     The Left side of the Invocation
    /// </summary>
    public BadExpression Left { get; }

    /// <summary>
    ///     Sets the arguments of the invocation
    /// </summary>
    /// <param name="exprs">The Arguments</param>
    public void SetArgs(IEnumerable<BadExpression> exprs)
    {
        m_Arguments.Clear();
        m_Arguments.AddRange(exprs);
    }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        for (int i = 0; i < m_Arguments.Count; i++)
        {
            m_Arguments[i] = BadConstantFoldingOptimizer.Optimize(m_Arguments[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression left in Left.GetDescendantsAndSelf())
        {
            yield return left;
        }

        foreach (BadExpression arg in m_Arguments.SelectMany(argument => argument.GetDescendantsAndSelf()))
        {
            yield return arg;
        }
    }

    /// <summary>
    ///     Returns the argument objects
    /// </summary>
    /// <param name="context">The Current Execution Context</param>
    /// <param name="args">The Arguments that will be evaluated</param>
    /// <returns>List of evaluates arguments</returns>
    public IEnumerable<BadObject> GetArgs(BadExecutionContext context, List<BadObject> args)
    {
        foreach (BadExpression argExpr in m_Arguments)
        {
            BadObject argObj = BadObject.Null;

            foreach (BadObject arg in argExpr.Execute(context))
            {
                argObj = arg;

                yield return arg;
            }

            args.Add(argObj.Dereference());
        }
    }

    /// <summary>
    ///     Invokes a function
    /// </summary>
    /// <param name="left">The Object to be invoked</param>
    /// <param name="args">The Invocation Arguments</param>
    /// <param name="position">The Source Position used to raise a BadRuntimeException</param>
    /// <param name="context">The Current Execution Context</param>
    /// <returns>List of Evaluated Objects. Last one is the Return Value of the Function</returns>
    /// <exception cref="BadRuntimeException">
    ///     Gets raised if the object is not a function or the Invocation Override Function
    ///     is not of type BadFunction
    /// </exception>
    public static IEnumerable<BadObject> Invoke(BadObject left,
                                                IEnumerable<BadObject> args,
                                                BadSourcePosition position,
                                                BadExecutionContext context)
    {
        if (left is BadFunction func)
        {
            foreach (BadObject o in func.Invoke(args.ToArray(), context))
            {
                yield return o;
            }
        }
        else if (left.HasProperty(BadStaticKeys.INVOCATION_OPERATOR_NAME, context.Scope))
        {
            if (left.GetProperty(BadStaticKeys.INVOCATION_OPERATOR_NAME, context.Scope)
                    .Dereference() is not BadFunction invocationOp)
            {
                throw new BadRuntimeException("Function Invocation Operator is not a function", position);
            }

            BadObject r = BadObject.Null;

            foreach (BadObject o in invocationOp.Invoke(args.ToArray(), context))
            {
                yield return o;

                r = o;
            }

            yield return r.Dereference();
        }
        else
        {
            throw new BadRuntimeException("Cannot invoke non-function object",
                                          position
                                         );
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();

        if (Left is IBadAccessExpression { NullChecked: true } && left.Equals(BadObject.Null))
        {
            yield return BadObject.Null;

            yield break;
        }

        if (Left is BadVariableExpression vExpr &&
            vExpr.Name == BadStaticKeys.BASE_KEY &&
            context.Scope.FunctionObject?.Name?.Text == BadStaticKeys.CONSTRUCTOR_NAME)
        {
            //Invoke Base Constructor
            left = left.GetProperty(BadStaticKeys.CONSTRUCTOR_NAME)
                       .Dereference();
        }

        List<BadObject> args = new List<BadObject>();

        foreach (BadObject o in GetArgs(context, args))
        {
            yield return o;
        }

        foreach (BadObject? o in Invoke(left, args.ToArray(), Position, context))
        {
            yield return o;
        }
    }

    /// <inheritdoc cref="BadExpression.ToString" />
    public override string ToString()
    {
        return $"({Left}({string.Join(", ", m_Arguments.Select(x => x.ToString()))}))";
    }
}