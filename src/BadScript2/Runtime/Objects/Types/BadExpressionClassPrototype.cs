using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Implements a Class Prototype created from Bad Expressions(e.g. Source Code)
/// </summary>
public class BadExpressionClassPrototype : BadClassPrototype
{
    /// <summary>
    ///     The Class Body(Members & Functions)
    /// </summary>
    private readonly BadExpression[] m_Body;

    /// <summary>
    ///     The Parent scope this class prototype was created in.
    /// </summary>
    private readonly BadScope m_ParentScope;

    private readonly BadScope m_StaticScope;


    /// <summary>
    ///     Creates a new BadExpressionClassPrototype
    /// </summary>
    /// <param name="name">Name of the Type</param>
    /// <param name="parentScope">The Parent scope this class prototype was created in.</param>
    /// <param name="body">The Class Body(Members & Functions)</param>
    /// <param name="baseClass">The Base class of the prototype</param>
    public BadExpressionClassPrototype(
        string name,
        BadScope parentScope,
        BadExpression[] body,
        BadClassPrototype? baseClass,
        BadInterfacePrototype[] interfaces,
        BadMetaData? meta,
        BadScope staticScope) : base(name, baseClass, interfaces, meta)
    {
        m_ParentScope = parentScope;
        m_Body = body;
        m_StaticScope = staticScope;

        //TODO: Validate interfaces before creating the class prototype? Might be faster :)
    }


    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        BadClass? baseInstance = null;
        BadExecutionContext ctx = new BadExecutionContext(
            m_ParentScope.CreateChild($"class instance {Name}", caller.Scope, true)
        );
        ctx.Scope.SetFlags(BadScopeFlags.None);

        if (BaseClass != null)
        {
            BadObject obj = Null;

            foreach (BadObject o in BaseClass.CreateInstance(caller, false))
            {
                obj = o;
            }

            if (obj is not BadClass cls)
            {
                throw new BadRuntimeException("Base class is not a class");
            }

            baseInstance = cls;
            ctx.Scope.GetTable().SetProperty("base", baseInstance, new BadPropertyInfo(BaseClass, true));
        }

        foreach (BadObject o in ctx.Execute(m_Body))
        {
            yield return o;
        }

        BadClass thisInstance = new BadClass(Name, ctx.Scope, baseInstance, this);
        ctx.Scope.ClassObject = thisInstance;

        if (setThis)
        {
            thisInstance.SetThis();

            if (Interfaces.Count != 0)
            {
                BadInterfaceValidatorResult result = thisInstance.Validate(Interfaces);

                if (!result.IsValid)
                {
                    throw new BadRuntimeException(
                        $"Class '{Name}' does not implement all required interfaces.\n{result}"
                    );
                }
            }
        }


        yield return thisInstance;
    }

    public override bool HasProperty(BadObject propName)
    {
        return m_StaticScope.HasLocal(propName) || base.HasProperty(propName);
    }

    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        return m_StaticScope.HasLocal(propName) ? m_StaticScope.GetVariable(propName, caller ?? m_ParentScope) : base.GetProperty(propName, caller);
    }
}