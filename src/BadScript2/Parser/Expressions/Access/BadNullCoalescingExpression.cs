using BadScript2.Common;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Access;

public class BadNullCoalescingExpression : BadBinaryExpression
{
    public BadNullCoalescingExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        foreach (BadObject o in Left.Execute(context))
        {
            left = o;
        }

        left = left.Dereference();

        if (left == BadObject.Null)
        {
            foreach (BadObject o in Right.Execute(context))
            {
                yield return o;
            }
        }
        else
        {
            yield return left;
        }
    }

    protected override string GetSymbol()
    {
        throw new NotImplementedException();
    }
}