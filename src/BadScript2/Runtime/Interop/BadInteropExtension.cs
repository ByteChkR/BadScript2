using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.Interop
{
    public abstract class BadInteropExtension
    {
        private static readonly List<Type> s_ActiveExtensions = new List<Type>();

        private static readonly Dictionary<BadObject, Func<BadObject, BadObject>> s_GlobalExtensions =
            new Dictionary<BadObject, Func<BadObject, BadObject>>();

        private static readonly Dictionary<Type, Dictionary<BadObject, Func<BadObject, BadObject>>> s_ObjectExtensions =
            new Dictionary<Type, Dictionary<BadObject, Func<BadObject, BadObject>>>();

        private static readonly Dictionary<Type, Dictionary<BadObject, Func<BadObject, BadObject>>> s_StaticExtensionCache =
            new Dictionary<Type, Dictionary<BadObject, Func<BadObject, BadObject>>>();

        public static BadObject[] GetExtensionNames()
        {
            return s_GlobalExtensions.Keys.ToArray();
        }

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

        private static bool HasTypeExtensions(Type t)
        {
            return s_ObjectExtensions.Any(x => x.Key.IsAssignableFrom(t));
        }

        private static bool HasGlobalExtensions(BadObject propName)
        {
            return s_GlobalExtensions.ContainsKey(propName);
        }

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

        public static void RegisterGlobal(BadObject propName, Func<BadObject, BadObject> func)
        {
            s_GlobalExtensions.Add(propName, func);
        }

        public static void RegisterGlobal(BadObject propName, BadObject obj)
        {
            RegisterGlobal(propName, _ => obj);
        }

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

        public static void RegisterObject(Type t, BadObject propName, BadObject obj)
        {
            RegisterObject(t, propName, _ => obj);
        }

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


        public static bool HasObject(Type t, BadObject propName)
        {
            return HasGlobalExtensions(propName) || (HasTypeExtensions(t) && GetTypeExtensions(t).ContainsKey(propName));
        }

        public static bool HasObject<T>(BadObject propName)
        {
            return HasObject(typeof(T), propName);
        }

        public static BadObjectReference GetObjectReference(Type t, BadObject propName, BadObject instance)
        {
            return BadObjectReference.Make(
                $"{t.Name}.{propName}",
                () => GetObject(t, propName, instance)
            );
        }

        public static BadObject GetObject(Type t, BadObject propName, BadObject instance)
        {
            if (HasGlobalExtensions(propName))
            {
                return s_GlobalExtensions[propName](instance);
            }

            return GetTypeExtensions(t)[propName](instance);
        }

        public static BadObject GetObject<T>(BadObject propName, BadObject instance)
        {
            return GetObject(typeof(T), propName, instance);
        }

        public static void AddExtension<T>() where T : BadInteropExtension, new()
        {
            if (s_ActiveExtensions.Contains(typeof(T)))
            {
                return;
            }

            T t = new T();
            t.Initialize();
        }

        public void Initialize()
        {
            if (s_ActiveExtensions.Contains(GetType()))
            {
                return;
            }

            s_ActiveExtensions.Add(GetType());
            AddExtensions();
        }

        protected abstract void AddExtensions();
    }
}