using BadScript2.Runtime.Interop.Reflection;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;


namespace BadScript2.Runtime.Interop.Functions.Extensions;

public static partial class BadDynamicFunctionExtensions
{
    private static BadClassPrototype? TryConvertType(Type t)
    {
        if (t == typeof(string))
        {
            return BadNativeClassBuilder.GetNative("string");
        }

        if (t == typeof(bool))
        {
            return BadNativeClassBuilder.GetNative("bool");
        }

        if (t.IsNumericType())
        {
            return BadNativeClassBuilder.GetNative("num");
        }

        if (t.IsArray || t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>) || t.GetGenericTypeDefinition() == typeof(IList<>)))
        {
            return BadNativeClassBuilder.GetNative("Array");
        }

        //if (t.IsFunction() || t.IsAction()) return BadNativeClassBuilder.GetNative("Function");

        return null;
    }

    private static BadFunctionParameter[] GetParameters(params Type[] t)
    {
        List<BadFunctionParameter> ret = new List<BadFunctionParameter>();
        bool canBeOptional = true;
        for (int i = t.Length - 1; i >= 0; i--)
        {
            Type type = t[i];
            bool isNullable = false;
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BadNullable<>))
            {
                isNullable = true;
                type = type.GetGenericArguments()[0];
            }
            bool optional = canBeOptional && isNullable;
            bool nullChecked = !isNullable;
            BadClassPrototype? bType = TryConvertType(type);
            ret.Insert(0, new BadFunctionParameter(type.Name, optional, nullChecked, false, null, bType));

            if (!isNullable)
            {
                canBeOptional = false;
            }
        }

        return ret.ToArray();
    }


    public static void SetFunction(this BadObject elem, string propName, Func<BadExecutionContext, BadObject> func)
    {
        elem.SetProperty(propName, new BadDynamicInteropFunction(propName, func));
    }

    public static void SetFunction<T>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T, BadObject> func)
    {
        elem.SetProperty(propName, new BadDynamicInteropFunction<T>(propName, func, GetParameters(typeof(T))));
    }

    public static void SetFunction<T1, T2>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2>(propName, func, GetParameters(typeof(T1), typeof(T2)))
        );
    }

    public static void SetFunction<T1, T2, T3>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
                propName,
                func,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12))
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
                propName,
                func,
                GetParameters(
                    typeof(T1),
                    typeof(T2),
                    typeof(T3),
                    typeof(T4),
                    typeof(T5),
                    typeof(T6),
                    typeof(T7),
                    typeof(T8),
                    typeof(T9),
                    typeof(T10),
                    typeof(T11),
                    typeof(T12),
                    typeof(T13)
                )
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
                propName,
                func,
                GetParameters(
                    typeof(T1),
                    typeof(T2),
                    typeof(T3),
                    typeof(T4),
                    typeof(T5),
                    typeof(T6),
                    typeof(T7),
                    typeof(T8),
                    typeof(T9),
                    typeof(T10),
                    typeof(T11),
                    typeof(T12),
                    typeof(T13),
                    typeof(T14)
                )
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                propName,
                func,
                GetParameters(
                    typeof(T1),
                    typeof(T2),
                    typeof(T3),
                    typeof(T4),
                    typeof(T5),
                    typeof(T6),
                    typeof(T7),
                    typeof(T8),
                    typeof(T9),
                    typeof(T10),
                    typeof(T11),
                    typeof(T12),
                    typeof(T13),
                    typeof(T14),
                    typeof(T15)
                )
            )
        );
    }
}