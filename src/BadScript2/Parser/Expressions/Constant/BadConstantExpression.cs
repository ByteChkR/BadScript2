using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant;

public class BadConstantExpression : BadExpression, IBadNativeExpression
{
    public BadConstantExpression(BadSourcePosition position, BadObject value) : base(true, false, position)
    {
        Value = value;
    }

    private BadObject Value { get; }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public abstract class BadConstantExpression<T> : BadExpression, IBadNativeExpression
{
    protected BadConstantExpression(T value, BadSourcePosition position) : base(true, false, position)
    {
        Value = value;
    }

    public T Value { get; }

    public override string ToString()
    {
        return Value!.ToString()!;
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        yield return BadObject.Wrap(Value);
    }
}