using System.Collections;
using System.Text;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects;

/// <summary>
///     Implements a Table Structure for the BadScript Language
/// </summary>
public class BadTable : BadObject, IBadEnumerable
{
    /// <summary>
    ///     The Prototype for the BadScript Table
    /// </summary>
    private static BadClassPrototype? s_Prototype;

    /// <summary>
    /// The Cache for Object References
    /// </summary>
    private readonly Dictionary<string, BadObjectReference> m_ReferenceCache =
        new Dictionary<string, BadObjectReference>();

    /// <summary>
    /// The Interceptor for Property Changes
    /// </summary>
    private Func<string, BadObject, BadObject, bool>? m_OnChangeProperty;

    /// <summary>
    ///     Creates a new Table Object
    /// </summary>
    public BadTable()
    {
        InnerTable = new Dictionary<string, BadObject>();
        PropertyInfos = new Dictionary<string, BadPropertyInfo>();
    }

    /// <summary>
    ///     Creates a new Table Object
    /// </summary>
    /// <param name="table">The Initial Values of the Table</param>
    public BadTable(Dictionary<string, BadObject> table)
    {
        InnerTable = table;
        PropertyInfos = new Dictionary<string, BadPropertyInfo>();

        foreach (KeyValuePair<string, BadObject> kvp in InnerTable)
        {
            PropertyInfos[kvp.Key] = new BadPropertyInfo(BadAnyPrototype.Instance);
        }
    }

    /// <summary>
    /// The Prototype for the BadScript Table
    /// </summary>
    public static BadClassPrototype Prototype => s_Prototype ??= BadNativeClassBuilder.GetNative("Table");

    /// <summary>
    ///     The Inner Table for this Object
    /// </summary>
    public Dictionary<string, BadObject> InnerTable { get; }

    /// <summary>
    ///     A Table of additional property information
    /// </summary>
    public Dictionary<string, BadPropertyInfo> PropertyInfos { get; }

#region IBadEnumerable Members

    /// <summary>
    ///     Returns the Enumerator for this Table
    /// </summary>
    /// <returns>The Enumerator for this Table</returns>
    public IEnumerator<BadObject> GetEnumerator()
    {
        return InnerTable
               .Select(kvp => new BadTable(new Dictionary<string, BadObject>
                                           {
                                               { "Key", kvp.Key }, { "Value", kvp.Value },
                                           }
                                          )
                      )
               .Cast<BadObject>()
               .GetEnumerator();
    }

    /// <summary>
    ///     Returns the Enumerator for this Table
    /// </summary>
    /// <returns>The Enumerator for this Table</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

#endregion

    /// <summary>
    /// The Event that is triggered when a Property is changed
    /// </summary>
    public event Action<string, BadObject, BadObject> OnChangedProperty = delegate { };

    /// <summary>
    /// Sets the Interceptor for Property Changes
    /// </summary>
    /// <param name="interceptor">The Interceptor</param>
    public void SetChangeInterceptor(Func<string, BadObject, BadObject, bool>? interceptor)
    {
        m_OnChangeProperty = interceptor;
    }


    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    /// <summary>
    ///     Returns Property Information for a given Key
    /// </summary>
    /// <param name="propName">Property Name</param>
    /// <returns>Property Info</returns>
    public BadPropertyInfo GetPropertyInfo(string propName)
    {
        return PropertyInfos[propName];
    }

