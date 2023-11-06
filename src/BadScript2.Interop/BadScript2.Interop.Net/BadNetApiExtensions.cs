using BadScript2.Runtime.Interop;

namespace BadScript2.Interop.Net;

public static class BadNetApiExtensions
{
    public static BadRuntime UseNetApi(this BadRuntime runtime)
    {
        BadInteropExtension.AddExtension<BadNetInteropExtensions>();
        runtime.Options.AddOrReplaceApi(new BadNetApi());

        return runtime;
    }

}