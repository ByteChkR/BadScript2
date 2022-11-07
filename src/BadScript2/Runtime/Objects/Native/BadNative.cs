using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native Type
/// </summary>
/// <typeparam name="T">The Type of the Native Type</typeparam>
public class BadNative<T> : BadObject, IBadNative
{
    /// <summary>
    ///     The Value of the Native Type
    /// </summary>
    private readonly T m_Value;
    public T Value => m_Value;


    /// <summary>
    ///     Creates a new Native Type
    /// </summary>
    /// <param name="value">The Value</param>
    /// <exception cref="Exception">Gets raised if the <paramref name="value" /> is null</exception>
    public BadNative(T value)
    {
        if (value == null)
        {
            throw new Exception("Can not construct native object with null value");
        }

        m_Value = value;
    }

    object IBadNative.Value => m_Value!;
    Type IBadNative.Type => m_Value!.GetType();


    /// <summary>
    ///     Returns true if this and the other native types are equal
    /// </summary>
    /// <param name="other">Other Native Type</param>
    /// <returns>True if equal</returns>
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
        return m_Value!.ToString()!;
    }

    /// <summary>
    ///     Implements the == operator
    /// </summary>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>True if equal</returns>
    public static bool operator ==(BadNative<T> a, BadObject b)
    {
        return a.Equals(b);
    }

    /// <summary>
    ///     Implements the != operator
    /// </summary>
    /// <param name="a">Left</param>
    /// <param name="b">Right</param>
    /// <returns>True if not equal</returns>
    public static bool operator !=(BadNative<T> a, BadObject b)
    {
        return !(a == b);
    }

    /// <summary>
    ///     Returns true if this and the other objects are equal
    /// </summary>
    /// <param name="obj">Other Instance</param>
    /// <returns>True if equal</returns>
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