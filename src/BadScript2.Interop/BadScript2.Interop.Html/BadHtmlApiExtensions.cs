namespace BadScript2.Interop.Html;

public static class BadHtmlApiExtensions
{
    public static BadRuntime UseHtmlApi(this BadRuntime runtime)
    {
        runtime.Options.AddOrReplaceApi(new BadHtmlApi());

        return runtime;
    }
}