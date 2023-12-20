using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Interop.Reflection;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.Objects;

public abstract class BadObject
{
	private static readonly BadClassPrototype s_Prototype = BadAnyPrototype.Instance;

	private static readonly Dictionary<string, BadString> s_StringCache = new Dictionary<string, BadString>();
	public static readonly BadObject Null = new BadNullObject();
	public static readonly BadObject True = new BadBoolean(true);
	public static readonly BadObject False = new BadBoolean(false);

	/// <summary>
	///     Returns the Prototype of this Object
	/// </summary>
	/// <returns>Instance of the ClassPrototype associated to this Type of BadObject</returns>
	public abstract BadClassPrototype GetPrototype();

	public static bool CanWrap(object? o)
	{
		return o is string || o is decimal || o is null || o.GetType().IsNumericType();
	}

	public static BadObject Wrap<T>(T obj, bool allowNative = true)
	{
		if (obj is BadObject bObj)
		{
			return bObj;
		}

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

		if (typeof(T).IsNumericType() || (obj != null && obj.GetType().IsNumericType()))
		{
			return new BadNumber(Convert.ToDecimal(obj));
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
	/// <returns>True if the Property or an Extension with that name exists</returns>
	public virtual bool HasProperty(BadObject propName, BadScope? caller = null)
	{
		if (caller == null)
		{
			return false;
		}

		return caller.Provider.HasObject(GetType(), propName);
	}

	/// <summary>
	///     Returns a Reference to the Property with the given Name
	/// </summary>
	/// <param name="propName">The Property Name</param>
	/// <returns>The Property Reference</returns>
	public virtual BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
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

	public static implicit operator BadObject(BadNullable<bool> b)
	{
		return b.HasValue ? b.Value : Null;
	}

	/// <summary>
	///     Implicit Converstion from Number to BadObject
	/// </summary>
	/// <param name="b">The Value</param>
	/// <returns>Bad Object Instance</returns>
	public static implicit operator BadObject(decimal d)
	{
		return Wrap(d);
	}

	public static implicit operator BadObject(BadNullable<decimal> b)
	{
		return b.HasValue ? b.Value : Null;
	}

	/// <summary>
	///     Implicit Converstion from String to BadObject
	/// </summary>
	/// <param name="b">The Value</param>
	/// <returns>Bad Object Instance</returns>
	public static implicit operator BadObject(string s)
	{
		return Wrap(s);
	}

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
		public object Value => null!;

		public Type Type => typeof(object);

		public bool Equals(IBadNative? other)
		{
			return Equals((object?)other);
		}

		public override BadClassPrototype GetPrototype()
		{
			return s_Prototype;
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
