using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.Objects
{
    public abstract class BadObject
    {
        private static readonly Dictionary<string, BadString> s_StringCache = new Dictionary<string, BadString>();
        public static readonly BadObject Null = new BadNullObject();
        public static readonly BadObject True = new BadBoolean(true);
        public static readonly BadObject False = new BadBoolean(false);
        public abstract BadClassPrototype GetPrototype();

        public static BadObject Wrap<T>(T obj)
        {
            if (obj is bool b)
            {
                if (b)
                {
                    return True;
                }

                return False;
            }

            if (obj is decimal d)
            {
                return new BadNumber(d);
            }

            if (obj is string s)
            {
                if (BadNativeOptimizationSettings.Instance.UseStringCaching)
                {
                    if (s_StringCache.ContainsKey(s))
                    {
                        return s_StringCache[s];
                    }

                    return s_StringCache[s] = new BadString(s);
                }

                return new BadString(s);
            }

            if (obj == null)
            {
                return Null;
            }

            return new BadNative<T>(obj);
        }

        public virtual bool HasProperty(BadObject propName)
        {
            return BadInteropExtension.HasObject(GetType(), propName);
        }

        public virtual BadObjectReference GetProperty(BadObject propName)
        {
            return BadInteropExtension.GetObjectReference(GetType(), propName, this);
        }

        public static implicit operator BadObject(bool b)
        {
            return Wrap(b);
        }

        public static implicit operator BadObject(decimal d)
        {
            return Wrap(d);
        }

        public static implicit operator BadObject(string s)
        {
            return Wrap(s);
        }

        public abstract string ToSafeString(List<BadObject> done);

        public override string ToString()
        {
            return ToSafeString(new List<BadObject>());
        }

        private class BadNullObject : BadObject, IBadNative
        {
            public object Value => null!;
            public Type Type => typeof(object);

            public bool Equals(IBadNative? other)
            {
                return Equals((object?)other);
            }

            public override BadClassPrototype GetPrototype()
            {
                return new BadNativeClassPrototype<BadNullObject>(
                    "null",
                    (_, _) => throw new BadRuntimeException("Cannot call methods on null")
                );
            }

            public override string ToSafeString(List<BadObject> done)
            {
                return "null";
            }


            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj);
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }
    }
}