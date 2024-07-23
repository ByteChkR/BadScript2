using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;
namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native Type
/// </summary>
/// <typeparam name="T">The Type of the Native Type</typeparam>
public class BadNative<T> : BadObject, IBadNative
{
    /// <summary>
    ///     The Prototype for the Native Type
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadNative<T>>(
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

    /// <summary>
    ///     The Value of the Native Type
    /// </summary>
    private readonly T m_Value;


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

    /// <summary>
    ///     The Value of the Native Type
    /// </summary>
    public T Value => m_Value;

    /// <summary>
    ///     The Value of the Native Type
    /// </summary>
    object IBadNative.Value => m_Value!;

    /// <summary>
    ///     The Type of the Native Type
    /// </summary>
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

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value!);
    }


    /// <inheritdoc />
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

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return caller != null && caller.Provider.HasObject<T>(propName);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        return BadObjectReference.Make(
            $"BadNative<{typeof(T).Name}>.{propName}",
            () => caller != null
                ? caller.Provider.GetObject<T>(propName, this, caller)
                : throw BadRuntimeException.Create(caller, $"No property named {propName} for type {GetType().Name}")
        );
    }
}