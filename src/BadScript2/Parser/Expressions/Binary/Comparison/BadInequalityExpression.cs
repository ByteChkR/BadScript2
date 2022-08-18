using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary.Comparison;

public class BadInequalityExpression : BadBinaryExpression
{
    public BadInequalityExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    private static BadObject NotEqual(BadObject left, BadObject right)
    {
        if (!left.Equals(right))
        {
            return BadObject.True;
        }

        return BadObject.False;
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
        BadObject right = BadObject.Null;
        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        yield return NotEqual(left, right);
    }

    protected override string GetSymbol()
    {
        return "!=";
    }
}