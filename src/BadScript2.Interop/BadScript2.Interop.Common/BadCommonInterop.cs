using BadScript2.Interop.Common.Apis;
using BadScript2.Interop.Common.Extensions;
using BadScript2.Runtime.Interop;

namespace BadScript2.Interop.Common
{
    public class BadCommonInterop
    {
        private static readonly BadInteropApi[] s_CommonApis =
        {
            new BadConsoleApi(),
            new BadRuntimeApi(),
            new BadMathApi(),
        };

        public static IEnumerable<BadInteropApi> Apis => s_CommonApis;

        public static void AddExtensions()
        {
            BadInteropExtension.AddExtension<BadObjectExtension>();
            BadInteropExtension.AddExtension<BadStringExtension>();
            BadInteropExtension.AddExtension<BadTableExtension>();
            BadInteropExtension.AddExtension<BadScopeExtension>();
            BadInteropExtension.AddExtension<BadArrayExtension>();
            BadInteropExtension.AddExtension<BadFunctionExtension>();
            BadInteropExtension.AddExtension<BadTypeSystemExtension>();
        }
    }
}