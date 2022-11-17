using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;

namespace BadScript2.Runtime.Objects.Types;

[Flags]
public enum BadPropertyVisibility
{
    Public = 1,
    Protected = 2,
    Private = 4,
    All = Public | Protected | Private,
}

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
    private readonly BadScope m_Scope;

    /// <summary>
    ///     Creates a new BadScript Class Instance
    /// </summary>
    /// <param name="name">The Type Name</param>
    /// <param name="scope">Table of all members of this type(excluding base class members)</param>
    /// <param name="baseClass">Base Class Instance</param>
    /// <param name="prototype">The Class Prototype used to create this instance.</param>
    public BadClass(string name, BadScope scope, BadClass? baseClass, BadClassPrototype prototype)
    {
        Name = name;
        m_Scope = scope;
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
        m_Scope.GetTable().GetProperty("this").Set(thisInstance, new BadPropertyInfo(thisInstance.Prototype, true));
        m_BaseClass?.SetThis(thisInstance);
    }

    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    public override bool HasProperty(BadObject propName)
    {
        if (m_Scope.GetTable().InnerTable.ContainsKey(propName))
        {
            return true;
        }

        if (m_BaseClass != null)
        {
            return m_BaseClass.HasProperty(propName);
        }

        return BadInteropExtension.HasObject(GetType(), propName);
    }


    

    public BadObjectReference GetProperty(BadObject propName, BadPropertyVisibility visibility, BadScope? caller = null)
    {
        BadPropertyVisibility vis = BadScope.GetPropertyVisibility(propName);
        if (caller != null)
        {
            if (caller.ClassObject == null)
            {
                visibility = BadPropertyVisibility.Public;
            }
            else if(caller.ClassObject == this)
            {
                visibility = BadPropertyVisibility.All;
            }
            else if (caller.ClassObject.InheritsFrom(Prototype))
            {
                visibility = BadPropertyVisibility.Public | BadPropertyVisibility.Protected;
            }
            else
            {
                visibility = BadPropertyVisibility.Public;
            }

            //Check if caller is this class
            // => All
            //check if caller is a child class 
            // => Public | Protected
            //else
            // => Public
        }

        if (!HasProperty(propName))
        {
            throw BadRuntimeException.Create(caller, $"Property {propName} not found in class {Name} or any of its base classes");
        }

        if ((vis & visibility) == 0)
        {
            throw BadRuntimeException.Create(caller, $"Property {Name}.{propName} is not visible from {caller?.Name ?? "global"}");
        }

        if (m_Scope.GetTable().InnerTable.ContainsKey(propName))
        {
            return BadObjectReference.Make(
                $"{Name}.{propName}",
                () => m_Scope.GetTable().InnerTable[propName],
                (o, _) =>
                {
                    if (m_Scope.GetTable().InnerTable.ContainsKey(propName))
                    {
                        BadPropertyInfo info = m_Scope.GetTable().GetPropertyInfo(propName);
                        if (m_Scope.GetTable().InnerTable[propName] != Null && info.IsReadOnly)
                        {
                            throw BadRuntimeException.Create(caller, $"{Name}.{propName} is read-only");
                        }

                        if (info.Type != null && !info.Type.IsAssignableFrom(o))
                        {
                            throw BadRuntimeException.Create(
                                caller,
                                $"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{info.Type.Name}'"
                            );
                        }
                    }

                    m_Scope.GetTable().InnerTable[propName] = o;
                }
            );
        }

        if (m_BaseClass != null)
        {
            return m_BaseClass.GetProperty(propName, visibility & ~BadPropertyVisibility.Private & BadPropertyVisibility.All, caller); //Allow public, protected
        }

        return BadInteropExtension.GetObjectReference(GetType(), propName, SuperClass ?? this, caller);
    }

    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        return GetProperty(propName, BadPropertyVisibility.Public, caller);
    }


    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return
            $"class {Name}\n{m_Scope.ToSafeString(done)}";
    }
}