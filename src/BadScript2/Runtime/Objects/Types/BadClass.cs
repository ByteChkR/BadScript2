using BadScript2.Runtime.Error;

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
    ///     Creates a new BadScript Class Instance
    /// </summary>
    /// <param name="name">The Type Name</param>
    /// <param name="scope">Table of all members of this type(excluding base class members)</param>
    /// <param name="baseClass">Base Class Instance</param>
    /// <param name="prototype">The Class Prototype used to create this instance.</param>
    public BadClass(string name, BadScope scope, BadClass? baseClass, BadClassPrototype prototype)
    {
        Name = name;
        Scope = scope;
        m_BaseClass = baseClass;
        Prototype = prototype;
    }

    /// <summary>
    ///     Table of all members of this type(excluding base class members)
    /// </summary>
    public BadScope Scope { get; }

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
        return proto.IsSuperClassOf(Prototype);
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
        Scope.GetTable().GetProperty("this").Set(thisInstance, new BadPropertyInfo(thisInstance.Prototype, true));
        m_BaseClass?.SetThis(thisInstance);
    }

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    /// <inheritdoc />
    public override bool HasProperty(BadObject propName, BadScope? caller = null)
    {
        if (Scope.GetTable().InnerTable.ContainsKey(propName))
        {
            return true;
        }

        if (m_BaseClass != null)
        {
            return m_BaseClass.HasProperty(propName, caller);
        }

        return caller != null && caller.Provider.HasObject(GetType(), propName);
    }


    /// <summary>
    /// Gets a property from this class or any of its base classes.
    /// </summary>
    /// <param name="propName">The name of the property to get.</param>
    /// <param name="visibility">The visibility of the property to get.</param>
    /// <param name="caller">The scope of the caller.</param>
    /// <returns>The property.</returns>
    /// <exception cref="BadRuntimeException">Thrown if the property is not found or is not visible.</exception>
    public BadObjectReference GetProperty(BadObject propName, BadPropertyVisibility visibility, BadScope? caller = null)
    {
        BadPropertyVisibility vis = BadScope.GetPropertyVisibility(propName);

        if (caller != null)
        {
            if (caller.ClassObject == null)
            {
                visibility = BadPropertyVisibility.Public;
            }
            else if (caller.ClassObject == this)
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
        }

        if (!HasProperty(propName, Scope))
        {
            throw BadRuntimeException.Create(
                caller,
                $"Property {propName} not found in class {Name} or any of its base classes"
            );
        }

        if ((vis & visibility) == 0)
        {
            throw BadRuntimeException.Create(
                caller,
                $"Property {Name}.{propName} is not visible from {caller?.Name ?? "global"}"
            );
        }

        if (Scope.GetTable().InnerTable.ContainsKey(propName))
        {
            return BadObjectReference.Make(
                $"{Name}.{propName}",
                () => Scope.GetTable().InnerTable[propName],
                (o, _) =>
                {
                    if (Scope.GetTable().InnerTable.ContainsKey(propName))
                    {
                        BadPropertyInfo info = Scope.GetTable().GetPropertyInfo(propName);

                        if (Scope.GetTable().InnerTable[propName] != Null && info.IsReadOnly)
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

                    Scope.GetTable().InnerTable[propName] = o;
                }
            );
        }

        if (m_BaseClass != null)
        {
            return m_BaseClass.GetProperty(
                propName,
                visibility & ~BadPropertyVisibility.Private & BadPropertyVisibility.All,
                caller
            ); //Allow public, protected
        }

        if (caller == null)
        {
            throw BadRuntimeException.Create(caller, $"No property named {propName} for type {Name}");
        }

        return caller.Provider.GetObjectReference(GetType(), propName, SuperClass ?? this, caller);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        return GetProperty(propName, BadPropertyVisibility.Public, caller);
    }


    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return
            $"class {Name}\n{Scope.ToSafeString(done)}";
    }
}