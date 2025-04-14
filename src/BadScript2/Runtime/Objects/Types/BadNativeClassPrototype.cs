using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Implements a Native Class Prototype
/// </summary>
/// <typeparam name="T">Native Type</typeparam>
public class BadNativeClassPrototype<T> : BadANativeClassPrototype
    where T : BadObject
{
    /// <summary>
    /// Factory for the Implemented Interfaces of the Class
    /// </summary>
    private readonly Func<BadInterfacePrototype[]> m_InterfaceFunc;

    /// <summary>
    /// List of Static Members
    /// </summary>
    private readonly Dictionary<string, BadObjectReference> m_StaticMembers =
        new Dictionary<string, BadObjectReference>();

    /// <summary>
    /// Cached Interfaces returned by the Factory
    /// </summary>
    private BadInterfacePrototype[]? m_InterfacesCache;
    /// <inheritdoc />
    protected override BadClassPrototype? BaseClass { get; } = BadAnyPrototype.Instance;

    /// <summary>
    ///     Creates a new Native Class Prototype
    /// </summary>
    /// <param name="name">Class Name</param>
    /// <param name="func">Class Constructor</param>
    /// <param name="interfaces">The Implemented interfaces</param>
    public BadNativeClassPrototype(string name,
                                   Func<BadExecutionContext, BadObject[], BadObject> func,
                                   Func<BadInterfacePrototype[]> interfaces,
                                   BadClassPrototype? baseClass) : base(name, func)
    {
        m_InterfaceFunc = interfaces;
        BaseClass = baseClass ?? BadAnyPrototype.Instance;
    }

    /// <summary>
    ///     Creates a new Native Class Prototype
    /// </summary>
    /// <param name="name">Class Name</param>
    /// <param name="func">Class Constructor</param>
    public BadNativeClassPrototype(string name,
                                   Func<BadExecutionContext, BadObject[], BadObject> func,
                                   BadClassPrototype? baseClass) : base(name, func)
    {
        m_InterfaceFunc = () => Array.Empty<BadInterfacePrototype>();
        BaseClass = baseClass ?? BadAnyPrototype.Instance;
    }

    /// <summary>
    ///     Creates a new Native Class Prototype
    /// </summary>
    /// <param name="name">Class Name</param>
    /// <param name="func">Class Constructor</param>
    /// <param name="staticMembers">Static Members of the Type</param>
    /// <param name="interfaces">The Implemented interfaces</param>
    public BadNativeClassPrototype(string name,
                                   Func<BadExecutionContext, BadObject[], BadObject> func,
                                   Dictionary<string, BadObjectReference> staticMembers,
                                   BadClassPrototype? baseClass) : base(name, func)
    {
        m_StaticMembers = staticMembers;
        m_InterfaceFunc = () => Array.Empty<BadInterfacePrototype>();
        BaseClass = baseClass ?? BadAnyPrototype.Instance;
    }

    /// <summary>
    ///     Creates a new Native Class Prototype
    /// </summary>
    /// <param name="name">Class Name</param>
    /// <param name="func">Class Constructor</param>
    /// <param name="staticMembers">Static Members of the Type</param>
    /// <param name="interfaces">The Implemented interfaces</param>
    public BadNativeClassPrototype(string name,
                                   Func<BadExecutionContext, BadObject[], BadObject> func,
                                   Dictionary<string, BadObjectReference> staticMembers,
                                   Func<BadInterfacePrototype[]> interfaces,
                                   BadClassPrototype? baseClass) : base(name, func)
    {
        m_StaticMembers = staticMembers;
        m_InterfaceFunc = interfaces;
        BaseClass = baseClass ?? BadAnyPrototype.Instance;
    }

    /// <inheritdoc />
    public override bool IsAbstract => false;

    /// <inheritdoc />
    public override IReadOnlyCollection<BadInterfacePrototype> Interfaces => m_InterfacesCache ??= m_InterfaceFunc();

    /// <inheritdoc />
    public override bool IsAssignableFrom(BadObject obj)
    {
        if (obj == Null)
        {
            return true;
        }

        return obj is T;
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return m_StaticMembers.ContainsKey(propName) || base.HasProperty(propName, caller);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        if (m_StaticMembers.TryGetValue(propName, out BadObjectReference? o))
        {
            return o;
        }

        return base.GetProperty(propName, caller);
    }
}