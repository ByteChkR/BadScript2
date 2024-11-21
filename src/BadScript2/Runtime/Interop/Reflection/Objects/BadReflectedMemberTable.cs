using System.Reflection;

using BadScript2.Common;
using BadScript2.Common.Logging;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Reflection.Objects.Members;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains the Classes for Reflection Objects
/// </summary>
namespace BadScript2.Runtime.Interop.Reflection.Objects;

/// <summary>
///     Implements a Member Table for a specific Type, including a Caching Mechanism for Reflected Members to avoid
///     building the Reflection Table for every Object
/// </summary>
public class BadReflectedMemberTable
{
    /// <summary>
    ///     The Member Table Cache
    /// </summary>
    private static readonly Dictionary<Type, BadReflectedMemberTable> s_TableCache =
        new Dictionary<Type, BadReflectedMemberTable>();

    /// <summary>
    ///     The Reflected Members
    /// </summary>
    private readonly Dictionary<string, BadReflectedMember> m_Members;

    /// <summary>
    ///     Creates a new BadReflectedMemberTable
    /// </summary>
    /// <param name="members">The Reflected Members</param>
    private BadReflectedMemberTable(Dictionary<string, BadReflectedMember> members)
    {
        m_Members = members;
    }

    /// <summary>
    ///     The Member Names
    /// </summary>
    public IEnumerable<string> MemberNames => m_Members.Keys;

    /// <summary>
    ///     Returns true if the Member Table contains the given Member
    /// </summary>
    /// <param name="name">The Member Name</param>
    /// <returns>True if the Member Table contains the given Member</returns>
    public bool Contains(string name)
    {
        return m_Members.ContainsKey(name);
    }

    /// <summary>
    ///     Returns a reference to the Member with the given Name
    /// </summary>
    /// <param name="instance">Instance to get the Member from</param>
    /// <param name="name">The Member Name</param>
    /// <returns>The Member Reference</returns>
    /// <exception cref="BadRuntimeException">If the Member does not exist</exception>
    public BadObjectReference GetMember(object instance, string name)
    {
        if (m_Members.TryGetValue(name, out BadReflectedMember member))
        {
            return member.IsReadOnly
                       ? BadObjectReference.Make(name, (p) => member.Get(instance))
                       : BadObjectReference.Make(name, (p) => member.Get(instance), (o,_, _) => member.Set(instance, o));
        }

        throw new BadRuntimeException("Member " + name + " not found");
    }

    /// <summary>
    ///     Creates a new Member Table for the given Type
    /// </summary>
    /// <typeparam name="T">The Type to create the Member Table for</typeparam>
    /// <returns>The Member Table</returns>
    public static BadReflectedMemberTable Create<T>()
    {
        return Create(typeof(T));
    }

    /// <summary>
    ///     Creates a new Member Table for the given Type
    /// </summary>
    /// <param name="t">The Type to create the Member Table for</param>
    /// <returns>The Member Table</returns>
    public static BadReflectedMemberTable Create(Type t)
    {
        if (s_TableCache.TryGetValue(t, out BadReflectedMemberTable? value))
        {
            return value;
        }

        BadLogger.Log($"Creating Member Table for {t.Name}", "BadReflection");
        BadReflectedMemberTable table = CreateInternal(t);
        s_TableCache[t] = table;

        return table;
    }

    /// <summary>
    ///     Creates a new Member Table for the given Type
    /// </summary>
    /// <param name="t">The Type to create the Member Table for</param>
    /// <returns>The Member Table</returns>
    private static BadReflectedMemberTable CreateInternal(Type t)
    {
        Dictionary<string, BadReflectedMember> members = new Dictionary<string, BadReflectedMember>();

        if (t.IsArray)
        {
            members.Add(BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME, new BadReflectedMethod(t.GetMethod("Get")!));
        }

        foreach (MemberInfo info in t.GetMembers())
        {
            if (info is FieldInfo field && !members.ContainsKey(field.Name))
            {
                members.Add(field.Name, new BadReflectedField(field));
            }
            else if (info is PropertyInfo property)
            {
                if (property.Name == "Item" &&
                    property.GetIndexParameters()
                            .Length >
                    0)
                {
                    if (!members.ContainsKey(BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME))
                    {
                        members.Add(BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
                                    new BadReflectedMethod(property.GetMethod)
                                   );
                    }
                }
                else if (!members.ContainsKey(property.Name))
                {
                    members.Add(property.Name, new BadReflectedProperty(property));
                }
            }
            else if (info is MethodInfo method)
            {
                if (method.Name == "GetEnumerator")
                {
                    members["GetEnumerator"] = new BadReflectedEnumeratorMethod(method);
                }
                else if (members.ContainsKey(method.Name) && members[method.Name] is BadReflectedMethod m)
                {
                    m.AddMethod(method);
                }
                else if (!members.ContainsKey(method.Name))
                {
                    members.Add(method.Name, new BadReflectedMethod(method));
                }
            }
        }

        return new BadReflectedMemberTable(members);
    }
}