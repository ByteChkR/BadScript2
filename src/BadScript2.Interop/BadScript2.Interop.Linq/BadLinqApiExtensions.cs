namespace BadScript2.Interop.Linq;

public static class BadLinqApiExtensions
{
    public static BadRuntime UseLinqApi(this BadRuntime runtime)
    {
        runtime.ConfigureContextOptions(opts => opts.AddExtension<BadLinqExtensions>());

        return runtime;
    }
}