using BadScript2.Runtime.Interop;

namespace BadScript2.Interop.Linq;

public static class BadLinqApiExtensions
{
    public static BadRuntime UseLinqApi(this BadRuntime runtime)
    {
        BadInteropExtension.AddExtension<BadLinqExtensions>();
        return runtime;
    }
}