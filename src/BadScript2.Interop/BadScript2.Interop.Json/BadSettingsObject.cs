using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Settings;

namespace BadScript2.Interop.Json;

/// <summary>
///     Implements a Settings Object Wrapper
/// </summary>
public class BadSettingsObject : BadObject
{
    /// <summary>
    ///     The Class Prototype
    /// </summary>
    public static readonly BadClassPrototype Prototype =
        new BadNativeClassPrototype<BadSettingsObject>("BadSettings", (_, args) => CreateObj(args));

    /// <summary>
    ///     Property References
    /// </summary>
    private readonly Dictionary<string, BadObjectReference> m_PropertyReferences;

    /// <summary>
    ///     Inner Settings Object
    /// </summary>
    private readonly BadSettings m_Settings;

    /// <summary>
    ///     Creates a new Settings Object
    /// </summary>
    /// <param name="settings">Inner Settings Object</param>
    public BadSettingsObject(BadSettings settings)
    {
        m_Settings = settings;

        Dictionary<string, BadObject> properties = new Dictionary<string, BadObject>
        {
            {
                "HasValue", new BadDynamicInteropFunction("HasValue",
                                                          _ => m_Settings.HasValue(),
                                                          BadNativeClassBuilder.GetNative("bool")
                                                         )
            },
            {
                "GetValue", new BadDynamicInteropFunction("GetValue",
                                                          _ => BadJson.ConvertNode(m_Settings.GetValue()),
                                                          BadAnyPrototype.Instance
                                                         )
            },
            {
                "SetValue", new BadDynamicInteropFunction<BadObject>("SetValue",
                                                                     (_, obj) =>
                                                                     {
                                                                         m_Settings.SetValue(BadJson.ConvertNode(obj));

                                                                         return Null;
                                                                     },
                                                                     BadAnyPrototype.Instance
                                                                    )
            },
            {
                "HasProperty", new BadDynamicInteropFunction<string>("HasProperty",
                                                                     (_, name) => m_Settings.HasProperty(name),
                                                                     BadNativeClassBuilder.GetNative("bool")
                                                                    )
            },
            {
                "GetProperty", new BadDynamicInteropFunction<string>("GetProperty",
                                                                     (_, name) =>
                                                                         new BadSettingsObject(m_Settings
                                                                                 .GetProperty(name)
                                                                             ),
                                                                     BadAnyPrototype.Instance
                                                                    )
            },
            {
                "FindProperty", new BadDynamicInteropFunction<string>("FindProperty",
                                                                      (_, name) =>
                                                                      {
                                                                          BadSettings? obj =
                                                                              m_Settings.FindProperty(name);

                                                                          return obj == null
                                                                              ? Null
                                                                              : new BadSettingsObject(obj);
                                                                      },
                                                                      BadAnyPrototype.Instance
                                                                     )
            },
            {
                "FindOrCreateProperty", new BadDynamicInteropFunction<string>("FindOrCreateProperty",
                                                                              (_, name) =>
                                                                              {
                                                                                  BadSettings obj =
                                                                                      m_Settings
                                                                                          .FindOrCreateProperty(name);

                                                                                  return new BadSettingsObject(obj);
                                                                              },
                                                                              BadAnyPrototype.Instance
                                                                             )
            },
            {
                "SetProperty", new BadDynamicInteropFunction<string, BadSettingsObject>("SetProperty",
                     (_, name, prop) =>
                     {
                         m_Settings.SetProperty(name, prop.m_Settings);

                         return Null;
                     },
                     BadAnyPrototype.Instance
                    )
            },
            {
                "RemoveProperty", new BadDynamicInteropFunction<string>("RemoveProperty",
                                                                        (_, name) => m_Settings.RemoveProperty(name),
                                                                        BadNativeClassBuilder.GetNative("bool")
                                                                       )
            },
            {
                "GetPropertyNames", new BadDynamicInteropFunction("GetPropertyNames",
                                                                  _ => new BadArray(m_Settings.PropertyNames
                                                                          .Select(x => (BadObject)x)
                                                                          .ToList()
                                                                      ),
                                                                  BadNativeClassBuilder.GetNative("Array")
                                                                 )
            },
            {
                "GetEnumerator", new BadDynamicInteropFunction("GetEnumerator",
                                                               _ => new BadInteropEnumerator(m_Settings.PropertyNames
                                                                       .Select(x => (BadObject)x)
                                                                       .GetEnumerator()
                                                                   ),
                                                               BadAnyPrototype.Instance
                                                              )
            },
            {
                BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
                new BadDynamicInteropFunction<string>(BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
                                                      (_, name) => BadObjectReference.Make($"BadSettings.{name}",
                                                           (p) => new BadSettingsObject(m_Settings.GetProperty(name)),
                                                           (o, p, _) =>
                                                           {
                                                               if (o is not BadSettingsObject obj)
                                                               {
                                                                   throw BadRuntimeException.Create(null, "BadSettingsObject expected", p
                                                                           );
                                                               }

                                                               m_Settings.SetProperty(name, obj.m_Settings);
                                                           }
                                                          ),
                                                      BadAnyPrototype.Instance
                                                     )
            },
        };

        m_PropertyReferences = new Dictionary<string, BadObjectReference>();

        foreach (KeyValuePair<string, BadObject> property in properties)
        {
            m_PropertyReferences.Add(property.Key,
                                     BadObjectReference.Make($"BadSettings.{property.Key}",
                                                             (p) => properties[property.Key]
                                                            )
                                    );
        }
    }

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    /// <summary>
    ///     Creates a new Settings Object
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <returns>BadSettingsObject</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the arguments are invalid</exception>
    private static BadObject CreateObj(IReadOnlyList<BadObject> args)
    {
        if (args.Count != 1)
        {
            return new BadSettingsObject(new BadSettings(string.Empty));
        }

        if (args[0] is not BadSettingsObject settings)
        {
            throw new BadRuntimeException("BadSettings expected");
        }

        return settings;
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return m_Settings.ToString();
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return m_PropertyReferences.ContainsKey(propName) ||
               m_Settings.HasProperty(propName) ||
               base.HasProperty(propName, caller);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        if (m_PropertyReferences.TryGetValue(propName, out BadObjectReference? property))
        {
            return property;
        }

        if (m_Settings.HasProperty(propName))
        {
            return BadObjectReference.Make($"BadSettings.{propName}",
                                           (p) => new BadSettingsObject(m_Settings.GetProperty(propName))
                                          );
        }

        return base.GetProperty(propName, caller);
    }
}