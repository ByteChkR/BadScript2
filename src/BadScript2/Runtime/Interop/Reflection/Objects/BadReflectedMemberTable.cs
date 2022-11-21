using System.Reflection;

using BadScript2.Common;
using BadScript2.Common.Logging;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Reflection.Objects.Members;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Reflection.Objects;

public class BadReflectedMemberTable
{
    private static readonly Dictionary<Type, BadReflectedMemberTable> m_TableCache = new Dictionary<Type, BadReflectedMemberTable>();
    private readonly Dictionary<string, BadReflectedMember> m_Members;

    private BadReflectedMemberTable(Dictionary<string, BadReflectedMember> members)
    {
        m_Members = members;
    }

    public IEnumerable<string> MemberNames => m_Members.Keys;

    public bool Contains(string name)
    {
        return m_Members.ContainsKey(name);
    }

    public BadObjectReference GetMember(object instance, string name)
    {
        if (m_Members.TryGetValue(name, out BadReflectedMember member))
        {
            if (member.IsReadOnly)
            {
                return BadObjectReference.Make(name, () => member.Get(instance));
            }

            return BadObjectReference.Make(name, () => member.Get(instance), (o, i) => member.Set(instance, o));
        }

        throw new BadRuntimeException("Member " + name + " not found");
    }

    public static BadReflectedMemberTable Create<T>()
    {
        return Create(typeof(T));
    }

    public static BadReflectedMemberTable Create(Type t)
    {
        if (m_TableCache.ContainsKey(t))
        {
            return m_TableCache[t];
        }

        BadLogger.Log($"Creating Member Table for {t.Name}", "BadReflection");
        BadReflectedMemberTable table = CreateInternal(t);
        m_TableCache[t] = table;

        return table;
    }

    private static BadReflectedMemberTable CreateInternal(Type t)
    {
        Dictionary<string, BadReflectedMember> members = new Dictionary<string, BadReflectedMember>();

        if (t.IsArray)
        {
            members.Add(BadStaticKeys.ArrayAccessOperatorName, new BadReflectedMethod(t.GetMethod("Get")));
        }

        foreach (MemberInfo info in t.GetMembers())
        {
            if (info is FieldInfo field)
            {
                members.Add(field.Name, new BadReflectedField(field));
            }
            else if (info is PropertyInfo property)
            {
                if (property.Name == "Item" && property.GetIndexParameters().Length > 0)
                {
                    if (!members.ContainsKey(BadStaticKeys.ArrayAccessOperatorName))
                    {
                        members.Add(BadStaticKeys.ArrayAccessOperatorName, new BadReflectedMethod(property.GetMethod));
                    }
                }
                else
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
                else
                {
                    members.Add(method.Name, new BadReflectedMethod(method));
                }
            }
        }

        return new BadReflectedMemberTable(members);
    }
}