using BadScript2.Common;
using BadScript2.Common.Logging;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;

namespace BadScript2.Parser.Expressions.Variables;

public class BadVariableExpression : BadExpression
{

    public BadVariableExpression(string name, BadSourcePosition position) : base(false, position)
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
       
        BadObject name = BadObject.Wrap(Name);

        BadObjectReference obj = context.Scope.GetVariable(name);
       

        yield return obj;
    }
}