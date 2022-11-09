using BadScript2.Common;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;

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