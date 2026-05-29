using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types.Interface;
using BadScript2.Runtime.VirtualMachine.Compiler;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Implements a Class Prototype created from Bad Expressions(e.g. Source Code)
/// </summary>
public class BadExpressionClassPrototype : BadClassPrototype, IBadGenericObject
{
    /// <summary>
    /// Raised when an instance member is about to be materialized.
    /// </summary>
    public static event Action<BadCompiledClassMemberTemplate>? OnMaterializeInstanceMember;

    /// <summary>
    /// Raised when a static member is about to be materialized.
    /// </summary>
    public static event Action<BadCompiledClassMemberTemplate>? OnMaterializeStaticMember;

    private readonly Func<BadObject[], BadClassPrototype?> m_BaseClassFunc;

    /// <summary>
    ///     Structured templates for the class body members.
    /// </summary>
    private readonly BadCompiledClassMemberTemplate[] m_Members;

    /// <summary>
    ///     Structured templates for the static class body members.
    /// </summary>
    private readonly BadCompiledClassMemberTemplate[] m_StaticMembers;

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
    /// Slot map for instance methods: methodName → slotIndex.
    /// Built lazily on first instance creation.
    /// </summary>
    private Dictionary<string, int>? m_MethodSlotMap;

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
                                       BadCompiledClassMemberTemplate[] members,
                                       BadCompiledClassMemberTemplate[] staticMembers,
                                       Func<BadObject[], BadClassPrototype?> baseClass,
                                       Func<BadObject[], BadInterfacePrototype[]> interfaces,
                                       BadMetaData? meta,
                                       Func<BadObject[], BadScope> staticScope,
                                       IReadOnlyList<string> genericParameters) : base(name, meta)
    {
        m_ParentScope = parentScope;
        m_Members = members;
        m_StaticMembers = staticMembers;
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
                                        BadCompiledClassMemberTemplate[] members,
                                        BadCompiledClassMemberTemplate[] staticMembers,
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
        m_Members = members;
        m_StaticMembers = staticMembers;
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
    private BadScope StaticScope => m_StaticScopeCache ??= InitializeStaticScope(m_StaticScope(Array.Empty<BadObject>()));

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
                                                                             m_Members,
                                                                              m_StaticMembers,
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

    private BadExecutionContext CreateInstanceContext(BadExecutionContext caller)
    {
        BadExecutionContext context =
            new BadExecutionContext(StaticScope.CreateChild($"class instance {Name}", caller.Scope, true));
        context.Scope.SetFlags(BadScopeFlags.None);

        return context;
    }

    private IEnumerable<BadObject> CreateBaseInstance(BadExecutionContext caller,
                                                      BadExecutionContext instanceContext,
                                                      Action<BadClass?> setBaseInstance)
    {
        BadClass? baseInstance = null;

        if (BaseClass is { IsAbstract: false })
        {
            BadObject obj = Null;

            foreach (BadObject o in BaseClass.CreateInstance(caller, false))
            {
                obj = o;
                yield return o;
            }

            if (obj is not BadClass cls)
            {
                throw new BadRuntimeException("Base class is not a class");
            }

            baseInstance = cls;

            instanceContext.Scope.GetTable()
                          .SetProperty(BadStaticKeys.BASE_KEY, baseInstance, new BadPropertyInfo(BaseClass, true));
        }

        setBaseInstance(baseInstance);
    }

    private BadClass CreateClassInstance(BadExecutionContext instanceContext, BadClass? baseInstance)
    {
        BadClass instance = new BadClass(Name, instanceContext, baseInstance, this);
        instanceContext.Scope.ClassObject = instance;

        return instance;
    }

    private IEnumerable<BadObject> MaterializeInstanceMembers(BadExecutionContext instanceContext)
    {
        foreach (BadObject o in MaterializeInstanceFields(instanceContext))
        {
            yield return o;
        }

        foreach (BadObject o in MaterializeInstanceUnknownMembers(instanceContext))
        {
            yield return o;
        }

        foreach (BadObject o in MaterializeInstanceMethods(instanceContext))
        {
            yield return o;
        }

        foreach (BadObject o in MaterializeInstanceProperties(instanceContext))
        {
            yield return o;
        }

        foreach (BadObject o in MaterializeInstanceConstructors(instanceContext))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeInstanceFields(BadExecutionContext instanceContext)
    {
        foreach (BadObject o in ExecuteMemberPhase(instanceContext, BadCompiledClassMemberKind.Field))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeInstanceUnknownMembers(BadExecutionContext instanceContext)
    {
        foreach (BadObject o in ExecuteMemberPhase(instanceContext, BadCompiledClassMemberKind.Unknown))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeInstanceMethods(BadExecutionContext instanceContext)
    {
        foreach (BadObject o in ExecuteMemberPhase(instanceContext, BadCompiledClassMemberKind.Method))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeInstanceProperties(BadExecutionContext instanceContext)
    {
        foreach (BadObject o in ExecuteMemberPhase(instanceContext, BadCompiledClassMemberKind.Property))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeInstanceConstructors(BadExecutionContext instanceContext)
    {
        foreach (BadObject o in ExecuteMemberPhase(instanceContext, BadCompiledClassMemberKind.Constructor))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> ExecuteMemberPhase(BadExecutionContext instanceContext,
                                                      BadCompiledClassMemberKind kind)
    {
        if (m_Members.Length == 0)
        {
            yield break;
        }

        foreach (BadCompiledClassMemberTemplate member in m_Members)
        {
            if (member.Kind != kind)
            {
                continue;
            }

            OnMaterializeInstanceMember?.Invoke(member);

            foreach (BadObject o in member.Execute(instanceContext))
            {
                yield return o;
            }
        }
    }

    private BadScope InitializeStaticScope(BadScope staticScope)
    {
        BadExecutionContext staticContext = new BadExecutionContext(staticScope);

        foreach (BadObject _ in MaterializeStaticMembers(staticContext))
        {
        }

        return staticScope;
    }

    private IEnumerable<BadObject> MaterializeStaticMembers(BadExecutionContext staticContext)
    {
        foreach (BadObject o in MaterializeStaticFields(staticContext))
        {
            yield return o;
        }

        foreach (BadObject o in MaterializeStaticUnknownMembers(staticContext))
        {
            yield return o;
        }

        foreach (BadObject o in MaterializeStaticMethods(staticContext))
        {
            yield return o;
        }

        foreach (BadObject o in MaterializeStaticProperties(staticContext))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeStaticFields(BadExecutionContext staticContext)
    {
        foreach (BadObject o in ExecuteStaticMemberPhase(staticContext, BadCompiledClassMemberKind.Field))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeStaticUnknownMembers(BadExecutionContext staticContext)
    {
        foreach (BadObject o in ExecuteStaticMemberPhase(staticContext, BadCompiledClassMemberKind.Unknown))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeStaticMethods(BadExecutionContext staticContext)
    {
        foreach (BadObject o in ExecuteStaticMemberPhase(staticContext, BadCompiledClassMemberKind.Method))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> MaterializeStaticProperties(BadExecutionContext staticContext)
    {
        foreach (BadObject o in ExecuteStaticMemberPhase(staticContext, BadCompiledClassMemberKind.Property))
        {
            yield return o;
        }
    }

    private IEnumerable<BadObject> ExecuteStaticMemberPhase(BadExecutionContext staticContext,
                                                           BadCompiledClassMemberKind kind)
    {
        if (m_StaticMembers.Length == 0)
        {
            yield break;
        }

        foreach (BadCompiledClassMemberTemplate member in m_StaticMembers)
        {
            if (member.Kind != kind)
            {
                continue;
            }

            OnMaterializeStaticMember?.Invoke(member);

            if (member.Property != null)
            {
                foreach (BadObject o in member.Property.Define(staticContext, false))
                {
                    yield return o;
                }
            }
            else
            {
                foreach (BadObject o in member.Execute(staticContext))
                {
                    yield return o;
                }
            }
        }
    }

    private void BindThis(BadClass instance, bool setThis)
    {
        if (!setThis)
        {
            return;
        }

        instance.SetThis();
    }

    private void ValidateInterfaces(BadClass instance, bool setThis)
    {
        if (!setThis || Interfaces.Count == 0)
        {
            return;
        }

        BadInterfaceValidatorResult result = instance.Validate(Interfaces);

        if (!result.IsValid)
        {
            throw new BadRuntimeException($"Class '{Name}' does not implement all required interfaces.\n{result}");
        }
    }

    private void EnsureMethodSlotMap()
    {
        if (m_MethodSlotMap != null)
        {
            return;
        }

        var map = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (BadCompiledClassMemberTemplate member in m_Members)
        {
            if (member.Kind == BadCompiledClassMemberKind.Method &&
                member.Name != null &&
                member.Name != BadStaticKeys.CONSTRUCTOR_NAME &&
                !map.ContainsKey(member.Name))
            {
                map[member.Name] = map.Count;
            }
        }

        m_MethodSlotMap = map;
    }

    /// <summary>
    /// Tries to get the slot index for a method by name.
    /// Returns false if the method is not in the slot map (e.g. constructor, property, field, or inherited method).
    /// </summary>
    internal bool TryGetMethodSlotIndex(string name, out int index)
    {
        EnsureMethodSlotMap();
        return m_MethodSlotMap!.TryGetValue(name, out index);
    }

    /// <summary>
    /// Returns the method slot map (name → index). Used by BadClass.InitializeMethodSlots.
    /// </summary>
    internal Dictionary<string, int> MethodSlotMap
    {
        get
        {
            EnsureMethodSlotMap();
            return m_MethodSlotMap!;
        }
    }


    /// <inheritdoc />
    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        BadExecutionContext instanceContext = CreateInstanceContext(caller);
        BadClass? baseInstance = null;

        foreach (BadObject o in CreateBaseInstance(caller, instanceContext, instance => baseInstance = instance))
        {
            yield return o;
        }

        BadClass thisInstance = CreateClassInstance(instanceContext, baseInstance);

        foreach (BadObject o in MaterializeInstanceMembers(instanceContext))
        {
            yield return o;
        }

        thisInstance.InitializeMethodSlots(MethodSlotMap);

        BindThis(thisInstance, setThis);
        ValidateInterfaces(thisInstance, setThis);

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