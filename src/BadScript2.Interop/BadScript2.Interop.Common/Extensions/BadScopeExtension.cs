using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Extensions
{
    public class BadScopeExtension : BadInteropExtension
    {
        protected override void AddExtensions()
        {
            RegisterObject<BadScope>(
                "GetLocals",
                o => new BadDynamicInteropFunction(
                    "GetLocals",
                    _ => GetLocals(o)
                )
            );
            RegisterObject<BadScope>(
                "GetParent",
                o => new BadDynamicInteropFunction(
                    "GetParent",
                    _ => GetParent(o)
                )
            );
        }

        private BadObject GetParent(BadScope scope)
        {
            return scope.Parent ?? BadObject.Null;
        }

        private BadObject GetLocals(BadScope scope)
        {
            return scope.GetTable();
        }
    }
}