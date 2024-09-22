using BadScript2.Interop.IO;

namespace BadScript2.Container
{
    public static class BadContainerRuntimeExtensions
    {
        public static BadRuntimeExecutionResult RunContainer(this BadRuntime runtime, BadFileSystemStack stack)
        {
            var fs = stack.Create();
            var rt = runtime
                .Clone()
                .ConfigureModuleImporter(i => i.Clear())
                .UseLocalModules(fs)
                .UseFileSystemApi(fs);

            return rt.ExecuteFile("main.bs", fs);
        }
    }
}