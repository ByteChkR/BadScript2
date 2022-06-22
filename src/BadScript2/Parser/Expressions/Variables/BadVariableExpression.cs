using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Variables;

public class BadVariableExpression : BadExpression
{
    public BadVariableExpression(string name, BadSourcePosition position) : base(false, true, position)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString()
    {
        return Name;
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        yield return context.Scope.GetVariable(BadObject.Wrap(Name));
    }
}