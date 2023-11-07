namespace BadScript2.Interop.NUnit;

public static class BadNUnitApiExtensions
{
    public static BadRuntime UseNUnitApi(this BadRuntime runtime)
    {
        runtime.ConfigureContextOptions(opts =>opts.AddOrReplaceApi(new BadNUnitApi()));

        return runtime;
    }

    public static BadRuntime UseNUnitConsole(this BadRuntime runtime, BadUnitTestContextBuilder console)
    {
        runtime.ConfigureContextOptions(opts =>opts.AddOrReplaceApi(new BadNUnitConsoleApi(console)));

        return runtime;
    }

}