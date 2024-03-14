using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Interop.Reflection;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.Objects;

/// <summary>
///     The Base Class for all BadScript Objects
/// </summary>
public abstract class BadObject
{
    /// <summary>
    ///     The Any Prototype for the BadScript Language
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = BadAnyPrototype.Instance;

    /// <summary>
    ///     The String Cache for the BadScript Language
    /// </summary>
    private static readonly Dictionary<string, BadString> s_StringCache = new Dictionary<string, BadString>();

    /// <summary>
    ///     The Null Value for the BadScript Language
    /// </summary>
    public static readonly BadObject Null = new BadNullObject();

    /// <summary>
    ///     The True Value for the BadScript Language
    /// </summary>
    public static readonly BadObject True = new BadBoolean(true);

    /// <summary>
    ///     The False Value for the BadScript Language
    /// </summary>
    public static readonly BadObject False = new BadBoolean(false);

    /// <summary>
    ///     Returns the Prototype of this Object
    /// </summary>
    /// <returns>Instance of the ClassPrototype associated to this Type of BadObject</returns>
    public abstract BadClassPrototype GetPrototype();

    /// <summary>
    ///     Returns true if the given object cam be wrapped
    /// </summary>
    /// <param name="o">The Object</param>
    /// <returns>True if the given object cam be wrapped</returns>
    public static bool CanWrap(object? o)
    {
        return o is string || o is decimal || o is null || o.GetType().IsNumericType();
    }

    /// <summary>
    ///     Wraps the given object into a BadObject Instance
    /// </summary>
    /// <param name="obj">The Object</param>
    /// <param name="allowNative">Allow Native Wrapping</param>
    /// <typeparam name="T">The Type of the Object</typeparam>
    /// <returns>The Wrapped Object</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the object cannot be wrapped</exception>
    public static BadObject Wrap<T>(T obj, bool allowNative = true)
    {
        switch (obj)
        {
            case BadObject bObj:
                return bObj;
            case bool b when b:
                return True;
            case bool:
                return False;
            case decimal d:
                return new BadNumber(d);
        }

        if (typeof(T).IsNumericType() || obj != null && obj.GetType().IsNumericType())
        {
            return new BadNumber(Convert.ToDecimal(obj));
        }

        if (obj is string s)
        {
            if (!BadNativeOptimizationSettings.Instance.UseStringCaching)
            {
                return new BadString(s);
            }

            if (s_StringCache.TryGetValue(s, out BadString? wrap))
            {
                return wrap;
            }

            return s_StringCache[s] = new BadString(s);
        }

        if (Equals(obj, default(T)))
        {
            return Null;
        }


        if (allowNative)
        {
            return new BadNative<T>(obj);
        }

        throw new BadRuntimeException("Cannot wrap native type");
    }

    /// <summary>
    ///     Returns true if the object contains a given property or there exists an extension for the current Instance
    /// </summary>
    /// <param name="propName">The Property Name</param>
    /// <param name="caller">The caller Scope</param>
    /// <returns>True if the Property or an Extension with that name exists</returns>
    public virtual bool HasProperty(string propName, BadScope? caller = null)
    {
        return caller != null && caller.Provider.HasObject(GetType(), propName);
    }

    /// <summary>
    ///     Returns a Reference to the Property with the given Name
    /// </summary>
    /// <param name="propName">The Property Name</param>
    /// <param name="caller">The caller Scope</param>
    /// <returns>The Property Reference</returns>
    public virtual BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        if (caller == null)
        {
            throw BadRuntimeException.Create(caller, $"No property named {propName} for type {GetType().Name}");
        }

        return caller.Provider.GetObjectReference(GetType(), propName, this, caller);
    }

    /// <summary>
    ///     Implicit Converstion from Boolean to BadObject
    /// </summary>
    /// <param name="b">The Value</param>
    /// <returns>Bad Object Instance</returns>
    public static implicit operator BadObject(bool b)
    {
        return Wrap(b);
    }

    /// <summary>
    ///     Converts the given object to a BadObject Instance
    /// </summary>
    /// <param name="b">The Object</param>
    /// <returns>The BadObject Instance</returns>
    public static implicit operator BadObject(BadNullable<bool> b)
    {
        return b.HasValue ? b.Value : Null;
    }

    /// <summary>
    ///     Implicit Converstion from Number to BadObject
    /// </summary>
    /// <param name="d">The Value</param>
    /// <returns>Bad Object Instance</returns>
    public static implicit operator BadObject(decimal d)
    {
        return Wrap(d);
    }

    /// <summary>
    ///     Converts the given object to a BadObject Instance
    /// </summary>
    /// <param name="b">The Object</param>
    /// <returns>The BadObject Instance</returns>
    public static implicit operator BadObject(BadNullable<decimal> b)
    {
        return b.HasValue ? b.Value : Null;
    }

    /// <summary>
    ///     Implicit Converstion from String to BadObject
    /// </summary>
    /// <param name="s">The Value</param>
    /// <returns>Bad Object Instance</returns>
    public static implicit operator BadObject(string s)
    {
        return Wrap(s);
    }

    /// <summary>
    ///     Converts the given object to a BadObject Instance
    /// </summary>
    /// <param name="b">The Object</param>
    /// <returns>The BadObject Instance</returns>
    public static implicit operator BadObject(BadNullable<string> b)
    {
        return b.HasValue ? b.Value! : Null;
    }

    /// <summary>
    ///     Returns a String Representation of this Object. This function is recursion proof and supports circular references
    /// </summary>
    /// <param name="done">The Visited Elements</param>
    /// <returns>String Representation</returns>
    public abstract string ToSafeString(List<BadObject> done);

    /// <summary>
    ///     Returns a String Representation of this Object.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return ToSafeString(new List<BadObject>());
    }

    /// <summary>
    ///     Implementation for the null-value
    /// </summary>
    private class BadNullObject : BadObject, IBadNative
    {
        /// <inheritdoc cref="IBadNative.Value" />
        public object Value => null!;

        /// <inheritdoc cref="IBadNative.Type" />
        public Type Type => typeof(object);

        /// <summary>
        ///     Returns the Prototype for the Null Object
        /// </summary>
        /// <param name="other">The Other Object</param>
        /// <returns>True if the Objects are equal</returns>
        public bool Equals(IBadNative? other)
        {
            return Equals((object?)other);
        }


        /// <inheritdoc />
        public override BadClassPrototype GetPrototype()
        {
            return s_Prototype;
        }

        /// <inheritdoc />
        public override string ToSafeString(List<BadObject> done)
        {
            return "null";
        }


        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }
    }
}