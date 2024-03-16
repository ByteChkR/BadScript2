using BadScript2.Common;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

///<summary>
///	Contains Common Interop Extensions for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements Array Extensions
/// </summary>
public class BadArrayExtension : BadInteropExtension
{
    /// <inheritdoc />
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadArray>(
            "Add",
            a => new BadDynamicInteropFunction<BadObject>(
                "Add",
                (_, o) => Add(a, o),
                BadAnyPrototype.Instance,
                "elem"
            )
        );

        provider.RegisterObject<BadArray>(
            "Clear",
            a => new BadDynamicInteropFunction(
                "Clear",
                _ => Clear(a),
                BadAnyPrototype.Instance
            )
        );

        provider.RegisterObject<BadArray>(
            "AddRange",
            a => new BadDynamicInteropFunction<BadObject>(
                "AddRange",
                (ctx, elems) =>
                {
                    if (elems is BadArray arr)
                    {
                        a.InnerArray.AddRange(arr.InnerArray);
                    }
                    else if (BadNativeClassBuilder.Enumerable.IsSuperClassOf(elems.GetPrototype()))
                    {
                        // Get the Enumerator
                        BadObject[] enumerated = BadNativeClassHelper.ExecuteEnumerate(ctx, elems).ToArray();
                        a.InnerArray.AddRange(enumerated);
                    }
                    else
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "Argument is not of type IEnumerable");
                    }

                    return BadObject.Null;
                },
                BadAnyPrototype.Instance,
                "elems"
            )
        );

        provider.RegisterObject<BadArray>(
            "Insert",
            a => new BadDynamicInteropFunction<decimal, BadObject>(
                "Insert",
                (_, i, o) => Insert(a, i, o),
                BadAnyPrototype.Instance,
                "elem"
            )
        );

        provider.RegisterObject<BadArray>(
            "InsertRange",
            a => new BadDynamicInteropFunction<decimal, BadObject>(
                "InsertRange",
                (ctx, num, elems) =>
                {
                    if (elems is BadArray arr)
                    {
                        a.InnerArray.InsertRange((int)num, arr.InnerArray);
                    }
                    else if (BadNativeClassBuilder.Enumerable.IsSuperClassOf(elems.GetPrototype()))
                    {
                        // Get the Enumerator
                        BadObject[] enumerated = BadNativeClassHelper.ExecuteEnumerate(ctx, elems).ToArray();
                        a.InnerArray.InsertRange((int)num, enumerated);
                    }
                    else
                    {
                        throw BadRuntimeException.Create(ctx.Scope, "Argument is not of type IEnumerable");
                    }

                    return BadObject.Null;
                },
                BadAnyPrototype.Instance,
                "index",
                "elems"
            )
        );

        provider.RegisterObject<BadArray>(
            "Remove",
            a => new BadDynamicInteropFunction<BadObject>(
                "Remove",
                (_, o) => Remove(a, o),
                BadNativeClassBuilder.GetNative("bool"),
                "elem"
            )
        );

        provider.RegisterObject<BadArray>(
            "Contains",
            a => new BadDynamicInteropFunction<BadObject>(
                "Contains",
                (_, o) => Contains(a, o),
                BadNativeClassBuilder.GetNative("bool"),
                "elem"
            )
        );

        provider.RegisterObject<BadArray>(
            "RemoveAt",
            a => new BadDynamicInteropFunction<decimal>(
                "RemoveAt",
                (_, o) => RemoveAt(a, o),
                BadAnyPrototype.Instance,
                "index"
            )
        );

        provider.RegisterObject<BadArray>(
            "Get",
            a => new BadDynamicInteropFunction<decimal>(
                "Get",
                (_, o) => Get(a, o),
                BadAnyPrototype.Instance,
                "index"
            )
        );

        provider.RegisterObject<BadArray>(
            "Set",
            a => new BadDynamicInteropFunction<decimal, BadObject>(
                "Set",
                (_, i, v) => Set(a, i, v),
                BadAnyPrototype.Instance,
                "index"
            )
        );

        provider.RegisterObject<BadArray>(
            "GetEnumerator",
            a => new BadDynamicInteropFunction(
                "GetEnumerator",
                _ => GetEnumerator(a),
                BadNativeClassBuilder.Enumerator
            )
        );

        provider.RegisterObject<BadArray>(
            BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
            a => new BadDynamicInteropFunction<BadObject>(
                BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
                (c, i) => ArrayAccess(c, a, i),
                BadAnyPrototype.Instance,
                "index"
            )
        );
        provider.RegisterObject<BadArray>(
            BadStaticKeys.ARRAY_ACCESS_REVERSE_OPERATOR_NAME,
            a => new BadDynamicInteropFunction<decimal>(
                BadStaticKeys.ARRAY_ACCESS_REVERSE_OPERATOR_NAME,
                (_, i) => BadObjectReference.Make(
                    $"{a}[^{i}]",
                    () => Get(a, a.InnerArray.Count - i),
                    (v, _) => Set(a, a.InnerArray.Count - i, v)
                ),
                BadAnyPrototype.Instance,
                "index"
            )
        );

        provider.RegisterObject<BadArray>("Length", a => BadObject.Wrap((decimal)a.InnerArray.Count));
    }

    private static BadObject ArrayAccess(BadExecutionContext context, BadArray array, BadObject enumerator)
    {
        BadSourcePosition position = BadSourcePosition.FromSource("ArrayAccess", 0, 1);
        if (enumerator is IBadNumber num)
        {
            return BadObjectReference.Make($"{array}[{num.Value}]", () => Get(array, num.Value), (v, _) => Set(array, num.Value, v));
        }

        BadArray result = new BadArray();

        if (enumerator.HasProperty("GetEnumerator", context.Scope) && enumerator.GetProperty("GetEnumerator", context.Scope).Dereference() is BadFunction getEnumerator)
        {
            foreach (BadObject e in getEnumerator.Invoke(Array.Empty<BadObject>(), context))
            {
                enumerator = e;
            }

            enumerator = enumerator.Dereference();
        }
        (BadFunction moveNext, BadFunction getCurrent) = BadForEachExpression.FindEnumerator(enumerator, context, position);
        BadObject r = BadObject.Null;
        
        if (context.Scope.IsError)
        {
            return BadObject.Null;
        }

        BadObject cond = BadObject.Null;

        foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), context))
        {
            cond = o;
        }

        if (context.Scope.IsError)
        {
            return BadObject.Null;
        }

        IBadBoolean bRet = cond.Dereference() as IBadBoolean ??
                           throw new BadRuntimeException("While Condition is not a boolean", position);

        while (bRet.Value)
        {
            using BadExecutionContext loopContext = new BadExecutionContext(
                context.Scope.CreateChild(
                    "ForEachLoop",
                    context.Scope,
                    null,
                    BadScopeFlags.Breakable | BadScopeFlags.Continuable
                )
            );

            BadObject current = BadObject.Null;

            foreach (BadObject o in getCurrent.Invoke(Array.Empty<BadObject>(), loopContext))
            {
                current = o;
            }

            if (loopContext.Scope.IsError)
            {
                return BadObject.Null;
            }


            if (current is not IBadNumber n)
            {
                throw BadRuntimeException.Create(context.Scope, "Enumerator Current did not return a number", position);
            }
            
            result.InnerArray.Add(Get(array, n.Value));

            foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), loopContext))
            {
                cond = o;
            }


            if (loopContext.Scope.IsError)
            {
                return BadObject.Null;
            }

            bRet = cond.Dereference() as IBadBoolean ??
                   throw BadRuntimeException.Create(
                       context.Scope,
                       "Enumerator MoveNext did not return a boolean",
                       position
                   );
        }
        
        return result;
    }

    /// <summary>
    ///     Returns an enumerator of the array
    /// </summary>
    /// <param name="array">The Array</param>
    /// <returns>Enumerator</returns>
    private static BadObject GetEnumerator(BadArray array)
    {
        return new BadInteropEnumerator(array.InnerArray.GetEnumerator());
    }

    /// <summary>
    ///     Clears the array
    /// </summary>
    /// <param name="arg">The Array</param>
    /// <returns>NULL</returns>
    private static BadObject Clear(BadArray arg)
    {
        arg.InnerArray.Clear();

        return BadObject.Null;
    }

    /// <summary>
    ///     Adds an element to the array
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Element</param>
    /// <returns>NULL</returns>
    private static BadObject Add(BadArray arg, BadObject obj)
    {
        arg.InnerArray.Add(obj);

        return BadObject.Null;
    }

    /// <summary>
    ///     Inserts an element at the given index
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="index">Index</param>
    /// <param name="obj">Element</param>
    /// <returns>NULL</returns>
    private static BadObject Insert(BadArray arg, decimal index, BadObject obj)
    {
        arg.InnerArray.Insert((int)index, obj);

        return BadObject.Null;
    }

    /// <summary>
    ///     Returns True if the array contains the given element
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Element</param>
    /// <returns>Boolean</returns>
    private static BadObject Contains(BadArray arg, BadObject obj)
    {
        return arg.InnerArray.Contains(obj);
    }

    /// <summary>
    ///     Removes an element from the array
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Element</param>
    /// <returns>NULL</returns>
    private static BadObject Remove(BadArray arg, BadObject obj)
    {
        return arg.InnerArray.Remove(obj);
    }

    /// <summary>
    ///     Removes an element at the given index
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Index</param>
    /// <returns>NULL</returns>
    private static BadObject RemoveAt(BadArray arg, decimal obj)
    {
        int index = (int)obj;
        arg.InnerArray.RemoveAt(index);

        return BadObject.Null;
    }

    /// <summary>
    ///     Returns a value from the array
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Index</param>
    /// <returns>Element at Index</returns>
    private static BadObject Get(BadArray arg, decimal obj)
    {
        int index = (int)obj;

        return arg.InnerArray[index];
    }

    /// <summary>
    ///     Sets a value in the array
    /// </summary>
    /// <param name="arg">The Array</param>
    /// <param name="obj">The Index</param>
    /// <param name="value">The value</param>
    /// <returns>NULL</returns>
    private static BadObject Set(BadArray arg, decimal obj, BadObject value)
    {
        int index = (int)obj;
        arg.InnerArray[index] = value;

        return BadObject.Null;
    }
}