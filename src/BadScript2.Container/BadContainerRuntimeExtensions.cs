using BadScript2.Interop.IO;

namespace BadScript2.Container
{
    /// <summary>
    /// Extensions for the BadScript2 Runtime to run in a container
    /// </summary>
    public static class BadContainerRuntimeExtensions
    {
        /// <summary>
        /// Runs a BadScript2 Container
        /// </summary>
        /// <param name="runtime">The Runtime</param>
        /// <param name="stack">The File System Stack</param>
        /// <returns>The Runtime Execution Result</returns>
        public static BadRuntimeExecutionResult RunContainer(this BadRuntime runtime, BadFileSystemStack stack)
        {
            var fs = stack.Create();
            var rt = runtime
                .ConfigureModuleImporter(i => i.Clear())
                .UseLocalModules(fs)
                .UseFileSystemApi(fs);

            return rt.ExecuteFile("main.bs", fs);
        }
    }
}