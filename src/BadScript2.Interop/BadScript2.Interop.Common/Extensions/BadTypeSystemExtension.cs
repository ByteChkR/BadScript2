using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Extensions;

public class BadTypeSystemExtension : BadInteropExtension
{
    protected override void AddExtensions()
    {
        RegisterGlobal(
            "IsInstanceOf",
            obj => new BadDynamicInteropFunction<BadClassPrototype>(
                "IsInstanceOf",
                (_, proto) => IsInstanceOf(proto, obj),
                new BadFunctionParameter("prototype", false, true, false)
            )
        );

        RegisterObject<BadClassPrototype>(
            "IsAssignableFrom",
            proto => new BadDynamicInteropFunction<BadObject>(
                "IsAssignableFrom",
                (_, o) => IsAssignableFrom(o, proto)
            )
        );
        RegisterObject<BadClassPrototype>(
            "IsBaseClassOf",
            proto => new BadDynamicInteropFunction<BadClassPrototype>(
                "IsBaseClassOf",
                (_, super) => IsBaseClassOf(proto, super)
            )
        );

        RegisterObject<BadClassPrototype>(
            "IsSuperClassOf",
            proto => new BadDynamicInteropFunction<BadClassPrototype>(
                "IsSuperClassOf",
                (_, super) => IsBaseClassOf(super, proto)
            )
        );
    }

    private static BadObject IsAssignableFrom(BadObject obj, BadClassPrototype proto)
    {
        return proto.IsAssignableFrom(obj);
    }

    private static BadObject IsBaseClassOf(
        BadClassPrototype proto,
        BadClassPrototype super)
    {
        return super.IsSuperClassOf(proto);
    }

    private static BadObject IsInstanceOf(BadClassPrototype proto, BadObject obj)
    {
        return proto.IsAssignableFrom(obj);
    }
}