using BadScript2.Common;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Extensions;

public class BadArrayExtension : BadInteropExtension
{
    protected override void AddExtensions()
    {
        RegisterObject<BadArray>(
            "Add",
            a => new BadDynamicInteropFunction<BadObject>(
                "Add",
                (_, o) => Add(a, o),
                "elem"
            )
        );

        RegisterObject<BadArray>(
            "AddRange",
            a => new BadDynamicInteropFunction<BadArray>(
                "AddRange",
                (_, elems) =>
                {
                    a.InnerArray.AddRange(elems.InnerArray);

                    return BadObject.Null;
                },
                "elems"
            )
        );

        RegisterObject<BadArray>(
            "Insert",
            a => new BadDynamicInteropFunction<decimal, BadObject>(
                "Insert",
                (_, i, o) => Insert(a, i, o),
                "elem"
            )
        );

        RegisterObject<BadArray>(
            "InsertRange",
            a => new BadDynamicInteropFunction<decimal, BadArray>(
                "InsertRange",
                (_, num, elems) =>
                {
                    a.InnerArray.InsertRange((int)num, elems.InnerArray);

                    return BadObject.Null;
                },
                "index",
                "elems"
            )
        );

        RegisterObject<BadArray>(
            "Remove",
            a => new BadDynamicInteropFunction<BadObject>(
                "Remove",
                (_, o) => Remove(a, o),
                "elem"
            )
        );

        RegisterObject<BadArray>(
            "Contains",
            a => new BadDynamicInteropFunction<BadObject>(
                "Contains",
                (_, o) => Contains(a, o),
                "elem"
            )
        );

        RegisterObject<BadArray>(
            "RemoveAt",
            a => new BadDynamicInteropFunction<decimal>(
                "RemoveAt",
                (_, o) => RemoveAt(a, o),
                "index"
            )
        );

        RegisterObject<BadArray>(
            "Get",
            a => new BadDynamicInteropFunction<decimal>(
                "Get",
                (_, o) => Get(a, o),
                "index"
            )
        );

        RegisterObject<BadArray>(
            "Set",
            a => new BadDynamicInteropFunction<decimal, BadObject>(
                "Set",
                (_, i, v) => Set(a, i, v),
                "index"
            )
        );

        RegisterObject<BadArray>(
            "GetEnumerator",
            a => new BadDynamicInteropFunction(
                "GetEnumerator",
                _ => GetEnumerator(a)
            )
        );

        RegisterObject<BadArray>(
            BadStaticKeys.ArrayAccessOperatorName,
            a => new BadDynamicInteropFunction<decimal>(
                BadStaticKeys.ArrayAccessOperatorName,
                (_, i) => BadObjectReference.Make($"{a}[{i}]", () => Get(a, i), (v, _) => Set(a, i, v)),
                "index"
            )
        );
        RegisterObject<BadArray>(
            BadStaticKeys.ArrayAccessReverseOperatorName,
            a => new BadDynamicInteropFunction<decimal>(
                BadStaticKeys.ArrayAccessReverseOperatorName,
                (_, i) => BadObjectReference.Make($"{a}[^{i}]", () => Get(a, a.InnerArray.Count - i), (v, _) => Set(a, a.InnerArray.Count - i, v)),
                "index"
            )
        );

        RegisterObject<BadArray>("Length", a => BadObject.Wrap((decimal)a.InnerArray.Count));
    }

    private BadObject GetEnumerator(BadArray array)
    {
        return new BadInteropEnumerator(array.InnerArray.GetEnumerator());
    }

    private BadObject Add(BadArray arg, BadObject obj)
    {
        arg.InnerArray.Add(obj);

        return BadObject.Null;
    }

    private BadObject Insert(BadArray arg, decimal index, BadObject obj)
    {
        arg.InnerArray.Insert((int)index, obj);

        return BadObject.Null;
    }

    private BadObject Contains(BadArray arg, BadObject obj)
    {
        return arg.InnerArray.Contains(obj);
    }

    private BadObject Remove(BadArray arg, BadObject obj)
    {
        arg.InnerArray.Remove(obj);

        return BadObject.Null;
    }

    private BadObject RemoveAt(BadArray arg, decimal obj)
    {
        int index = (int)obj;
        arg.InnerArray.RemoveAt(index);

        return BadObject.Null;
    }

    private BadObject Get(BadArray arg, decimal obj)
    {
        int index = (int)obj;

        return arg.InnerArray[index];
    }

    private BadObject Set(BadArray arg, decimal obj, BadObject value)
    {
        int index = (int)obj;
        arg.InnerArray[index] = value;

        return BadObject.Null;
    }
}