using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Implements a Type Instance in the BadScript Language
/// </summary>
public class BadClass : BadObject
{
    /// <summary>
    ///     Base Class Instance
    /// </summary>
    private readonly BadClass? m_BaseClass;

    /// <summary>
    ///     Table of all members of this type(excluding base class members)
    /// </summary>
    private readonly BadTable m_Table;

    /// <summary>
    ///     Creates a new BadScript Class Instance
    /// </summary>
    /// <param name="name">The Type Name</param>
    /// <param name="table">Table of all members of this type(excluding base class members)</param>
    /// <param name="baseClass">Base Class Instance</param>
    /// <param name="prototype">The Class Prototype used to create this instance.</param>
    public BadClass(string name, BadTable table, BadClass? baseClass, BadClassPrototype prototype)
    {
        Name = name;
        m_Table = table;
        m_BaseClass = baseClass;
        Prototype = prototype;
    }

    /// <summary>
    ///     The Type Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     The Class Prototype used to create this instance.
    /// </summary>
    public BadClassPrototype Prototype { get; }

    /// <summary>
    ///     The Super Class of this Class(if this class is not at the end of the inheritance chain)
    /// </summary>
    public BadClass? SuperClass { get; private set; }


    /// <summary>
    ///     Returns true if the given object is an instance of the specified prototype
    /// </summary>
    /// <param name="proto">The Prototype</param>
    /// <returns>True if the given object is an instance of the specified prototype</returns>
    public bool InheritsFrom(BadClassPrototype proto)
    {
        return Prototype == proto || m_BaseClass != null && m_BaseClass.InheritsFrom(proto);
    }

    /// <summary>
    ///     Sets the 'this' parameter of the class scope to be this instance
    /// </summary>
    public void SetThis()
    {
        SetThis(this);
    }

    /// <summary>
    ///     Sets the 'this' parameter of the class scope to be the specified instance
    /// </summary>
    /// <param name="thisInstance">New This Instance</param>
    private void SetThis(BadClass thisInstance)
    {
        SuperClass = thisInstance;
        m_Table.GetProperty("this").Set(thisInstance, new BadPropertyInfo(thisInstance.Prototype, true));
        m_BaseClass?.SetThis(thisInstance);
    }

    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    public override bool HasProperty(BadObject propName)
    {
        if (m_Table.InnerTable.ContainsKey(propName))
        {
            return true;
        }

        if (m_BaseClass != null)
        {
            return m_BaseClass.HasProperty(propName);
        }

        return BadInteropExtension.HasObject(GetType(), propName);
    }

    public override BadObjectReference GetProperty(BadObject propName)
    {
        if (!HasProperty(propName))
        {
            throw new BadRuntimeException($"Property {propName} not found in class {Name} or any of its base classes");
        }

        if (m_Table.InnerTable.ContainsKey(propName))
        {
            return BadObjectReference.Make(
                $"{Name}.{propName}",
                () => m_Table.InnerTable[propName],
                (o, _) =>
                {
                    if (m_Table.InnerTable.ContainsKey(propName))
                    {
                        BadPropertyInfo info = m_Table.GetPropertyInfo(propName);
                        if (m_Table.InnerTable[propName] != Null && info.IsReadOnly)
                        {
                            throw new BadRuntimeException($"{Name}.{propName} is read-only");
                        }

                        if (info.Type != null && !info.Type.IsAssignableFrom(o))
                        {
                            throw new BadRuntimeException(
                                $"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{info.Type.Name}'"
                            );
                        }
                    }

                    m_Table.InnerTable[propName] = o;
                }
            );
        }

        if (m_BaseClass != null)
        {
            return m_BaseClass.GetProperty(propName);
        }

        return BadInteropExtension.GetObjectReference(GetType(), propName, SuperClass ?? this);
    }


    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return
            $"class {Name}\n{m_Table.ToSafeString(done)}";
    }
}