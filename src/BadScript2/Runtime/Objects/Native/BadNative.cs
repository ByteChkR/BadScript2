using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native
{
    public class BadNative<T> : BadObject, IBadNative
    {
        public readonly T Value;


        public BadNative(T value)
        {
            if (value == null)
            {
                throw new Exception("Can not construct native object with null value");
            }

            Value = value;
        }

        object IBadNative.Value => Value!;
        Type IBadNative.Type => Value!.GetType();


        public bool Equals(IBadNative? other)
        {
            if (other is null)
            {
                return false;
            }

            IBadNative thisN = this;

            return thisN.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value!);
        }


        public override string ToSafeString(List<BadObject> done)
        {
            return Value!.ToString()!;
        }

        public static bool operator ==(BadNative<T> a, BadObject b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BadNative<T> a, BadObject b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            return obj is IBadNative other && Equals(other);
        }

        public override BadClassPrototype GetPrototype()
        {
            return new BadNativeClassPrototype<BadNative<T>>(
                typeof(T).Name,
                (_, args) =>
                {
                    if (args.Length != 1 || args[0] is not BadNative<T> t)
                    {
                        throw new BadRuntimeException("BadNative<T> constructor takes one argument of type BadNative<T>");
                    }

                    return t;
                }
            );
        }

        public override bool HasProperty(BadObject propName)
        {
            return BadInteropExtension.HasObject<T>(propName);
        }

        public override BadObjectReference GetProperty(BadObject propName)
        {
            return BadObjectReference.Make(
                $"BadNative<{typeof(T).Name}>.{propName}",
                () => BadInteropExtension.GetObject<T>(propName, this)
            );
        }
    }
}