    /// <summary>
    ///     Removes a Property from the Table
    /// </summary>
    /// <param name="key">Property Key</param>
    public bool RemoveKey(string key)
    {
        PropertyInfos.Remove(key);

        return InnerTable.Remove(key);
    }


    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return InnerTable.ContainsKey(propName) || (caller != null && caller.Provider.HasObject<BadTable>(propName));
    }


    /// <summary>
    ///     Returns a Reference to the Property with the given Name
    /// </summary>
    /// <param name="propName">The Property Name</param>
    /// <param name="useExtensions">Use Extension Properties</param>
    /// <param name="caller">The caller Scope</param>
    /// <returns>The Property Reference</returns>
    public BadObjectReference GetProperty(string propName, bool useExtensions, BadScope? caller = null)
    {
        return !useExtensions ? GetLocalReference(propName) : GetProperty(propName, caller);
    }

    /// <summary>
    /// Gets called when a Property is changed
    /// </summary>
    /// <param name="propName">The Property Name</param>
    /// <param name="oldValue">The Old Value</param>
    /// <param name="newValue">The New Value</param>
    /// <returns>True if the change was handled by the interceptor</returns>
    private bool OnChangePropertyInternal(string propName, BadObject oldValue, BadObject newValue)
    {
        if (m_OnChangeProperty != null)
        {
            return m_OnChangeProperty(propName, oldValue, newValue);
        }

        return false;
    }

    /// <summary>
    ///     Returns a Reference to a Local Property with the given Name
    /// </summary>
    /// <param name="propName">The Property Name</param>
    /// <returns>The Property Reference</returns>
    private BadObjectReference GetLocalReference(string propName)
    {
        if (m_ReferenceCache.TryGetValue(propName, out BadObjectReference reference))
        {
            return reference;
        }

        BadObjectReference r = BadObjectReference.Make($"BadTable.{propName}",
            (p) =>
            {
                if (InnerTable.TryGetValue(propName, out var result))
                {
                    return result;
                }
                throw BadRuntimeException.Create(null, $"Property '{propName}' not found", p);
            },
                                                       (o, p, t, noChange) =>
                                                       {
                                                           if (InnerTable.TryGetValue(propName,
                                                                    out BadObject? propValue
                                                                   ))
                                                           {
                                                               BadPropertyInfo info = GetPropertyInfo(propName);

                                                               if (propValue != Null && info.IsReadOnly)
                                                               {
                                                                   throw
                                                                       BadRuntimeException.Create(null,$"{propName} is read-only", p);
                                                               }

                                                               if (info.Type != null && !info.Type.IsAssignableFrom(o))
                                                               {
                                                                   throw BadRuntimeException.Create(null,$"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{info.Type.Name}'", p);
                                                               }

                                                               if (propValue is BadObjectReference propRef)
                                                               {
                                                                   BadObject? oldValueRef = propRef.Dereference(p);

                                                                   if (OnChangePropertyInternal(propName,
                                                                            oldValueRef,
                                                                            o
                                                                           ))
                                                                   {
                                                                       return;
                                                                   }

                                                                   propRef.Set(o, p, t);

                                                                   if (!noChange)
                                                                   {
                                                                       OnChangedProperty(propName, oldValueRef, o);
                                                                   }

                                                                   return;
                                                               }
                                                           }
                                                           else
                                                           {
                                                               PropertyInfos[propName] =
                                                                   t ?? new BadPropertyInfo(BadAnyPrototype.Instance);

                                                               if (t?.Type != null && !t.Type.IsAssignableFrom(o))
                                                               {
                                                                   throw BadRuntimeException.Create(null,$"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{t.Type.Name}'", p);
                                                               }
                                                           }

                                                           if (OnChangePropertyInternal(propName, propValue ?? Null, o))
                                                           {
                                                               return;
                                                           }

                                                           InnerTable[propName] = o;

                                                           if (!noChange)
                                                           {
                                                               OnChangedProperty(propName, propValue ?? Null, o);
                                                           }
                                                       },
                                                       (p) => { InnerTable.Remove(propName); }
                                                      );

        m_ReferenceCache[propName] = r;

        return r;
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        if (!InnerTable.ContainsKey(propName) && caller != null && caller.Provider.HasObject<BadTable>(propName))
        {
            return caller.Provider.GetObjectReference(GetType(), propName, this, caller);
        }

        return GetLocalReference(propName);
    }


    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        sb.AppendLine();

        foreach (KeyValuePair<string, BadObject> kvp in InnerTable)
        {
            if (kvp.Value is BadScope)
            {
                sb.AppendLine("RECURSION_PROTECT");

                continue;
            }

            string kStr = kvp.Key;

            string vStr = "{...}";

            if (!done.Contains(kvp.Value))
            {
                vStr = kvp.Value.ToSafeString(done)
                          .Trim();
            }

            if (kStr.Contains("\n"))
            {
                kStr = kStr.Replace("\n", "\n\t");
            }

            if (vStr.Contains("\n"))
            {
                vStr = vStr.Replace("\n", "\n\t");
            }

            sb.AppendLine($"\t{kStr}: {vStr}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}