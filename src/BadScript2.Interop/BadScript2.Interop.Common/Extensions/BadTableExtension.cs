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

public class BadTableExtension : BadInteropExtension
{
    protected override void AddExtensions()
    {
        RegisterObject<BadTable>(
            "RemoveKey",
            o => new BadDynamicInteropFunction<BadObject>(
                "RemoveKey",
                (_, k) => RemoveKey(o, k),
                "key"
            )
        );

        RegisterObject<BadTable>(
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
                }
            )
        );

        RegisterObject<BadTable>(
            BadStaticKeys.ArrayAccessOperatorName,
            t => new BadDynamicInteropFunction<BadObject>(
                BadStaticKeys.ArrayAccessOperatorName,
                (c, o) => t.GetProperty(o, c.Scope),
                "key"
            )
        );

        RegisterObject<BadTable>("Keys", Keys);
        RegisterObject<BadTable>("Values", Values);
        RegisterObject<BadTable>("Length", a => BadObject.Wrap((decimal)a.InnerTable.Count));

        RegisterObject<BadTable>(
            "Join",
            t => new BadInteropFunction(
                "Join",
                (c, a) => JoinTable(c, t, a),
                new BadFunctionParameter("overwrite", false, true, false, null, BadNativeClassBuilder.GetNative("bool")),
                new BadFunctionParameter("others", false, true, true, null)
            )
        );
    }

    private BadObject JoinTable(BadExecutionContext ctx, BadTable self, BadObject[] args)
    {
        if (args.Length == 0)
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

    private static BadObject RemoveKey(BadTable table, BadObject key)
    {
        table.RemoveKey(key);

        return BadObject.Null;
    }

    private static BadObject Keys(BadTable table)
    {
        return new BadArray(table.InnerTable.Keys.ToList());
    }

    private static BadObject Values(BadTable table)
    {
        return new BadArray(table.InnerTable.Values.ToList());
    }
}