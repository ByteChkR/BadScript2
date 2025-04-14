using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Implements a Class Prototype created from Bad Expressions(e.g. Source Code)
/// </summary>
public class BadExpressionClassPrototype : BadClassPrototype, IBadGenericObject
{
    private readonly Func<BadObject[], BadClassPrototype?> m_BaseClassFunc;

    /// <summary>
    ///     The Class Body(Members & Functions)
    /// </summary>
    private readonly BadExpression[] m_Body;

    /// <summary>
    /// The Generic Definition of this Class Prototype if it is a Generic Type
    /// </summary>
    private readonly BadExpressionClassPrototype? m_GenericDefinition;

    /// <summary>
    /// The Factory for the Implemented Interfaces of the Class
    /// </summary>
    private readonly Func<BadObject[], BadInterfacePrototype[]> m_InterfacesFunc;

    /// <summary>
    ///     The Parent scope this class prototype was created in.
    /// </summary>
    private readonly BadScope m_ParentScope;

    /// <summary>
    /// The Factory for the Static Scope of the Class
    /// </summary>
    private readonly Func<BadObject[], BadScope> m_StaticScope;

    /// <summary>
    /// Cache for Generic Instances
    /// </summary>
    private readonly Dictionary<int, BadExpressionClassPrototype> s_GenericCache =
        new Dictionary<int, BadExpressionClassPrototype>();

    /// <summary>
    /// Cache for the Base Class of the Class Prototype
    /// </summary>
    private BadClassPrototype? m_BaseClassCache;
    /// <summary>
    /// Cache for the Implemented Interfaces of the Class Prototype
    /// </summary>
    private BadInterfacePrototype[]? m_InterfacesCache;
    
    /// <summary>
    /// Cache for the Static Scope of the Class Prototype
    /// </summary>
    private BadScope? m_StaticScopeCache;

    /// <summary>
    ///     Creates a new BadExpressionClassPrototype
    /// </summary>
    /// <param name="name">Name of the Type</param>
    /// <param name="parentScope">The Parent scope this class prototype was created in.</param>
    /// <param name="body">The Class Body(Members & Functions)</param>
    /// <param name="baseClass">The Base class of the prototype</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <param name="meta">The Metadata of the Class</param>
    /// <param name="staticScope">The Static Scope of the Class</param>
    public BadExpressionClassPrototype(string name,
                                       BadScope parentScope,
                                       BadExpression[] body,
                                       Func<BadObject[], BadClassPrototype?> baseClass,
                                       Func<BadObject[], BadInterfacePrototype[]> interfaces,
                                       BadMetaData? meta,
                                       Func<BadObject[], BadScope> staticScope,
                                       IReadOnlyList<string> genericParameters) : base(name, meta)
    {
        m_ParentScope = parentScope;
        m_Body = body;
        m_StaticScope = staticScope;
        m_InterfacesFunc = interfaces;
        GenericParameters = genericParameters;
        m_BaseClassFunc = baseClass;

        if (IsGeneric)
        {
            GenericName = $"{name}<{string.Join(", ", genericParameters)}>";
        }
        else
        {
            GenericName = name;
        }
    }

    /// <summary>
    ///     Creates a new BadExpressionClassPrototype
    /// </summary>
    /// <param name="name">Name of the Type</param>
    /// <param name="parentScope">The Parent scope this class prototype was created in.</param>
    /// <param name="body">The Class Body(Members & Functions)</param>
    /// <param name="baseClass">The Base class of the prototype</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <param name="meta">The Metadata of the Class</param>
    /// <param name="staticScope">The Static Scope of the Class</param>
    private BadExpressionClassPrototype(string name,
                                        BadScope parentScope,
                                        BadExpression[] body,
                                        Func<BadObject[], BadClassPrototype?> baseClass,
                                        Func<BadObject[], BadInterfacePrototype[]> interfaces,
                                        BadMetaData? meta,
                                        Func<BadObject[], BadScope> staticScope,
                                        IReadOnlyList<string> genericParameters,
                                        BadExpressionClassPrototype genericDefinition,
                                        string genericName) : base(name, meta)
    {
        GenericName = genericName;
        m_ParentScope = parentScope;
        m_Body = body;
        m_StaticScope = staticScope;
        m_InterfacesFunc = interfaces;
        GenericParameters = genericParameters;
        m_GenericDefinition = genericDefinition;
        m_BaseClassFunc = baseClass;
    }

