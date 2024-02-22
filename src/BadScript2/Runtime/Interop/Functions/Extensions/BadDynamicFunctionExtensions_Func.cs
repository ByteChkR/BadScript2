using BadScript2.Parser;
using BadScript2.Runtime.Interop.Reflection;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Functions.Extensions;

public class TestApi : BadInteropApi
{
    public TestApi(string name) : base(name) { }

    private string Test(string a, decimal b)
    {
        return a + b;
    }
    protected override void LoadApi(BadTable target)
    {
        T? GetParameter<T>(BadObject[] args, int i) => args.Length>i?args[i].Unwrap<T>():default;
        target.SetProperty(
            "Test",
            new BadInteropFunction(
                "Test",
                (ctx, args) => BadObject.Wrap(Test(GetParameter<string>(args, 0), GetParameter<decimal>(args, 1)), true),
                false,
                BadNativeClassBuilder.GetNative("string"),
                new BadFunctionParameter("a", false, false, false, null, BadNativeClassBuilder.GetNative("string")),
                new BadFunctionParameter("b", false, false, false, null, BadNativeClassBuilder.GetNative("num"))
            ).SetMetaData(
                new BadMetaData(
                    "FUNC DESC",
                    "RET DESC",
                    "string",
                    new Dictionary<string, BadParameterMetaData>
                    {
                        { "a", new BadParameterMetaData("string", "A DESC") },
                        { "b", new BadParameterMetaData("num", "B DESC") }
                    }
                )
            )
        );
    }
}

public static partial class BadDynamicFunctionExtensions
{
    /// <summary>
    /// Tries to convert a Type to a BadClassPrototype
    /// </summary>
    /// <param name="t">The Type to convert</param>
    /// <returns>The BadClassPrototype or null</returns>
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

        if (t.IsArray ||
            t.IsGenericType &&
            (t.GetGenericTypeDefinition() == typeof(List<>) || t.GetGenericTypeDefinition() == typeof(IList<>)))
        {
            return BadNativeClassBuilder.GetNative("Array");
        }

        //if (t.IsFunction() || t.IsAction()) return BadNativeClassBuilder.GetNative("Function");

        return null;
    }

    /// <summary>
    /// Returns a list of BadFunctionParameters for the given Types
    /// </summary>
    /// <param name="t">The Types to convert</param>
    /// <returns>The BadFunctionParameters</returns>
    private static BadFunctionParameter[] GetParameters(params Type[] t)
    {
        List<BadFunctionParameter> ret = new List<BadFunctionParameter>();
        bool canBeOptional = true;

        for (int i = t.Length - 1; i >= 0; i--)
        {
            Type type = t[i];
            bool isNullable = false;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BadNullable<>))
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

    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    
    public static void SetFunction(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(propName, new BadDynamicInteropFunction(propName, func, returnType));
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T">The Type of the first Argument</typeparam>
    public static void SetFunction<T>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T>(propName, func, returnType, GetParameters(typeof(T)))
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    public static void SetFunction<T1, T2>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2>(propName, func, returnType, GetParameters(typeof(T1), typeof(T2)))
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    public static void SetFunction<T1, T2, T3>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3>(
                propName,
                func,
                returnType,
                GetParameters(typeof(T1), typeof(T2), typeof(T3))
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4>(
                propName,
                func,
                returnType,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4))
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5>(
                propName,
                func,
                returnType,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5))
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6>(
                propName,
                func,
                returnType,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7>(
                propName,
                func,
                returnType,
                GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7))
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    /// <typeparam name="T8">The Type of the eighth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
                propName,
                func,
                returnType,
                GetParameters(
                    typeof(T1),
                    typeof(T2),
                    typeof(T3),
                    typeof(T4),
                    typeof(T5),
                    typeof(T6),
                    typeof(T7),
                    typeof(T8)
                )
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    /// <typeparam name="T8">The Type of the eighth Argument</typeparam>
    /// <typeparam name="T9">The Type of the ninth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
                propName,
                func,
                returnType,
                GetParameters(
                    typeof(T1),
                    typeof(T2),
                    typeof(T3),
                    typeof(T4),
                    typeof(T5),
                    typeof(T6),
                    typeof(T7),
                    typeof(T8),
                    typeof(T9)
                )
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    /// <typeparam name="T8">The Type of the eighth Argument</typeparam>
    /// <typeparam name="T9">The Type of the ninth Argument</typeparam>
    /// <typeparam name="T10">The Type of the tenth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
                propName,
                func,
                returnType,
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
                    typeof(T10)
                )
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    /// <typeparam name="T8">The Type of the eighth Argument</typeparam>
    /// <typeparam name="T9">The Type of the ninth Argument</typeparam>
    /// <typeparam name="T10">The Type of the tenth Argument</typeparam>
    /// <typeparam name="T11">The Type of the eleventh Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
                propName,
                func,
                returnType,
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
                    typeof(T11)
                )
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    /// <typeparam name="T8">The Type of the eighth Argument</typeparam>
    /// <typeparam name="T9">The Type of the ninth Argument</typeparam>
    /// <typeparam name="T10">The Type of the tenth Argument</typeparam>
    /// <typeparam name="T11">The Type of the eleventh Argument</typeparam>
    /// <typeparam name="T12">The Type of the twelfth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
                propName,
                func,
                returnType,
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
                    typeof(T12)
                )
            )
        );
    }
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    /// <typeparam name="T8">The Type of the eighth Argument</typeparam>
    /// <typeparam name="T9">The Type of the ninth Argument</typeparam>
    /// <typeparam name="T10">The Type of the tenth Argument</typeparam>
    /// <typeparam name="T11">The Type of the eleventh Argument</typeparam>
    /// <typeparam name="T12">The Type of the twelfth Argument</typeparam>
    /// <typeparam name="T13">The Type of the thirteenth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
                propName,
                func,
                returnType,
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
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    /// <typeparam name="T8">The Type of the eighth Argument</typeparam>
    /// <typeparam name="T9">The Type of the ninth Argument</typeparam>
    /// <typeparam name="T10">The Type of the tenth Argument</typeparam>
    /// <typeparam name="T11">The Type of the eleventh Argument</typeparam>
    /// <typeparam name="T12">The Type of the twelfth Argument</typeparam>
    /// <typeparam name="T13">The Type of the thirteenth Argument</typeparam>
    /// <typeparam name="T14">The Type of the fourteenth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
                propName,
                func,
                returnType,
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
    /// <summary>
    /// Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    /// <typeparam name="T5">The Type of the fifth Argument</typeparam>
    /// <typeparam name="T6">The Type of the sixth Argument</typeparam>
    /// <typeparam name="T7">The Type of the seventh Argument</typeparam>
    /// <typeparam name="T8">The Type of the eighth Argument</typeparam>
    /// <typeparam name="T9">The Type of the ninth Argument</typeparam>
    /// <typeparam name="T10">The Type of the tenth Argument</typeparam>
    /// <typeparam name="T11">The Type of the eleventh Argument</typeparam>
    /// <typeparam name="T12">The Type of the twelfth Argument</typeparam>
    /// <typeparam name="T13">The Type of the thirteenth Argument</typeparam>
    /// <typeparam name="T14">The Type of the fourteenth Argument</typeparam>
    /// <typeparam name="T15">The Type of the fifteenth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
        this BadObject elem,
        string propName,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetProperty(
            propName,
            new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                propName,
                func,
                returnType,
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