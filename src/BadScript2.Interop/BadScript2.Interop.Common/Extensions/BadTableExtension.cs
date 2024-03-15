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

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements Table Extensions
/// </summary>
public class BadTableExtension : BadInteropExtension
{
    /// <inheritdoc />
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadTable>(
            "RemoveKey",
            o => new BadDynamicInteropFunction<BadObject>(
                "RemoveKey",
                (c, k) =>
                {
                    if (k is not IBadString s)
                    {
                        throw BadRuntimeException.Create(c.Scope, "Key is not a string");
                    }

                    return RemoveKey(o, s.Value);
                },
                BadNativeClassBuilder.GetNative("bool"),
                "key"
            )
        );

        provider.RegisterObject<BadTable>(
            "MakeReadOnly",
            table => new BadDynamicInteropFunction(
                "MakeReadOnly",
                _ =>
                {
                    foreach (string key in table.InnerTable.Keys)
                    {
                        table.PropertyInfos[key].IsReadOnly = true;
                    }

                    return BadObject.Null;
                },
                BadAnyPrototype.Instance
            )
        );

        provider.RegisterObject<BadTable>(
            BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
            t => new BadDynamicInteropFunction<BadObject>(
                BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
                (c, o) => ArrayAccess(c, t, o),
                BadAnyPrototype.Instance,
                "key"
            )
        );

        provider.RegisterObject<BadTable>("Keys", Keys);
        provider.RegisterObject<BadTable>("Values", Values);
        provider.RegisterObject<BadTable>("Length", a => BadObject.Wrap((decimal)a.InnerTable.Count));

        provider.RegisterObject<BadTable>(
            "Join",
            t => new BadInteropFunction(
                "Join",
                (c, a) => JoinTable(c, t, a),
                false,
                BadNativeClassBuilder.GetNative("Table"),
                new BadFunctionParameter(
                    "overwrite",
                    false,
                    true,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("bool")
                ),
                new BadFunctionParameter("others", false, true, true, null)
            )
        );
    }
    
    private static BadObject ArrayAccess(BadExecutionContext context, BadTable table, BadObject enumerator)
    {
        BadSourcePosition position = BadSourcePosition.FromSource("ArrayAccess", 0, 1);
        if (enumerator is IBadString str)
        {
            return table.GetProperty(str.Value, context.Scope);
        }

        BadTable result = new BadTable();

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


            if (current is not IBadString key)
            {
                throw BadRuntimeException.Create(context.Scope, "Enumerator Current did not return a string", position);
            }
            
            result.SetProperty(key.Value, table.GetProperty(key.Value, context.Scope).Dereference());

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
    ///     Joins two or more tables together
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="self">The 'self' table</param>
    /// <param name="args">The Arguments</param>
    /// <returns>The 'self' table</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the arguments are invalid</exception>
    private static BadObject JoinTable(BadExecutionContext ctx, BadTable self, IReadOnlyList<BadObject> args)
    {
        if (args.Count == 0)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count");
        }

        BadObject overwrite = args[0];
        BadObject[] others = args.Skip(1).ToArray();

        if (overwrite is not IBadBoolean ov)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Overwrite is not a boolean value");
        }

        foreach (BadObject o in others)
        {
            if (o is not BadTable other)
            {
                throw BadRuntimeException.Create(ctx.Scope, "Others can only be tables");
            }

            foreach (KeyValuePair<string, BadObject> kvp in other.InnerTable)
            {
                if (ov.Value || !self.InnerTable.ContainsKey(kvp.Key))
                {
                    self.GetProperty(kvp.Key, ctx.Scope).Set(kvp.Value);
                }
            }
        }

        return self;
    }

    /// <summary>
    ///     Removes a key from the table
    /// </summary>
    /// <param name="table">Table</param>
    /// <param name="key">Key</param>
    /// <returns>NULL</returns>
    private static BadObject RemoveKey(BadTable table, string key)
    {
        return table.RemoveKey(key);
    }

    /// <summary>
    ///     Returns the Keys of the Table
    /// </summary>
    /// <param name="table">Table</param>
    /// <returns>Array of keys</returns>
    private static BadObject Keys(BadTable table)
    {
        return new BadArray(table.InnerTable.Keys.Select(x => (BadObject)x).ToList());
    }

    /// <summary>
    ///     Returns the Values of the Table
    /// </summary>
    /// <param name="table">Table</param>
    /// <returns>Array of Values</returns>
    private static BadObject Values(BadTable table)
    {
        return new BadArray(table.InnerTable.Values.ToList());
    }
}