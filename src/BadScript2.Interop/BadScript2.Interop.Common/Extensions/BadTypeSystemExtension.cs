using BadScript2.Runtime;
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
                (ctx, proto) => IsInstanceOf(ctx, proto, obj),
                new BadFunctionParameter("prototype", false, true, false)
            )
        );

        RegisterObject<BadClassPrototype>(
            "IsAssignableFrom",
            proto => new BadDynamicInteropFunction<BadObject>(
                "IsAssignableFrom",
                (ctx, o) => IsAssignableFrom(ctx, o, proto)
            )
        );
    }

    private static BadObject IsAssignableFrom(BadExecutionContext ctx, BadObject obj, BadClassPrototype proto)
    {
        return proto.IsAssignableFrom(obj);
    }

    private static BadObject IsInstanceOf(BadExecutionContext ctx, BadClassPrototype proto, BadObject obj)
    {
        return proto.IsAssignableFrom(obj);
    }
}