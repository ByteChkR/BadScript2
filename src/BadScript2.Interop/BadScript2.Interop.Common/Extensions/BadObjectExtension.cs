using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements Generic Object Extensions
/// </summary>
public class BadObjectExtension : BadInteropExtension
{
    /// <inheritdoc/>
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterGlobal(
            "ToString",
            o => new BadDynamicInteropFunction(
                "ToString",
                _ => o.ToString(),
                BadNativeClassBuilder.GetNative("string")
            )
        );
        provider.RegisterGlobal(
            "GetType",
            o => new BadDynamicInteropFunction(
                "GetType",
                _ => o.GetPrototype(),
                BadClassPrototype.Prototype
            )
        );
    }
}