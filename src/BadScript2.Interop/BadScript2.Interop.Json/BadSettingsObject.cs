using BadScript2.Common;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Settings;

namespace BadScript2.Interop.Json;

public class BadSettingsObject : BadObject
{
    public static readonly BadClassPrototype Prototype =
        new BadNativeClassPrototype<BadSettingsObject>("BadSettings", (_, args) => CreateObj(args));

    private readonly Dictionary<BadObject, BadObject> m_Properties;
    private readonly Dictionary<BadObject, BadObjectReference> m_PropertyReferences;
    private readonly BadSettings m_Settings;

    public BadSettingsObject(BadSettings settings)
    {
        m_Settings = settings;
        m_Properties = new Dictionary<BadObject, BadObject>
        {
            { "HasValue", new BadDynamicInteropFunction("HasValue", _ => m_Settings.HasValue()) },
            { "GetValue", new BadDynamicInteropFunction("GetValue", _ => BadJson.ConvertNode(m_Settings.GetValue())) },
            {
                "SetValue", new BadDynamicInteropFunction<BadObject>(
                    "SetValue",
                    (_, obj) =>
                    {
                        m_Settings.SetValue(BadJson.ConvertNode(obj));

                        return Null;
                    }
                )
            },
            {
                "HasProperty",
                new BadDynamicInteropFunction<string>("HasProperty", (_, name) => m_Settings.HasProperty(name))
            },
            {
                "GetProperty",
                new BadDynamicInteropFunction<string>(
                    "GetProperty",
                    (_, name) => new BadSettingsObject(m_Settings.GetProperty(name))
                )
            },
            {
                "FindProperty",
                new BadDynamicInteropFunction<string>(
                    "FindProperty",
                    (_, name) =>
                    {
                        BadSettings? obj = m_Settings.FindProperty(name);

                        if (obj == null)
                        {
                            return Null;
                        }

                        return new BadSettingsObject(obj);
                    }
                )
            },
            {
                "FindOrCreateProperty",
                new BadDynamicInteropFunction<string>(
                    "FindOrCreateProperty",
                    (_, name) =>
                    {
                        BadSettings obj = m_Settings.FindOrCreateProperty(name);

                        return new BadSettingsObject(obj);
                    }
                )
            },
            {
                "SetProperty",
                new BadDynamicInteropFunction<string, BadSettingsObject>(
                    "SetProperty",
                    (_, name, prop) =>
                    {
                        m_Settings.SetProperty(name, prop.m_Settings);

                        return Null;
                    }
                )
            },
            {
                "RemoveProperty", new BadDynamicInteropFunction<string>(
                    "RemoveProperty",
                    (_, name) =>
                    {
                        m_Settings.RemoveProperty(name);

                        return Null;
                    }
                )
            },
            {
                "GetPropertyNames",
                new BadDynamicInteropFunction(
                    "GetPropertyNames",
                    _ => new BadArray(m_Settings.PropertyNames.Select(x => (BadObject)x).ToList())
                )
            },
            {
                "GetEnumerator",
                new BadDynamicInteropFunction(
                    "GetEnumerator",
                    _ => new BadInteropEnumerator(m_Settings.PropertyNames.Select(x => (BadObject)x).GetEnumerator())
                )
            },
            {
                BadStaticKeys.ArrayAccessOperatorName,
                new BadDynamicInteropFunction<string>(
                    BadStaticKeys.ArrayAccessOperatorName,
                    (_, name) => BadObjectReference.Make(
                        $"BadSettings.{name}",
                        () => new BadSettingsObject(m_Settings.GetProperty(name)),
                        (o, _) =>
                        {
                            if (o is not BadSettingsObject obj)
                            {
                                throw new BadRuntimeException("BadSettingsObject expected");
                            }

                            m_Settings.SetProperty(name, obj.m_Settings);
                        }
                    )
                )
            },
        };

        m_PropertyReferences = new Dictionary<BadObject, BadObjectReference>();
        foreach (KeyValuePair<BadObject, BadObject> property in m_Properties)
        {
            m_PropertyReferences.Add(
                property.Key,
                BadObjectReference.Make($"BadSettings.{property.Key}", () => m_Properties[property.Key])
            );
        }
    }

    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    private static BadObject CreateObj(BadObject[] args)
    {
        if (args.Length == 1)
        {
            if (args[0] is not BadSettingsObject settings)
            {
                throw new BadRuntimeException("BadSettings expected");
            }

            return settings;
        }

        return new BadSettingsObject(new BadSettings());
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return m_Settings.ToString();
    }

    public override bool HasProperty(BadObject propName)
    {
        return m_PropertyReferences.ContainsKey(propName) ||
               propName is IBadString str && m_Settings.HasProperty(str.Value) ||
               base.HasProperty(propName);
    }

    public override BadObjectReference GetProperty(BadObject propName)
    {
        if (m_PropertyReferences.ContainsKey(propName))
        {
            return m_PropertyReferences[propName];
        }

        if (propName is IBadString str && m_Settings.HasProperty(str.Value))
        {
            return BadObjectReference.Make(
                $"BadSettings.{propName}",
                () => new BadSettingsObject(m_Settings.GetProperty(str.Value))
            );
        }

        return base.GetProperty(propName);
    }
}