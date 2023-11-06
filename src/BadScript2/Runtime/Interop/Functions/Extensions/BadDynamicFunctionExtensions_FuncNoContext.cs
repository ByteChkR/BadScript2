using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Functions.Extensions;

public static partial class BadDynamicFunctionExtensions
{
    public static void SetFunction(this BadObject elem, string propName, Func<BadObject> func)
    {
        elem.SetFunction(propName, _ => func());
    }

    public static void SetFunction<T>(this BadObject elem, string propName, Func<T, BadObject> func)
    {
        elem.SetFunction<T>(propName, (_, t) => func(t));
    }

    public static void SetFunction<T1, T2>(this BadObject elem, string propName, Func<T1, T2, BadObject> func)
    {
        elem.SetFunction<T1, T2>(propName, (_, t1, t2) => func(t1, t2));
    }

    public static void SetFunction<T1, T2, T3>(this BadObject elem, string propName, Func<T1, T2, T3, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3>(propName, (_, t1, t2, t3) => func(t1, t2, t3));
    }

    public static void SetFunction<T1, T2, T3, T4>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4>(propName, (_, t1, t2, t3, t4) => func(t1, t2, t3, t4));
    }

    public static void SetFunction<T1, T2, T3, T4, T5>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5>(
            propName,
            (
                _,
                t1,
                t2,
                t3,
                t4,
                t5) => func(t1, t2, t3, t4, t5)
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6>(
            propName,
            (
                _,
                t1,
                t2,
                t3,
                t4,
                t5,
                t6) => func(t1, t2, t3, t4, t5, t6)
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7>(
            propName,
            (
                _,
                t1,
                t2,
                t3,
                t4,
                t5,
                t6,
                t7) => func(t1, t2, t3, t4, t5, t6, t7)
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
            propName,
            (
                _,
                t1,
                t2,
                t3,
                t4,
                t5,
                t6,
                t7,
                t8) => func(t1, t2, t3, t4, t5, t6, t7, t8)
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            propName,
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
                t9) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9)
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            propName,
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
                t10) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10)
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            propName,
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
                t11) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11)
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            propName,
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
                t12) => func(
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
                t12
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            propName,
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
                t13) => func(
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
                t13
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            propName,
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
                t14) => func(
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
                t14
            )
        );
    }

    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
        this BadObject elem,
        string propName,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            propName,
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
                t15) => func(
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
                t15
            )
        );
    }
}