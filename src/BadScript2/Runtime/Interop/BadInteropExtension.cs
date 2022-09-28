using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.Interop
{
    /// <summary>
    ///     Public Extension API for the BS2 Runtime
    /// </summary>
    public abstract class BadInteropExtension
    {
        /// <summary>
        ///     List of all active extensions
        /// </summary>
        private static readonly List<Type> s_ActiveExtensions = new List<Type>();

        /// <summary>
        ///     Global extensions that are available for all objects in the runtime
        /// </summary>
        private static readonly Dictionary<BadObject, Func<BadObject, BadObject>> s_GlobalExtensions =
            new Dictionary<BadObject, Func<BadObject, BadObject>>();

        /// <summary>
        ///     Object Extensions that are available for objects of the specified type
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<BadObject, Func<BadObject, BadObject>>> s_ObjectExtensions =
            new Dictionary<Type, Dictionary<BadObject, Func<BadObject, BadObject>>>();

        /// <summary>
        ///     Cache for already built extension tables.
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<BadObject, Func<BadObject, BadObject>>> s_StaticExtensionCache =
            new Dictionary<Type, Dictionary<BadObject, Func<BadObject, BadObject>>>();

        /// <summary>
        ///     Returns all Global Extension names
        /// </summary>
        /// <returns>List of Extension Names</returns>
        public static BadObject[] GetExtensionNames()
        {
            return s_GlobalExtensions.Keys.ToArray();
        }

        /// <summary>
        ///     Returns all Extension Names for the specified object
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>List of Extension Names</returns>
        public static BadObject[] GetExtensionNames(BadObject obj)
        {
            Type t = obj.GetType();


            List<BadObject> objs = new List<BadObject>(GetExtensionNames());

            if (HasTypeExtensions(t))
            {
                objs.AddRange(GetTypeExtensions(t).Keys);
            }

            if (obj is IBadNative native)
            {
                t = native.Type;
                if (HasTypeExtensions(t))
                {
                    objs.AddRange(GetTypeExtensions(t).Keys);
                }
            }

            return objs.ToArray();
        }

        /// <summary>
        ///     Returns true if any extensions are available for the specified object(excluding global extensions)
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if any extensions are available</returns>
        private static bool HasTypeExtensions(Type t)
        {
            return s_ObjectExtensions.Any(x => x.Key.IsAssignableFrom(t));
        }

        /// <summary>
        ///     Returns true if a global extension with the specified name is available
        /// </summary>
        /// <param name="propName">Extension Name</param>
        /// <returns>True if the extension is available</returns>
        private static bool HasGlobalExtensions(BadObject propName)
        {
            return s_GlobalExtensions.ContainsKey(propName);
        }

        /// <summary>
        ///     Returns all type extensions for the specified type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Type Extensions</returns>
        private static Dictionary<BadObject, Func<BadObject, BadObject>> GetTypeExtensions(Type type)
        {
            if (s_StaticExtensionCache.ContainsKey(type))
            {
                return s_StaticExtensionCache[type];
            }

            Dictionary<BadObject, Func<BadObject, BadObject>>
                exts = new Dictionary<BadObject, Func<BadObject, BadObject>>();

            foreach (KeyValuePair<Type, Dictionary<BadObject, Func<BadObject, BadObject>>> kvp in s_ObjectExtensions.Where(
                         x => x.Key.IsAssignableFrom(type)
                     ))
            {
                foreach (KeyValuePair<BadObject, Func<BadObject, BadObject>> keyValuePair in kvp.Value)
                {
                    exts[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            if (BadNativeOptimizationSettings.Instance.UseStaticExtensionCaching)
            {
                s_StaticExtensionCache[type] = exts;
            }

            return exts;
        }

        /// <summary>
        ///     Registers the specified extension for all objects
        /// </summary>
        /// <param name="propName">The Extension Name</param>
        /// <param name="func">The Extension Provider Function</param>
        public static void RegisterGlobal(BadObject propName, Func<BadObject, BadObject> func)
        {
            s_GlobalExtensions.Add(propName, func);
        }

        /// <summary>
        ///     Registers the specified extension for all objects
        /// </summary>
        /// <param name="propName">The Extension Name</param>
        /// <param name="obj">The Extension</param>
        public static void RegisterGlobal(BadObject propName, BadObject obj)
        {
            RegisterGlobal(propName, _ => obj);
        }

        /// <summary>
        ///     Registers the specified extension for the specified type
        /// </summary>
        /// <param name="propName">The Extension Name</param>
        /// <param name="obj">The Extension</param>
        /// <typeparam name="T">Object Type</typeparam>
        /// <exception cref="BadRuntimeException">Gets raised if the provided object can not be cast to the type for the extension</exception>
        public static void RegisterObject<T>(BadObject propName, Func<T, BadObject> obj)
        {
            RegisterObject(
                typeof(T),
                propName,
                o =>
                {
                    if (o is T t)
                    {
                        return obj(t);
                    }

                    if (o is BadNative<T> nT)
                    {
                        return obj(nT.Value);
                    }

                    throw new BadRuntimeException("Cannot cast object to type " + typeof(T));
                }
            );
        }

        /// <summary>
        ///     Registers the specified extension for the specified type
        /// </summary>
        /// <param name="t">The Type</param>
        /// <param name="propName">The Extension Name</param>
        /// <param name="obj">The Extension</param>
        /// <exception cref="BadRuntimeException">Gets raised if the provided object can not be cast to the type for the extension</exception>
        public static void RegisterObject(Type t, BadObject propName, BadObject obj)
        {
            RegisterObject(t, propName, _ => obj);
        }

        /// <summary>
        ///     Registers the specified extension for the specified type
        /// </summary>
        /// <param name="propName">The Extension Name</param>
        /// <param name="obj">The Extension</param>
        /// <param name="t">The Type</param>
        /// <exception cref="BadRuntimeException">Gets raised if the provided object can not be cast to the type for the extension</exception>
        public static void RegisterObject(Type t, BadObject propName, Func<BadObject, BadObject> obj)
        {
            if (s_ObjectExtensions.ContainsKey(t))
            {
                s_ObjectExtensions[t][propName] = obj;
            }
            else
            {
                s_ObjectExtensions[t] = new Dictionary<BadObject, Func<BadObject, BadObject>> { { propName, obj } };
            }
        }


        /// <summary>
        ///     Returns True if the specified type has the specified extension
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="propName">Extension Name</param>
        /// <returns>True if the extension is available</returns>
        public static bool HasObject(Type t, BadObject propName)
        {
            return HasGlobalExtensions(propName) || HasTypeExtensions(t) && GetTypeExtensions(t).ContainsKey(propName);
        }

        /// <summary>
        ///     Returns True if the specified type has the specified extension
        /// </summary>
        /// <param name="propName">Extension Name</param>
        /// <typeparam name="T">Object Type</typeparam>
        /// <returns>True if the extension is available</returns>
        public static bool HasObject<T>(BadObject propName)
        {
            return HasObject(typeof(T), propName);
        }

        /// <summary>
        ///     Returns a reference to the specified extension
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="propName">Extension Name</param>
        /// <param name="instance">Object Instance</param>
        /// <returns>Object Reference</returns>
        public static BadObjectReference GetObjectReference(Type t, BadObject propName, BadObject instance)
        {
            return BadObjectReference.Make(
                $"{t.Name}.{propName}",
                () => GetObject(t, propName, instance)
            );
        }

        /// <summary>
        ///     Returns the specified extension
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="propName">Extension Name</param>
        /// <param name="instance">Object Instance</param>
        /// <returns>Object Instance</returns>
        public static BadObject GetObject(Type t, BadObject propName, BadObject instance)
        {
            if (HasGlobalExtensions(propName))
            {
                return s_GlobalExtensions[propName](instance);
            }

            return GetTypeExtensions(t)[propName](instance);
        }


        /// <summary>
        ///     Returns the specified extension
        /// </summary>
        /// <param name="propName">Extension Name</param>
        /// <param name="instance">Object Instance</param>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Object Instance</returns>
        public static BadObject GetObject<T>(BadObject propName, BadObject instance)
        {
            return GetObject(typeof(T), propName, instance);
        }

        /// <summary>
        ///     Adds an Extension Class to the List of registered extensions
        /// </summary>
        /// <typeparam name="T">Extension Type Class</typeparam>
        public static void AddExtension<T>() where T : BadInteropExtension, new()
        {
            if (s_ActiveExtensions.Contains(typeof(T)))
            {
                return;
            }

            T t = new T();
            t.Initialize();
        }

        /// <summary>
        ///     Initializes the Extension Class
        /// </summary>
        private void Initialize()
        {
            if (s_ActiveExtensions.Contains(GetType()))
            {
                return;
            }

            s_ActiveExtensions.Add(GetType());
            AddExtensions();
        }

        /// <summary>
        ///     Adds extensions to the list of registered extensions
        /// </summary>
        protected abstract void AddExtensions();
    }
}