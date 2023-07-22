using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary;

public class BadInExpression : BadBinaryExpression
{
    public BadInExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left, right, position) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        BadObject right = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        foreach (BadObject? o in InWithOverride(context, left, right, Position))
        {
            yield return o;
        }
    }

    public static BadObject In(BadObject left, BadObject right)
    {
        return right.HasProperty(left);
    }

    public static IEnumerable<BadObject> InWithOverride(BadExecutionContext context, BadObject left, BadObject right, BadSourcePosition position)
    {
        if (right.HasProperty(BadStaticKeys.InOperatorName))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         right,
                         left,
                         context,
                         BadStaticKeys.InOperatorName,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return In(left, right);
        }
    }

    protected override string GetSymbol()
    {
        return "in";
    }
}