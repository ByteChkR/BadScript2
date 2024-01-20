using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements Scope Extensions
/// </summary>
public class BadScopeExtension : BadInteropExtension
{
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadScope>(
            "GetLocals",
            o => new BadDynamicInteropFunction(
                "GetLocals",
                _ => GetLocals(o),
                BadNativeClassBuilder.GetNative("Table")
            )
        );
        provider.RegisterObject<BadScope>(
            "GetParent",
            o => new BadDynamicInteropFunction(
                "GetParent",
                _ => GetParent(o),
                BadScope.Prototype
            )
        );
    }

    /// <summary>
    ///     Gets the Parent Scope
    /// </summary>
    /// <param name="scope">The Scope</param>
    /// <returns>Parent Scope or NULL</returns>
    private BadObject GetParent(BadScope scope)
    {
        return scope.Parent ?? BadObject.Null;
    }

    /// <summary>
    ///     Returns the Local Variable Table of the Scope
    /// </summary>
    /// <param name="scope">The Scope</param>
    /// <returns>Local Variable Table</returns>
    private BadObject GetLocals(BadScope scope)
    {
        return scope.GetTable();
    }
}