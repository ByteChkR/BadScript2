namespace BadScript2.Interop.Html;

public static class BadHtmlApiExtensions
{
    public static BadRuntime UseHtmlApi(this BadRuntime runtime)
    {
        runtime.ConfigureContextOptions(opts =>opts.AddOrReplaceApi(new BadHtmlApi()));

        return runtime;
    }
}