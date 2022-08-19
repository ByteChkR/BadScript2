using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Types;

public class BadClassPrototypeExpression : BadExpression
{
    private readonly BadExpression? m_BaseClass;
    private readonly BadExpression[] m_Body;
    private readonly string m_Name;

    public BadClassPrototypeExpression(
        string name,
        BadExpression[] body,
        BadExpression? baseClass,
        BadSourcePosition position) : base(false, position)
    {
        m_Name = name;
        m_Body = body;
        m_BaseClass = baseClass;
    }

    public override void Optimize()
    {
        for (int i = 0; i < m_Body.Length; i++)
        {
            m_Body[i] = BadExpressionOptimizer.Optimize(m_Body[i]);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadClassPrototype? basePrototype = null;
        if (m_BaseClass != null)
        {
            BadObject obj = BadObject.Null;
            foreach (BadObject o in m_BaseClass.Execute(context))
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

        BadClassPrototype p = new BadExpressionClassPrototype(m_Name, context.Scope, m_Body, basePrototype);
        context.Scope.DefineVariable(m_Name, p);

        yield return p;
    }
}