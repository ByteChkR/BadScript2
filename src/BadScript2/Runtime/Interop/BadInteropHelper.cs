using System.Collections;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions.Extensions;
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

	public static object? Unwrap(this BadObject obj, BadScope? caller = null)
	{
		if (obj is IBadNative native)
		{
			return native.Value;
		}

		throw BadRuntimeException.Create(caller, $"Can not unwrap object '{obj}'");
	}

	public static object Unwrap(this BadObject obj, Type t, BadScope? caller = null)
	{
		Type oType = obj.GetType();

		if (t.IsAssignableFrom(oType))
		{
			return obj;
		}

		if (oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(BadNullable<>))
		{
			Type innerType = oType.GetGenericArguments()[0];

			if (obj == BadObject.Null)
			{
				return Activator.CreateInstance(typeof(BadNullable<>).MakeGenericType(innerType));
			}

			return Activator.CreateInstance(typeof(BadNullable<>).MakeGenericType(innerType),
				obj.Unwrap(innerType, caller));
		}

		if (obj is IBadString str && t == typeof(string))
		{
			return str.Value;
		}

		if (obj is IBadNumber num && t.IsNumericType())
		{
			return Convert.ChangeType(num.Value, t);
		}

		if (obj is IBadNative native && t.IsAssignableFrom(native.Type))
		{
			return native.Value;
		}

		if (obj is BadArray arr)
		{
			if (t.IsArray)
			{
				if (t.GetArrayRank() != 1)
				{
					throw BadRuntimeException.Create(caller, $"Can not unwrap object '{obj}' to type " + t);
				}

				object[] sarr = arr.InnerArray.Select(x => x.Unwrap(t.GetElementType()!, caller)).ToArray();
				Array rarr = Array.CreateInstance(t.GetElementType()!, arr.InnerArray.Count);

				for (int i = 0; i < sarr.Length; i++)
				{
					rarr.SetValue(sarr[i], i);
				}

				return rarr;
			}

			if (t.IsGenericType &&
			    (t.GetGenericTypeDefinition() == typeof(List<>) || t.GetGenericTypeDefinition() == typeof(IList<>)))
			{
				Type elemType = t.GetGenericArguments()[0];
				IEnumerable<object> sarr = arr.InnerArray.Select(x => x.Unwrap(elemType, caller));
				IList? rarr = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));

				foreach (object o in sarr)
				{
					rarr.Add(o);
				}

				return rarr;
			}
		}

		throw BadRuntimeException.Create(caller, $"Can not unwrap object '{obj}' to type " + t);
	}

	public static T Unwrap<T>(this BadObject obj, BadScope? caller = null)
	{
		if (obj is T t)
		{
			return t;
		}

		if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(BadNullable<>))
		{
			Type innerType = typeof(T).GetGenericArguments()[0];

			if (obj == BadObject.Null)
			{
				return (T)Activator.CreateInstance(typeof(BadNullable<>).MakeGenericType(innerType));
			}

			return (T)Activator.CreateInstance(typeof(BadNullable<>).MakeGenericType(innerType),
				obj.Unwrap(innerType, caller));
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

		if (obj is BadArray arr)
		{
			Type type = typeof(T);

			if (type.IsArray)
			{
				if (type.GetArrayRank() != 1)
				{
					throw BadRuntimeException.Create(caller, $"Can not unwrap object '{obj}' to type " + typeof(T));
				}

				return (T)(object)arr.InnerArray.Select(x => x.Unwrap(type.GetElementType()!, caller)).ToArray();
			}

			if (type.IsGenericType &&
			    (type.GetGenericTypeDefinition() == typeof(List<>) ||
			     type.GetGenericTypeDefinition() == typeof(IList<>)))
			{
				return (T)(object)arr.InnerArray.Select(x => x.Unwrap(type.GetElementType()!, caller)).ToList();
			}
		}

		throw BadRuntimeException.Create(caller, $"Can not unwrap object '{obj}' to type " + typeof(T));
	}

	public static bool IsFunction(this Type t)
	{
		if (!t.IsGenericType)
		{
			return false;
		}

		Type? gt = t.GetGenericTypeDefinition();

		return gt == typeof(Func<>) ||
		       gt == typeof(Func<,>) ||
		       gt == typeof(Func<,,>) ||
		       gt == typeof(Func<,,,>) ||
		       gt == typeof(Func<,,,,>) ||
		       gt == typeof(Func<,,,,,>) ||
		       gt == typeof(Func<,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,,,,,,,,>) ||
		       gt == typeof(Func<,,,,,,,,,,,,,,,,>);
	}

	public static bool IsAction(this Type t)
	{
		//Check if type is action or func of any kind
		if (t.IsGenericType)
		{
			Type? gt = t.GetGenericTypeDefinition();

			return gt == typeof(Action<>) ||
			       gt == typeof(Action<,>) ||
			       gt == typeof(Action<,,>) ||
			       gt == typeof(Action<,,,>) ||
			       gt == typeof(Action<,,,,>) ||
			       gt == typeof(Action<,,,,,>) ||
			       gt == typeof(Action<,,,,,,>) ||
			       gt == typeof(Action<,,,,,,,>) ||
			       gt == typeof(Action<,,,,,,,,>) ||
			       gt == typeof(Action<,,,,,,,,,>) ||
			       gt == typeof(Action<,,,,,,,,,,>) ||
			       gt == typeof(Action<,,,,,,,,,,,>) ||
			       gt == typeof(Action<,,,,,,,,,,,,>) ||
			       gt == typeof(Action<,,,,,,,,,,,,,>) ||
			       gt == typeof(Action<,,,,,,,,,,,,,,,>);
		}

		return t == typeof(Action);
	}
}
