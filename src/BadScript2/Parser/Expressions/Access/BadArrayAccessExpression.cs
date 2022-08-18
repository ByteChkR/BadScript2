using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Expressions.Access;


public class BadArrayAccessReverseExpression : BadExpression
{
    public readonly BadExpression[] Arguments;
    public readonly bool NullChecked;

    public BadArrayAccessReverseExpression(
        BadExpression left,
        BadExpression[] args,
        BadSourcePosition position,
        bool nullChecked = false) : base(
        false,
        true,
        position
    )
    {
        Left = left;
        Arguments = args;
        NullChecked = nullChecked;
    }

    public BadExpression Left { get; private set; }

    public override void Optimize()
    {
        Left = BadExpressionOptimizer.Optimize(Left);

        for (int i = 0; i < Arguments.Length; i++)
        {
            Arguments[i] = BadExpressionOptimizer.Optimize(Arguments[i]);
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
        foreach (BadExpression argExpr in Arguments)
        {
            BadObject argObj = BadObject.Null;
            foreach (BadObject arg in argExpr.Execute(context))
            {
                argObj = arg;

                yield return arg;
            }

            args.Add(argObj);
        }

        if (left.HasProperty(BadStaticKeys.ArrayAccessReverseOperatorName))
        {
            BadFunction? func = left.GetProperty(BadStaticKeys.ArrayAccessReverseOperatorName).Dereference() as BadFunction;
            if (func == null)
            {
                throw new BadRuntimeException("Array access reverse operator is not a function", Position);
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
            throw new BadRuntimeException("Array access reverse operator is not defined", Position);
        }
    }
}

public class BadArrayAccessExpression : BadExpression
{
    public readonly BadExpression[] Arguments;
    public readonly bool NullChecked;

    public BadArrayAccessExpression(
        BadExpression left,
        BadExpression[] args,
        BadSourcePosition position,
        bool nullChecked = false) : base(
        false,
        true,
        position
    )
    {
        Left = left;
        Arguments = args;
        NullChecked = nullChecked;
    }

    public BadExpression Left { get; private set; }

    public override void Optimize()
    {
        Left = BadExpressionOptimizer.Optimize(Left);

        for (int i = 0; i < Arguments.Length; i++)
        {
            Arguments[i] = BadExpressionOptimizer.Optimize(Arguments[i]);
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
        foreach (BadExpression argExpr in Arguments)
        {
            BadObject argObj = BadObject.Null;
            foreach (BadObject arg in argExpr.Execute(context))
            {
                argObj = arg;

                yield return arg;
            }

            args.Add(argObj);
        }

        if (left.HasProperty(BadStaticKeys.ArrayAccessOperatorName))
        {
            BadFunction? func = left.GetProperty(BadStaticKeys.ArrayAccessOperatorName).Dereference() as BadFunction;
            if (func == null)
            {
                throw new BadRuntimeException("Array access operator is not a function", Position);
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
            throw new BadRuntimeException("Array access operator is not defined", Position);
        }
    }
}