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
    private static BadClassPrototype? s_Prototype;

    /// <summary>
    ///     Creates a new Table Object
    /// </summary>
    public BadTable()
    {
        InnerTable = new Dictionary<BadObject, BadObject>();
        PropertyInfos = new Dictionary<BadObject, BadPropertyInfo>();
    }

    /// <summary>
    ///     Creates a new Table Object
    /// </summary>
    /// <param name="table">The Initial Values of the Table</param>
    public BadTable(Dictionary<BadObject, BadObject> table)
    {
        InnerTable = table;
        PropertyInfos = new Dictionary<BadObject, BadPropertyInfo>();

        foreach (KeyValuePair<BadObject, BadObject> kvp in InnerTable)
        {
            PropertyInfos[kvp.Key] = new BadPropertyInfo(BadAnyPrototype.Instance);
        }
    }

    public static BadClassPrototype Prototype => s_Prototype ??= BadNativeClassBuilder.GetNative("Table");

    /// <summary>
    ///     The Inner Table for this Object
    /// </summary>
    public Dictionary<BadObject, BadObject> InnerTable { get; }

    /// <summary>
    ///     A Table of additional property information
    /// </summary>
    public Dictionary<BadObject, BadPropertyInfo> PropertyInfos { get; }

    public IEnumerator<BadObject> GetEnumerator()
    {
        foreach (KeyValuePair<BadObject, BadObject> kvp in InnerTable)
        {
            yield return new BadTable(
                new Dictionary<BadObject, BadObject>
                {
                    {
                        "Key", kvp.Key
                    },
                    {
                        "Value", kvp.Value
                    },
                }
            );
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    /// <summary>
    ///     Returns Property Information for a given Key
    /// </summary>
    /// <param name="propName">Property Name</param>
    /// <returns>Property Info</returns>
    public BadPropertyInfo GetPropertyInfo(BadObject propName)
    {
        return PropertyInfos[propName];
    }

    /// <summary>
    ///     Removes a Property from the Table
    /// </summary>
    /// <param name="key">Property Key</param>
    public bool RemoveKey(BadObject key)
    {
        PropertyInfos.Remove(key);

        return InnerTable.Remove(key);
    }


    public override bool HasProperty(BadObject propName, BadScope? caller = null)
    {
        return InnerTable.ContainsKey(propName) || caller != null && caller.Provider.HasObject<BadTable>(propName);
    }


    public BadObjectReference GetProperty(BadObject propName, bool useExtensions, BadScope? caller = null)
    {
        if (!useExtensions)
        {
            return GetLocalReference(propName);
        }

        return GetProperty(propName, caller);
    }

    private BadObjectReference GetLocalReference(BadObject propName)
    {
        return BadObjectReference.Make(
            $"BadTable.{propName}",
            () => InnerTable[propName],
            (o, t) =>
            {
                if (InnerTable.TryGetValue(propName, out BadObject? propValue))
                {
                    BadPropertyInfo info = GetPropertyInfo(propName);

                    if (propValue != Null && info.IsReadOnly)
                    {
                        throw new BadRuntimeException($"{propName} is read-only");
                    }

                    if (info.Type != null && !info.Type.IsAssignableFrom(o))
                    {
                        throw new BadRuntimeException(
                            $"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{info.Type.Name}'"
                        );
                    }
                }
                else
                {
                    PropertyInfos[propName] = t ?? new BadPropertyInfo(BadAnyPrototype.Instance);

                    if (t?.Type != null && !t.Type.IsAssignableFrom(o))
                    {
                        throw new BadRuntimeException(
                            $"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{t.Type.Name}'"
                        );
                    }
                }

                InnerTable[propName] = o;
            },
            () => { InnerTable.Remove(propName); }
        );
    }

    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        if (!InnerTable.ContainsKey(propName) && caller != null && caller.Provider.HasObject<BadTable>(propName))
        {
            return caller.Provider.GetObjectReference(GetType(), propName, this, caller);
        }

        return GetLocalReference(propName);
    }


    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        sb.AppendLine();

        foreach (KeyValuePair<BadObject, BadObject> kvp in InnerTable)
        {
            if (kvp.Key is BadScope || kvp.Value is BadScope)
            {
                sb.AppendLine("RECURSION_PROTECT");

                continue;
            }

            string kStr = "{...}";

            if (!done.Contains(kvp.Key))
            {
                kStr = kvp.Key.ToSafeString(done)!.Trim();
            }

            string vStr = "{...}";

            if (!done.Contains(kvp.Value))
            {
                vStr = kvp.Value.ToSafeString(done)!.Trim();
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