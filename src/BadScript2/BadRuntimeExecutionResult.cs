using BadScript2.Runtime.Objects;

namespace BadScript2;

/// <summary>
///     Gets returned by the bad runtime when an execution is finished
/// </summary>
public readonly struct BadRuntimeExecutionResult
{
    /// <summary>
    ///     The Result of the Execution
    /// </summary>
    public readonly BadObject Result;

    /// <summary>
    ///     The Exported Object
    /// </summary>
    public readonly BadObject? Exports;

    /// <summary>
    ///     Creates a new BadRuntimeExecutionResult
    /// </summary>
    /// <param name="result">The Result of the Execution</param>
    /// <param name="exports">The Exported Object</param>
    public BadRuntimeExecutionResult(BadObject result, BadObject? exports)
    {
        Result = result;
        Exports = exports;
    }
}