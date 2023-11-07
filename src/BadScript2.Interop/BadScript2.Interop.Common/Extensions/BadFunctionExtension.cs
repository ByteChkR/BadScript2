using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements Function Extensions
/// </summary>
public class BadFunctionExtension : BadInteropExtension
{
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadFunction>("Name", f => f.Name?.Text ?? "<anonymous>");
        provider.RegisterObject<BadFunction>(
            "Parameters",
            f => new BadArray(f.Parameters.Select(x => BadObject.Wrap(x)).ToList())
        );
        provider.RegisterObject<BadFunction>(
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

        provider.RegisterObject<BadFunction>("Meta", f => f.MetaData);

        provider.RegisterObject<BadFunctionParameter>("Name", p => p.Name);
        provider.RegisterObject<BadFunctionParameter>("IsNullChecked", p => p.IsNullChecked);
        provider.RegisterObject<BadFunctionParameter>("IsOptional", p => p.IsOptional);
        provider.RegisterObject<BadFunctionParameter>("IsRestArgs", p => p.IsRestArgs);
        provider.RegisterObject<BadFunctionParameter>("Type", p => p.Type ?? BadObject.Null);
    }
}