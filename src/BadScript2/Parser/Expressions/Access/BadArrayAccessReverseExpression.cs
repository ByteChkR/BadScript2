using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Expressions.Access;

/// <summary>
///     Implements the Reverse Array Access to set or get properties from an object.
///     <Left>[^<Right>]
/// </summary>
public class BadArrayAccessReverseExpression : BadExpression, IBadAccessExpression
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
    public BadArrayAccessReverseExpression(
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

    public IEnumerable<BadExpression> Arguments => m_Arguments;

    public int ArgumentCount => m_Arguments.Length;

    /// <summary>
    ///     Left side of the Expression
    /// </summary>
    public BadExpression Left { get; set; }

    /// <summary>
    ///     Indicates if the expression will be null-checked by the runtime
    /// </summary>
    public bool NullChecked { get; }

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


    public override void Optimize()
    {
        Left = BadConstantFoldingOptimizer.Optimize(Left);

        for (int i = 0; i < m_Arguments.Length; i++)
        {
            m_Arguments[i] = BadConstantFoldingOptimizer.Optimize(m_Arguments[i]);
        }
    }

    public static IEnumerable<BadObject> Access(
        BadExecutionContext context,
        BadObject left,
        BadObject[] args,
        BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.ArrayAccessReverseOperatorName))
        {
            BadFunction? func =
                left.GetProperty(BadStaticKeys.ArrayAccessReverseOperatorName, context.Scope)
                    .Dereference() as BadFunction;

            if (func == null)
            {
                throw new BadRuntimeException("Array access reverse operator is not a function", position);
            }

            BadObject r = BadObject.Null;

            foreach (BadObject o in func.Invoke(args.ToArray(), context))
            {
                yield return o;

                r = o;
            }

            yield return r;
        }
        else
        {
            throw new BadRuntimeException("Array access reverse operator is not defined", position);
        }
    }

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