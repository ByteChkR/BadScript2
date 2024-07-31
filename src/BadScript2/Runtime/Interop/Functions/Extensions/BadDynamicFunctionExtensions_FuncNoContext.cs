using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Functions.Extensions;

public static partial class BadDynamicFunctionExtensions
{
    /// <summary>
    ///     Sets a Function on the given Object
    /// </summary>
    /// <param name="elem">The Object to set the Function on</param>
    /// <param name="propName">The Property Name</param>
    /// <param name="func">The Function to set</param>
    /// <param name="returnType">The Return Type of the Function</param>
    public static void SetFunction(this BadObject elem,
                                   string propName,
                                   Func<BadObject> func,
                                   BadClassPrototype returnType)
    {
        elem.SetFunction(propName, _ => func(), returnType);
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
                                      Func<T, BadObject> func,
                                      BadClassPrototype returnType)
    {
        elem.SetFunction<T>(propName, (_, t) => func(t), returnType);
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
                                           Func<T1, T2, BadObject> func,
                                           BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2>(propName, (_, t1, t2) => func(t1, t2), returnType);
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
                                               Func<T1, T2, T3, BadObject> func,
                                               BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3>(propName, (_, t1, t2, t3) => func(t1, t2, t3), returnType);
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
                                                   Func<T1, T2, T3, T4, BadObject> func,
                                                   BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4>(propName, (_, t1, t2, t3, t4) => func(t1, t2, t3, t4), returnType);
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
                                                       Func<T1, T2, T3, T4, T5, BadObject> func,
                                                       BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5>(propName,
                                             (_,
                                              t1,
                                              t2,
                                              t3,
                                              t4,
                                              t5) => func(t1, t2, t3, t4, t5),
                                             returnType
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
                                                           Func<T1, T2, T3, T4, T5, T6, BadObject> func,
                                                           BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6>(propName,
                                                 (_,
                                                  t1,
                                                  t2,
                                                  t3,
                                                  t4,
                                                  t5,
                                                  t6) => func(t1, t2, t3, t4, t5, t6),
                                                 returnType
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
                                                               Func<T1, T2, T3, T4, T5, T6, T7, BadObject> func,
                                                               BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7>(propName,
                                                     (_,
                                                      t1,
                                                      t2,
                                                      t3,
                                                      t4,
                                                      t5,
                                                      t6,
                                                      t7) => func(t1, t2, t3, t4, t5, t6, t7),
                                                     returnType
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
                                                                   Func<T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func,
                                                                   BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8>(propName,
                                                         (_,
                                                          t1,
                                                          t2,
                                                          t3,
                                                          t4,
                                                          t5,
                                                          t6,
                                                          t7,
                                                          t8) => func(t1, t2, t3, t4, t5, t6, t7, t8),
                                                         returnType
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
                                                                       Func<T1, T2, T3, T4, T5, T6, T7, T8, T9,
                                                                           BadObject> func,
                                                                       BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(propName,
                                                             (_,
                                                              t1,
                                                              t2,
                                                              t3,
                                                              t4,
                                                              t5,
                                                              t6,
                                                              t7,
                                                              t8,
                                                              t9) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9),
                                                             returnType
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
                                                                            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10
                                                                                , BadObject> func,
                                                                            BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(propName,
                                                                  (_,
                                                                   t1,
                                                                   t2,
                                                                   t3,
                                                                   t4,
                                                                   t5,
                                                                   t6,
                                                                   t7,
                                                                   t8,
                                                                   t9,
                                                                   t10) => func(t1,
                                                                                t2,
                                                                                t3,
                                                                                t4,
                                                                                t5,
                                                                                t6,
                                                                                t7,
                                                                                t8,
                                                                                t9,
                                                                                t10
                                                                               ),
                                                                  returnType
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
                                                                                 Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func,
                                                                                 BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(propName,
                                                                       (_,
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
                                                                        t11) => func(t1,
                                                                            t2,
                                                                            t3,
                                                                            t4,
                                                                            t5,
                                                                            t6,
                                                                            t7,
                                                                            t8,
                                                                            t9,
                                                                            t10,
                                                                            t11
                                                                           ),
                                                                       returnType
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
                                                                                      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func,
                                                                                      BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(propName,
                                                                            (_,
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
                                                                                 t12
                                                                                ),
                                                                            returnType
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
                                                                                           Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func,
                                                                                           BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(propName,
             (_,
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
                           t13
                          ),
             returnType
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
                                                                                                Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func,
                                                                                                BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(propName,
             (_,
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
                           t14
                          ),
             returnType
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
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func,
        BadClassPrototype returnType)
    {
        elem.SetFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(propName,
             (_,
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
                           t15
                          ),
             returnType
            );
    }
}