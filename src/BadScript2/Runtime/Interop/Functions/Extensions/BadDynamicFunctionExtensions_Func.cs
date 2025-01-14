using BadScript2.Runtime.Interop.Reflection;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Functions.Extensions;

public static partial class BadDynamicFunctionExtensions
{
    /// <summary>
    ///     Tries to convert a Type to a BadClassPrototype
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
            (t.IsGenericType &&
             (t.GetGenericTypeDefinition() == typeof(List<>) || t.GetGenericTypeDefinition() == typeof(IList<>))))
        {
            return BadNativeClassBuilder.GetNative("Array");
        }

        //if (t.IsFunction() || t.IsAction()) return BadNativeClassBuilder.GetNative("Function");

        return null;
    }
    /// <summary>
    ///     Returns a list of BadFunctionParameters for the given Types
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
    ///     Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    public static void SetFunction(this BadObject elem,
                                   string propName,
                                   Func<BadExecutionContext, BadObject> func,
                                   BadClassPrototype returnType)
    {
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction(propName, func, returnType),
                         new BadPropertyInfo(BadFunction.Prototype, true)
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T">The Type of the first Argument</typeparam>
    public static void SetFunction<T>(this BadObject elem,
                                      string propName,
                                      Func<BadExecutionContext, T, BadObject> func,
                                      BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T>(propName, func, returnType, parameters[0]),
                         new BadPropertyInfo(BadFunction.Prototype, true)
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    public static void SetFunction<T1, T2>(this BadObject elem,
                                           string propName,
                                           Func<BadExecutionContext, T1, T2, BadObject> func,
                                           BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2>(propName,
                                                               func,
                                                               returnType,
                                                               parameters[0], parameters[1]
                                                              )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    public static void SetFunction<T1, T2, T3>(this BadObject elem,
                                               string propName,
                                               Func<BadExecutionContext, T1, T2, T3, BadObject> func,
                                               BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3>(propName,
                                                                   func,
                                                                   returnType,
                                                                     parameters[0], parameters[1], parameters[2]
                                                                  )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <typeparam name="T1">The Type of the first Argument</typeparam>
    /// <typeparam name="T2">The Type of the second Argument</typeparam>
    /// <typeparam name="T3">The Type of the third Argument</typeparam>
    /// <typeparam name="T4">The Type of the fourth Argument</typeparam>
    public static void SetFunction<T1, T2, T3, T4>(this BadObject elem,
                                                   string propName,
                                                   Func<BadExecutionContext, T1, T2, T3, T4, BadObject> func,
                                                   BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4>(propName,
                                                                       func,
                                                                       returnType,
                                                                          parameters[0], parameters[1], parameters[2], parameters[3]
                                                                      )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5>(this BadObject elem,
                                                       string propName,
                                                       Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> func,
                                                       BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5>(propName,
                                                                           func,
                                                                           returnType,
                                                                                parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]
                                                                          )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6>(this BadObject elem,
                                                           string propName,
                                                           Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject>
                                                               func,
                                                           BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6>(propName,
                                                                               func,
                                                                               returnType,
                                                                               parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]
                                                                              )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7>(this BadObject elem,
                                                               string propName,
                                                               Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7,
                                                                   BadObject> func,
                                                               BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7>(propName,
                              func,
                              returnType,
                                parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6]
                             )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(this BadObject elem,
                                                                   string propName,
                                                                   Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7,
                                                                       T8, BadObject> func,
                                                                   BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8>(propName,
                              func,
                              returnType,
                              parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7]
                             )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this BadObject elem,
                                                                       string propName,
                                                                       Func<BadExecutionContext, T1, T2, T3, T4, T5, T6,
                                                                           T7, T8, T9, BadObject> func,
                                                                       BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(propName,
                              func,
                              returnType,
                                parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8]
                             )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this BadObject elem,
                                                                            string propName,
                                                                            Func<BadExecutionContext, T1, T2, T3, T4, T5
                                                                                , T6, T7, T8, T9, T10, BadObject> func,
                                                                            BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(propName,
                              func,
                              returnType,
                                parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9]
                             )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this BadObject elem,
                                                                                 string propName,
                                                                                 Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func,
                                                                                 BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(propName,
                              func,
                              returnType,
                                parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9], parameters[10]
                             )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this BadObject elem,
                                                                                      string propName,
                                                                                      Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func,
                                                                                      BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(propName,
                              func,
                              returnType,
                                parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9], parameters[10], parameters[11]
                             )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this BadObject elem,
                                                                                           string propName,
                                                                                           Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func,
                                                                                           BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(propName,
                              func,
                              returnType,
                                parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9], parameters[10], parameters[11], parameters[12]
                             )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
    public static void SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this BadObject elem,
                                                                                                string propName,
                                                                                                Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func,
                                                                                                BadClassPrototype returnType)
    {
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
                             T14>(propName,
                                  func,
                                  returnType,
                                    parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9], parameters[10], parameters[11], parameters[12], parameters[13]
                                 )
                        );
    }

    /// <summary>
    ///     Sets a Function on the given Object
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
        var parameters = GetParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15));
        elem.SetProperty(propName,
                         new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14,
                             T15>(propName,
                                  func,
                                  returnType,
                                    parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9], parameters[10], parameters[11], parameters[12], parameters[13], parameters[14]
                                 )
                        );
    }
}