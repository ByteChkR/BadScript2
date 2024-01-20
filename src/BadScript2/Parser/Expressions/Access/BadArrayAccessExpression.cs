using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
/// <summary>
/// Contains the Access Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions.Access;

/// <summary>
///     Implements the Array Access to set or get properties from an object.
///     LEFT[RIGHT]
/// </summary>
public class BadArrayAccessExpression : BadExpression, IBadAccessExpression
{
    /// <summary>
    ///     Arguments of the array access.
    /// </summary>
    private readonly BadExpression[] m_Arguments;

    /// <summary>
    ///     Constructor of the Array Access Expression
    /// </summary>
    /// <param name="left">Left side of the expression</param>
    /// <param name="args">Right side of the expression</param>
    /// <param name="position">Position inside the source code</param>
    /// <param name="nullChecked">Indicates if the expression will be null-checked by the runtime</param>
    public BadArrayAccessExpression(
        BadExpression left,
        BadExpression[] args,
        BadSourcePosition position,
        bool nullChecked = false) : base(
        false,
        position
    )
    {
        Left = left;
        m_Arguments = args;
        NullChecked = nullChecked;
    }

    /// <summary>
    ///     The count of the right side arguments.
    /// </summary>
    public int ArgumentCount => m_Arguments.Length;

    /// <summary>
    ///     The Arguments of the Array Access.
    /// </summary>
    public IEnumerable<BadExpression> Arguments => m_Arguments;

    /// <summary>
    ///     Left side of the expression.
    /// </summary>
    public BadExpression Left { get; private set; }

    /// <summary>
    ///     Indicates if the expression will be null-checked by the runtime
    /// </summary>
    public bool NullChecked { get; }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression expression in Left.GetDescendantsAndSelf())
        {
            yield return expression;
        }

        foreach (BadExpression expression in m_Arguments)
        {
            foreach (BadExpression descendant in expression.GetDescendantsAndSelf())
            {
                yield return descendant;
            }
        }
    }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        Left = BadConstantFoldingOptimizer.Optimize(Left);

        for (int i = 0; i < m_Arguments.Length; i++)
        {
            m_Arguments[i] = BadConstantFoldingOptimizer.Optimize(m_Arguments[i]);
        }
    }

    /// <summary>
    /// Executes the Array Access Expression.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <param name="left">Left side of the expression.</param>
    /// <param name="args">Right side of the expression.</param>
    /// <param name="position">Position inside the source code.</param>
    /// <returns>The result of the array access(last item of the enumeration).</returns>
    /// <exception cref="BadRuntimeException">If the array access operator is not defined.</exception>
    public static IEnumerable<BadObject> Access(
        BadExecutionContext? context,
        BadObject left,
        IEnumerable<BadObject> args,
        BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME, context?.Scope))
        {
            if (left.GetProperty(BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME, context?.Scope).Dereference() is not BadFunction func)
            {
                throw new BadRuntimeException("Array access operator is not a function", position);
            }

            BadObject r = BadObject.Null;

            foreach (BadObject o in func.Invoke(args.ToArray(), context!))
            {
                yield return o;

                r = o;
            }

            yield return r;
        }
        else
        {
            throw new BadRuntimeException("Array access operator is not defined", position);
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

        if (NullChecked && left == BadObject.Null)
        {
            yield return left;

            yield break;
        }

        List<BadObject> args = new List<BadObject>();

        foreach (BadExpression argExpr in m_Arguments)
        {
            BadObject argObj = BadObject.Null;

            foreach (BadObject arg in argExpr.Execute(context))
            {
                argObj = arg;

                yield return arg;
            }

            args.Add(argObj);
        }

        foreach (BadObject o in Access(context, left, args.ToArray(), Position))
        {
            yield return o;
        }
    }
}