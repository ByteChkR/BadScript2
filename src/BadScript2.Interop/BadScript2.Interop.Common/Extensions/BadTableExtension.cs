using BadScript2.Common;
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
                (_, k) => RemoveKey(o, k),
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
                    foreach (BadObject key in table.InnerTable.Keys)
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
                (c, o) => t.GetProperty(o, c.Scope),
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

            foreach (KeyValuePair<BadObject, BadObject> kvp in other.InnerTable)
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
    private static BadObject RemoveKey(BadTable table, BadObject key)
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
        return new BadArray(table.InnerTable.Keys.ToList());
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