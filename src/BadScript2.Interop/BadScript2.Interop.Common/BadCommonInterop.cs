using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Apis;
using BadScript2.Interop.Common.Extensions;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.Common.Versioning;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
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
    ///     All Common Interop Apis
    /// </summary>
    private static readonly BadInteropApi[] s_CommonApis =
    {
        new BadConsoleApi(BadConsole.GetConsole()),
        new BadRuntimeApi(),
        new BadMathApi(),
        new BadOperatingSystemApi(),
        new BadXmlApi(),
    };

    /// <summary>
    ///     All Common Interop Apis
    /// </summary>
    public static IEnumerable<BadInteropApi> Apis => s_CommonApis;


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

        BadTaskRunner.Instance.AddTask(task,
                                       true
                                      );

        while (!BadTaskRunner.Instance.IsIdle)
        {
            BadTaskRunner.Instance.RunStep();
        }

        return task.Runnable.GetReturn();
    }

    /// <summary>
    ///     Configures the Runtime to use the Common Interop Extensions
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <param name="useAsync">Whether to use the Async Extensions</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseCommonInterop(this BadRuntime runtime, bool useAsync = true)
    {
        if (useAsync)
        {
            if (BadNativeClassBuilder.NativeTypes.All(x => x.Name != BadTask.Prototype.Name))
            {
                BadNativeClassBuilder.AddNative(BadTask.Prototype);
            }

            runtime
                .UseApi(new BadTaskRunnerApi(BadTaskRunner.Instance), true)
                .UseExecutor(ExecuteTask)
                .UseSingleton(BadTaskRunner.Instance);
        }

        if (BadNativeClassBuilder.NativeTypes.All(x => x.Name != BadVersion.Prototype.Name))
        {
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
        }

        return runtime.UseCommonExtensions(useAsync)
                      .UseApis(Apis, true);
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
            return runtime.UseApi(new BadConsoleApi(console), true);
        }

        return runtime.UseApi(new BadConsoleApi(BadConsole.GetConsole()), true);
    }

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