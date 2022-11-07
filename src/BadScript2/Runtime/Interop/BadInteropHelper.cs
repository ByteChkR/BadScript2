using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Reflection;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Interop;

/// <summary>
///     Interop Extensions for working with the runtime api
/// </summary>
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

    public static bool CanUnwrap(this BadObject obj)
    {
        return obj is IBadNative;
    }

    public static object? Unwrap(this BadObject obj)
    {
        if (obj is IBadNative native)
        {
            return native.Value;
        }

        throw new BadRuntimeException($"Can not unwrap object '{obj}'");
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

        if (obj is IBadNumber num && typeof(T).IsNumericType())
        {
            return (T)Convert.ChangeType(num.Value, typeof(T));
        }

        if (obj is BadNative<T> n)
        {
            return n.Value;
        }

        throw new BadRuntimeException($"Can not unwrap object '{obj}' to type " + typeof(T));
    }
}