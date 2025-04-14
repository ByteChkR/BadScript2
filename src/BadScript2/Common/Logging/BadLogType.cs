namespace BadScript2.Common.Logging;

/// <summary>
///     Log Types
/// </summary>
public enum BadLogType
{
    /// <summary>
    /// Log (used for Debugging)
    /// </summary>
    Log,
    /// <summary>
    /// Warning most of the time this is visible.
    /// </summary>
    Warning,
    /// <summary>
    /// Error. This is always visible. and indicates a serious problem.
    /// </summary>
    Error,
}