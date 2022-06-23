using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Types;

public class BadClassPrototypeExpression : BadExpression
{
    public readonly BadExpression? BaseClass;
    public readonly BadExpression[] Body;
    public readonly string Name;

    public BadClassPrototypeExpression(
        string name,
        BadExpression[] body,
        BadExpression? baseClass,
        BadSourcePosition position) : base(false, false, position)
    {
        Name = name;
        Body = body;
        BaseClass = baseClass;
    }

    public override void Optimize()
    {
        for (int i = 0; i < Body.Length; i++)
        {
            Body[i] = BadExpressionOptimizer.Optimize(Body[i]);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadClassPrototype? basePrototype = null;
        if (BaseClass != null)
        {
            BadObject obj = BadObject.Null;
            foreach (BadObject o in BaseClass.Execute(context))
            {
                obj = o;

                yield return o;
            }

            obj = obj.Dereference();
            if (obj is not BadClassPrototype cls)
            {
                throw new BadRuntimeException("Base class must be a class prototype", Position);
            }

            basePrototype = cls;
        }

        BadClassPrototype p = new BadExpressionClassPrototype(Name, context.Scope, Body, basePrototype);
        context.Scope.DefineVariable(Name, p);

        yield return p;
    }
}