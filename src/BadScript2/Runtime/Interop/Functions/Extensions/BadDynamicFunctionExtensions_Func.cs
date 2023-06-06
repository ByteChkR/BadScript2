using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Functions.Extensions;

public static partial class BadDynamicFunctionExtensions
{
	public static void SetFunction(this BadObject elem, string propName, Func<BadExecutionContext, BadObject> func)
	{
		elem.SetProperty(propName, new BadDynamicInteropFunction(propName, func));
	}

	public static void SetFunction<T>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T, BadObject> func)
	{
		elem.SetProperty(propName, new BadDynamicInteropFunction<T>(propName, func, typeof(T).Name));
	}

	public static void SetFunction<T1, T2>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2>(propName, func, typeof(T1).Name, typeof(T2).Name));
	}

	public static void SetFunction<T1, T2, T3>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name));
	}

	public static void SetFunction<T1, T2, T3, T4>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name,
				typeof(T8).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name,
				typeof(T8).Name,
				typeof(T9).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name,
				typeof(T8).Name,
				typeof(T9).Name,
				typeof(T10).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name,
				typeof(T8).Name,
				typeof(T9).Name,
				typeof(T10).Name,
				typeof(T11).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name,
				typeof(T8).Name,
				typeof(T9).Name,
				typeof(T10).Name,
				typeof(T11).Name,
				typeof(T12).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name,
				typeof(T8).Name,
				typeof(T9).Name,
				typeof(T10).Name,
				typeof(T11).Name,
				typeof(T12).Name,
				typeof(T13).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name,
				typeof(T8).Name,
				typeof(T9).Name,
				typeof(T10).Name,
				typeof(T11).Name,
				typeof(T12).Name,
				typeof(T13).Name,
				typeof(T14).Name));
	}

	public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
		this BadObject elem,
		string propName,
		Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func)
	{
		elem.SetProperty(propName,
			new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(propName,
				func,
				typeof(T1).Name,
				typeof(T2).Name,
				typeof(T3).Name,
				typeof(T4).Name,
				typeof(T5).Name,
				typeof(T6).Name,
				typeof(T7).Name,
				typeof(T8).Name,
				typeof(T9).Name,
				typeof(T10).Name,
				typeof(T11).Name,
				typeof(T12).Name,
				typeof(T13).Name,
				typeof(T14).Name,
				typeof(T15).Name));
	}
}
