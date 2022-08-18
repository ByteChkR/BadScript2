using BadScript2.Common;
using BadScript2.Debugging;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Expressions;

public abstract class BadExpression
{
    protected BadExpression(bool isConstant, bool isLValue, BadSourcePosition position)
    {
        IsConstant = isConstant;
        Position = position;
    }

    public bool IsConstant { get; }

    public BadSourcePosition Position { get; }

    public virtual void Optimize() { }

    protected abstract IEnumerable<BadObject> InnerExecute(BadExecutionContext context);

    public IEnumerable<BadObject> Execute(BadExecutionContext context)
    {
        if (BadDebugger.IsAttached)
        {
            BadDebugger.Step(new BadDebuggerStep(context, Position, this));
        }

        foreach (BadObject o in InnerExecute(context))
        {
            yield return o;
        }
    }

    protected IEnumerable<BadObject> ExecuteOperatorOverride(
        BadObject left,
        BadObject right,
        BadExecutionContext context,
        string name)
    {
        BadFunction? func = left.GetProperty(name).Dereference() as BadFunction;

        if (func == null)
        {
            throw new BadRuntimeException(
                $"{left.GetType().Name} has no {name} property",
                Position
            );
        }

        foreach (BadObject o in func.Invoke(new[] { right }, context))
        {
            yield return o;
        }
    }
}