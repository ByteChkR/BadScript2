using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Interop;

public static class BadInteropHelper
{
    public static void SetProperty(
        this BadObject elem,
        BadObject propName,
        BadObject value,
        BadPropertyInfo? info = null)
    {
        elem.GetProperty(propName).Set(value, info);
    }

    public static T Unwrap<T>(this BadObject obj)
    {
        if (obj is T t)
        {
            return t;
        }

        if (obj is IBadString str && typeof(T) == typeof(string))
        {
            return (T)(object)str.Value;
        }

        if (obj is BadNative<T> n)
        {
            return n.Value;
        }

        throw new BadRuntimeException($"Can not unwrap object '{obj}' to type " + typeof(T));
    }
}