using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Apis;
using BadScript2.Interop.Common.Extensions;
using BadScript2.Interop.Common.Regex;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.Common.Versioning;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

///<summary>
///	Contains Common Interop Extensions and APIs for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.Common;

/// <summary>
///     Implements the Common Interop Wrapper
/// </summary>
public static class BadCommonInterop
{


    /// <summary>
    ///     Configures the Runtime to use the specified startup arguments
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <param name="args">The Startup Arguments</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseStartupArguments(this BadRuntime runtime, IEnumerable<string> args)
    {
        BadRuntimeApi.StartupArguments = args;

        return runtime;
    }

    /// <summary>
    ///     The Default Executor for asynchronous execution
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="exprs">The Expressions to execute</param>
    /// <returns>The Result of the Execution</returns>
    private static BadObject ExecuteTask(BadExecutionContext ctx, IEnumerable<BadExpression> exprs)
    {
        BadTask task = new BadTask(new BadInteropRunnable(ctx.Execute(exprs.ToArray())
                                                             .GetEnumerator()
                                                         ),
                                   "Main"
                                  );

        var runner = ctx.Scope.GetSingleton<BadTaskRunner>();
        if(runner == null)throw BadRuntimeException.Create(ctx.Scope, "No Task Runner found");

        runner.AddTask(task,
                                       true
                                      );

        while (!runner.IsIdle)
        {
            runner.RunStep();
        }

        return task.Runnable.GetReturn();
    }

    /// <summary>
    ///     Configures the Runtime to use the Common Interop Extensions
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <param name="useAsync">Whether to use the Async Extensions</param>
    /// <param name="runner">The Task Runner Instance</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseCommonInterop(this BadRuntime runtime, bool useAsync = true, BadTaskRunner? runner = null)
    {
        if (useAsync)
        {
            BadNativeClassBuilder.AddNative(BadTask.Prototype);

            var taskRunner = runner ?? new BadTaskRunner();
            runtime
                .ConfigureContext(c => c.Scope.AddSingleton(taskRunner))
                .UseApi(() => new BadTaskRunnerApi(), true)
                .UseExecutor(ExecuteTask);
        }

        BadNativeClassBuilder.AddNative(BadRegex.Prototype);
        BadNativeClassBuilder.AddNative(BadMatch.Prototype);
        BadNativeClassBuilder.AddNative(BadGroup.Prototype);
        BadNativeClassBuilder.AddNative(BadCapture.Prototype);
        BadNativeClassBuilder.AddNative(BadVersion.Prototype);
        return runtime
            .UseCommonExtensions(useAsync)
            .UseApis(() => [
                new BadConsoleApi(BadConsole.GetConsole()),
                new BadRuntimeApi(),
                new BadMathApi(),
                new BadOperatingSystemApi(),
                new BadXmlApi()
            ], true);
    }

    /// <summary>
    ///     Configures the Runtime to use the Console API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <param name="console">The Console to use</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseConsoleApi(this BadRuntime runtime, IBadConsole? console = null)
    {
        if (console != null)
        {
            return runtime.UseApi(() => new BadConsoleApi(console), true);
        }

        return runtime.UseApi(() => new BadConsoleApi(BadConsole.GetConsole()), true);
    }

    /// <summary>
    /// Configures the Runtime to use the Common Interop Extensions
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <param name="useAsync">Whether to use the Async Extensions</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseCommonExtensions(this BadRuntime runtime, bool useAsync = true)
    {
        if (useAsync)
        {
            runtime.UseExtension<BadTaskExtensions>();
        }

        return runtime
               .UseExtension<BadObjectExtension>()
               .UseExtension<BadStringExtension>()
               .UseExtension<BadNumberExtension>()
               .UseExtension<BadDateExtension>()
               .UseExtension<BadTableExtension>()
               .UseExtension<BadScopeExtension>()
               .UseExtension<BadArrayExtension>()
               .UseExtension<BadFunctionExtension>()
               .UseExtension<BadTypeSystemExtension>();
    }
}