    /// <inheritdoc />
    protected override BadClassPrototype? BaseClass =>
        m_BaseClassCache ??= m_BaseClassFunc.Invoke(Array.Empty<BadObject>());

    /// <inheritdoc />
    public override bool IsAbstract => false;

    /// <inheritdoc />
    public override IReadOnlyCollection<BadInterfacePrototype> Interfaces =>
        m_InterfacesCache ??= m_InterfacesFunc.Invoke(Array.Empty<BadObject>());

    /// <summary>
    /// The Static Scope of the Class Prototype
    /// </summary>
    private BadScope StaticScope => m_StaticScopeCache ??= m_StaticScope(Array.Empty<BadObject>());

#region IBadGenericObject Members

/// <inheritdoc />
    public bool IsResolved => m_GenericDefinition != null;

    /// <inheritdoc />
    public bool IsGeneric => GenericParameters.Count != 0;

    /// <inheritdoc />
    public string GenericName { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> GenericParameters { get; }

    /// <inheritdoc />
    public BadObject CreateGeneric(BadObject[] args)
    {
        if (GenericParameters.Count != args.Length)
        {
            throw new BadRuntimeException("Invalid Generic Argument Count");
        }

        if (IsResolved)
        {
            throw new BadRuntimeException("Interface is already resolved");
        }

        int hash = args[0]
            .GetHashCode();

        //Add the other arguments to the hash
        for (int i = 1; i < args.Length; i++)
        {
            hash = (hash * 397) ^
                   args[i]
                       .GetHashCode();
        }

        if (s_GenericCache.TryGetValue(hash, out BadExpressionClassPrototype? cached))
        {
            return cached;
        }

        BadClassPrototype[] types = args.Cast<BadClassPrototype>()
                                        .ToArray();

        BadExpressionClassPrototype result = new BadExpressionClassPrototype(Name,
                                                                             m_ParentScope,
                                                                             m_Body,
                                                                             _ => m_BaseClassFunc(args),
                                                                             _ => m_InterfacesFunc(args),
                                                                             MetaData,
                                                                             _ => m_StaticScope(args),
                                                                             GenericParameters.ToArray(),
                                                                             this,
                                                                             $"{Name}<{string.Join(", ", types.Select(x => x is IBadGenericObject g ? g.GenericName : x.Name))}>"
                                                                            );
        s_GenericCache[hash] = result;

        return result;
    }

#endregion

/// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        if (IsGeneric)
        {
            return $"class {Name}<{string.Join(", ", GenericParameters)}>";
        }

        return $"class {Name}";
    }


    /// <inheritdoc />
    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        BadClass? baseInstance = null;

        BadExecutionContext ctx =
            new BadExecutionContext(StaticScope.CreateChild($"class instance {Name}", caller.Scope, true));
        ctx.Scope.SetFlags(BadScopeFlags.None);

        if (BaseClass is { IsAbstract: false })
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

            ctx.Scope.GetTable()
               .SetProperty(BadStaticKeys.BASE_KEY, baseInstance, new BadPropertyInfo(BaseClass, true));
        }

        BadClass thisInstance = new BadClass(Name, ctx, baseInstance, this);
        ctx.Scope.ClassObject = thisInstance;

        if (m_Body.Length != 0)
        {
            foreach (BadObject o in ctx.Execute(m_Body))
            {
                yield return o;
            }
        }

        if (setThis)
        {
            thisInstance.SetThis();

            if (Interfaces.Count != 0)
            {
                BadInterfaceValidatorResult result = thisInstance.Validate(Interfaces);

                if (!result.IsValid)
                {
                    throw new
                        BadRuntimeException($"Class '{Name}' does not implement all required interfaces.\n{result}");
                }
            }
        }

        yield return thisInstance;
    }

    /// <inheritdoc />
    public override bool IsSuperClassOf(BadClassPrototype proto)
    {
        return (GenericParameters.Count != 0 &&
                proto is BadExpressionClassPrototype gProto &&
                gProto.m_GenericDefinition == this) ||
               base.IsSuperClassOf(proto);
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return StaticScope.HasLocal(propName, caller ?? StaticScope) || base.HasProperty(propName, caller);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        return StaticScope.HasLocal(propName, caller ?? StaticScope)
                   ? StaticScope.GetVariable(propName, caller ?? m_ParentScope)
                   : base.GetProperty(propName, caller);
    }
}