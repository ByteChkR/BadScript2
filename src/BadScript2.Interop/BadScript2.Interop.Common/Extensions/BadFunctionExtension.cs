using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Extensions;

public class BadFunctionExtension : BadInteropExtension
{
    protected override void AddExtensions()
    {
        RegisterObject<BadFunction>("Name", f => f.Name?.Text ?? "<anonymous>");
        RegisterObject<BadFunction>(
            "Parameters",
            f => new BadArray(f.Parameters.Select(x => BadObject.Wrap(x)).ToList())
        );
        RegisterObject<BadFunction>(
            "Invoke",
            f => new BadDynamicInteropFunction<BadArray>(
                "Invoke",
                (ctx, a) =>
                {
                    BadObject r = BadObject.Null;
                    foreach (BadObject o in f.Invoke(a.InnerArray.ToArray(), ctx))
                    {
                        r = o;
                    }

                    return r;
                },
                "args"
            )
        );

        RegisterObject<BadFunctionParameter>("Name", p => p.Name);
        RegisterObject<BadFunctionParameter>("IsNullChecked", p => p.IsNullChecked);
        RegisterObject<BadFunctionParameter>("IsOptional", p => p.IsOptional);
        RegisterObject<BadFunctionParameter>("IsRestArgs", p => p.IsRestArgs);
    }
}