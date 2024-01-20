using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Implements the 'AsTask' Function
/// </summary>
public class BadTaskExtensions : BadInteropExtension
{
    /// <inheritdoc/>
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<BadFunction>(
            "AsTask",
            func => new BadInteropFunction(
                "AsTask",
                (ctx, args) => AsTask(ctx, func, args),
                func.IsStatic,
                BadTask.Prototype,
                func.Parameters
            )
        );
    }

    /// <summary>
    ///     Converts a Function into a task
    /// </summary>
    /// <param name="ctx">Execution Context</param>
    /// <param name="func">Function</param>
    /// <param name="args">Arguments</param>
    /// <returns>BadTask</returns>
    private static BadObject AsTask(BadExecutionContext ctx, BadFunction func, BadObject[] args)
    {
        return BadTask.Create(func, ctx, func.Name?.Text, args);
    }
}