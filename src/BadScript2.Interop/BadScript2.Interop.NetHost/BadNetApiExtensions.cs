namespace BadScript2.Interop.NetHost;

public static class BadNetApiExtensions
{
	public static BadRuntime UseNetHostApi(this BadRuntime runtime)
	{
		runtime.ConfigureContextOptions(opts => opts.AddExtension<BadNetHostExtensions>());
		runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadNetHostApi()));

		return runtime;
	}
}
