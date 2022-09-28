using System.Text;

using Newtonsoft.Json.Linq;

namespace BadScript2.Settings
{
    /// <summary>
    ///     Public Api for the Settings System
    /// </summary>
    public class BadSettings
    {
        /// <summary>
        ///     Cache for the Serialized Value of the Settings
        /// </summary>
        private readonly Dictionary<Type, object> m_ObjectCache = new Dictionary<Type, object>();

        /// <summary>
        ///     The properties of the Current Settings Object
        /// </summary>
        private readonly Dictionary<string, BadSettings> m_Properties;

        /// <summary>
        ///     The Json Token of the Settings Object
        /// </summary>
        private JToken? m_Value;

        /// <summary>
        ///     Creates a new empty Settings Object
        /// </summary>
        public BadSettings()
        {
            m_Value = null;
            m_Properties = new Dictionary<string, BadSettings>();
        }

        /// <summary>
        ///     Creates a new Settings Object from a Json Token
        /// </summary>
        /// <param name="value">The Json Token</param>
        public BadSettings(JToken? value)
        {
            m_Value = value;
            m_Properties = new Dictionary<string, BadSettings>();
        }

        /// <summary>
        ///     Creates a new Settings Object from a Dictionary of Properties
        /// </summary>
        /// <param name="properties">The Properties</param>
        public BadSettings(Dictionary<string, BadSettings> properties)
        {
            m_Value = null;
            m_Properties = properties;
        }

        /// <summary>
        ///     The Property Names of the Settings Object
        /// </summary>
        public IEnumerable<string> PropertyNames => m_Properties.Keys;

        /// <summary>
        ///     Returns the Json Token of the Settings Object
        /// </summary>
        /// <returns>Json Token</returns>
        public JToken? GetValue()
        {
            return m_Value;
        }

        /// <summary>
        ///     Returns a Deserialized Value of the Settings Object
        /// </summary>
        /// <typeparam name="T">The Type to Deserialize into</typeparam>
        /// <returns>Deserialized Result</returns>
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

        /// <summary>
        ///     Returns true if the Settings Object has a JToken Value
        /// </summary>
        /// <returns></returns>
        public bool HasValue()
        {
            return m_Value != null;
        }

        /// <summary>
        ///     Returns true if the Settings Object can be Deserialized into the given Type
        /// </summary>
        /// <typeparam name="T">The Type</typeparam>
        /// <returns>true if Deserializable</returns>
        private bool HasValue<T>()
        {
            if (m_Value?.Type == JTokenType.Array && !typeof(T).IsArray)
            {
                return false;
            }

            return m_Value != null && m_Value.ToObject<T>() != null;
        }

        /// <summary>
        ///     Sets the Json Token of the Current Settings Object
        /// </summary>
        /// <param name="value">Json Token</param>
        public void SetValue(JToken? value)
        {
            m_ObjectCache.Clear();
            m_Value = value;
        }

        /// <summary>
        ///     Sets the Json Token of the Current Settings Object
        /// </summary>
        /// <param name="value">The Deserialized Object</param>
        private void SetValue(object? value)
        {
            SetValue(value == null ? JValue.CreateNull() : JToken.FromObject(value));
        }

        /// <summary>
        ///     Returns true if the Settings Object has the given Property
        /// </summary>
        /// <param name="propertyName">The Property Name</param>
        /// <returns>true if the Property exists</returns>
        public bool HasProperty(string propertyName)
        {
            return m_Properties.ContainsKey(propertyName);
        }

        /// <summary>
        ///     Returns the Property with the given Name
        /// </summary>
        /// <param name="propertyName">The Property Name</param>
        /// <returns>The Property</returns>
        public BadSettings GetProperty(string propertyName)
        {
            return m_Properties[propertyName];
        }

        /// <summary>
        ///     Sets the Property with the given Name
        /// </summary>
        /// <param name="propertyName">The Property Name</param>
        /// <param name="value">The Property Value</param>
        public void SetProperty(string propertyName, BadSettings value)
        {
            m_Properties[propertyName] = value;
        }

        /// <summary>
        ///     Removes the Property with the given Name
        /// </summary>
        /// <param name="propertyName">The Property Name</param>
        public void RemoveProperty(string propertyName)
        {
            m_Properties.Remove(propertyName);
        }


        /// <summary>
        ///     Populates the current object with the settings provided
        /// </summary>
        /// <param name="settings">The settings this object will be populated with</param>
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

        /// <summary>
        ///     Finds a property based on the property path relative to this object
        /// </summary>
        /// <param name="propertyName">Property Path</param>
        /// <typeparam name="T">Type to deserialize into</typeparam>
        /// <returns>Deserialized Value</returns>
        public T? FindProperty<T>(string propertyName) where T : class
        {
            BadSettings? settings = FindProperty(propertyName);

            return settings?.GetValue<T>();
        }

        /// <summary>
        ///     Finds a property based on the property path relative to this object
        /// </summary>
        /// <param name="propertyPath">Property Path</param>
        /// <returns>Settings Object</returns>
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

        /// <summary>
        ///     Finds a property based on the property path relative to this object
        /// </summary>
        /// <param name="propertyPath">Property Path</param>
        /// <returns>Settings Object</returns>
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

        /// <summary>
        ///     Returns a string representation of the current object
        /// </summary>
        /// <returns>String Representation</returns>
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

        /// <summary>
        ///     Resolves Environment Variables in the current object
        /// </summary>
        /// <param name="root">The Root settings</param>
        /// <param name="parent">The Parent Object</param>
        /// <param name="str">The String to Expand</param>
        /// <returns>the Resolved variable</returns>
        /// <exception cref="Exception">
        ///     Gets raised if the environment variable syntax is invalid or the environment variable is
        ///     not found
        /// </exception>
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

        /// <summary>
        ///     Resolves Environment Variables in the current object
        /// </summary>
        /// <param name="root">The Root settings</param>
        public static void ResolveEnvironmentVariables(BadSettings root)
        {
            ResolveEnvironmentVariables(root, root, root);
        }

        /// <summary>
        ///     Resolves Environment Variables in the current object
        /// </summary>
        /// <param name="root">The Root settings</param>
        /// <param name="settings">The Current Settings Object</param>
        /// <param name="parent">The Parent Object</param>
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
}