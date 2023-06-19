using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

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

        yield return right.HasProperty(left);
    }

    protected override string GetSymbol()
    {
        return "in";
    }
}