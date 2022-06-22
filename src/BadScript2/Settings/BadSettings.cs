using System.Text;

using Newtonsoft.Json.Linq;

namespace BadScript2.Settings;

public class BadSettings
{
    private readonly Dictionary<Type, object> m_ObjectCache = new Dictionary<Type, object>();
    private readonly Dictionary<string, BadSettings> m_Properties;

    private JToken? m_Value;

    public BadSettings()
    {
        m_Value = null;
        m_Properties = new Dictionary<string, BadSettings>();
    }

    public BadSettings(JToken? value)
    {
        m_Value = value;
        m_Properties = new Dictionary<string, BadSettings>();
    }

    public BadSettings(Dictionary<string, BadSettings> properties)
    {
        m_Value = null;
        m_Properties = properties;
    }

    public IEnumerable<string> PropertyNames => m_Properties.Keys;

    public JToken? GetValue()
    {
        return m_Value;
    }

    public T? GetValue<T>()
    {
        if (m_Value == null)
        {
            return default;
        }

        Type t = typeof(T);
        if (m_ObjectCache.ContainsKey(t))
        {
            return (T)m_ObjectCache[t];
        }

        T? v = m_Value.ToObject<T>();
        if (v != null)
        {
            m_ObjectCache[typeof(T)] = v;
        }


        return v;
    }

    public bool HasValue()
    {
        return m_Value != null;
    }

    private bool HasValue<T>()
    {
        if (m_Value?.Type == JTokenType.Array && !typeof(T).IsArray)
        {
            return false;
        }

        return m_Value != null && m_Value.ToObject<T>() != null;
    }

    public void SetValue(JToken? value)
    {
        m_ObjectCache.Clear();
        m_Value = value;
    }

    private void SetValue(object? value)
    {
        SetValue(value == null ? JValue.CreateNull() : JToken.FromObject(value));
    }

    public bool HasProperty(string propertyName)
    {
        return m_Properties.ContainsKey(propertyName);
    }

    public BadSettings GetProperty(string propertyName)
    {
        return m_Properties[propertyName];
    }

    public void SetProperty(string propertyName, BadSettings value)
    {
        m_Properties[propertyName] = value;
    }

    public void RemoveProperty(string propertyName)
    {
        m_Properties.Remove(propertyName);
    }


    public void Populate(params BadSettings[] settings)
    {
        if (m_Value is JArray arr)
        {
            foreach (BadSettings setting in settings)
            {
                if (!setting.HasValue())
                {
                    continue;
                }

                if (setting.GetValue() is JArray arr2)
                {
                    foreach (JToken jToken in arr2)
                    {
                        arr.Add(jToken);
                    }
                }
                else
                {
                    arr.Add(setting.GetValue() ?? JValue.CreateNull());
                }
            }
        }

        foreach (BadSettings setting in settings)
        {
            foreach (string propertyName in setting.PropertyNames)
            {
                if (HasProperty(propertyName))
                {
                    GetProperty(propertyName).Populate(setting.GetProperty(propertyName));
                }
                else
                {
                    SetProperty(propertyName, setting.GetProperty(propertyName));
                }
            }
        }
    }

    public T? FindProperty<T>(string propertyName) where T : class
    {
        BadSettings? settings = FindProperty(propertyName);

        return settings?.GetValue<T>();
    }

    public BadSettings FindOrCreateProperty(string propertyPath)
    {
        string[] path = propertyPath.Split('.');
        BadSettings current = this;
        foreach (string s in path)
        {
            if (!current.HasProperty(s))
            {
                BadSettings se = new BadSettings();
                current.SetProperty(s, se);
                current = se;
            }
            else
            {
                current = current.GetProperty(s);
            }
        }

        return current;
    }

    public BadSettings? FindProperty(string propertyPath)
    {
        string[] path = propertyPath.Split('.');
        BadSettings current = this;
        foreach (string s in path)
        {
            if (!current.HasProperty(s))
            {
                return null;
            }

            current = current.GetProperty(s);
        }

        return current;
    }

    public override string ToString()
    {
        if (HasValue())
        {
            return GetValue()?.ToString() ?? "NULL";
        }

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("{");
        foreach (string propertyName in PropertyNames)
        {
            BadSettings s = GetProperty(propertyName);
            string str = s.ToString().Replace("\n", "\n\t");
            sb.AppendLine($"  {propertyName}: {str}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string ResolveEnvironmentVariables(BadSettings root, BadSettings parent, string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '$' && i + 1 < str.Length && str[i + 1] == '(')
            {
                int end = str.IndexOf(')', i + 2);
                if (end == -1)
                {
                    throw new Exception("Unclosed environment variable");
                }

                string envVar = str.Substring(i + 2, end - i - 2);

                string? env = root.FindProperty<string>(envVar);
                if (env == null)
                {
                    env = parent.FindProperty<string>(envVar);
                    if (env == null)
                    {
                        throw new Exception($"Environment variable '{envVar}' not found");
                    }
                }

                str = str.Replace(str.Substring(i, end - i + 1), env);
                i--;
            }
        }

        return str;
    }

    public static void ResolveEnvironmentVariables(BadSettings root)
    {
        ResolveEnvironmentVariables(root, root, root);
    }

    public static void ResolveEnvironmentVariables(BadSettings root, BadSettings settings, BadSettings parent)
    {
        if (settings.HasValue<string>())
        {
            string value = settings.GetValue<string>()!;

            settings.SetValue(ResolveEnvironmentVariables(root, parent, value));
        }
        else if (settings.HasValue<string[]>())
        {
            string[] value = settings.GetValue<string[]>()!;

            for (int i = 0; i < value.Length; i++)
            {
                value[i] = ResolveEnvironmentVariables(root, parent, value[i]);
            }

            settings.SetValue(value);
        }

        foreach (string propertyName in settings.PropertyNames)
        {
            ResolveEnvironmentVariables(root, settings.GetProperty(propertyName), settings);
        }
    }
}