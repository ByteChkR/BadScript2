using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Functions.Extensions;

public static partial class BadDynamicFunctionExtensions
{
	public static void SetFunction(
		this BadObject elem,
		string propName,
		Func<BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction(propName, _ => func(), returnType);
	}

	public static void SetFunction<T>(
		this BadObject elem,
		string propName,
		Func<T, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T>(propName, (_, t) => func(t), returnType);
	}

	public static void SetFunction<T1, T2>(
		this BadObject elem,
		string propName,
		Func<T1, T2, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2>(propName, (_, t1, t2) => func(t1, t2), returnType);
	}

	public static void SetFunction<T1, T2, T3>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3>(propName, (_, t1, t2, t3) => func(t1, t2, t3), returnType);
	}

	public static void SetFunction<T1, T2, T3, T4>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4>(propName, (_, t1, t2, t3, t4) => func(t1, t2, t3, t4), returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5) => func(t1, t2, t3, t4, t5),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6) => func(t1, t2, t3, t4, t5, t6),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7) => func(t1, t2, t3, t4, t5, t6, t7),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8) => func(t1, t2, t3, t4, t5, t6, t7, t8),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11,
				t12) => func(t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11,
				t12),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11,
				t12,
				t13) => func(t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11,
				t12,
				t13),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11,
				t12,
				t13,
				t14) => func(t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11,
				t12,
				t13,
				t14),
			returnType);
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
		this BadObject elem,
		string propName,
		Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func,
		BadClassPrototype returnType)
	{
		elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(propName,
			(
				_,
				t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11,
				t12,
				t13,
				t14,
				t15) => func(t1,
				t2,
				t3,
				t4,
				t5,
				t6,
				t7,
				t8,
				t9,
				t10,
				t11,
				t12,
				t13,
				t14,
				t15),
			returnType);
	}
}
