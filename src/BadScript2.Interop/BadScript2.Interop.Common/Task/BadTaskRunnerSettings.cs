using BadScript2.Settings;
namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Settings of the Task Runner
/// </summary>
public class BadTaskRunnerSettings : BadSettingsProvider<BadTaskRunnerSettings>
{
    private readonly BadEditableSetting<BadTaskRunnerSettings, int> m_TaskIterationTime =
        new BadEditableSetting<BadTaskRunnerSettings, int>(nameof(TaskIterationTime), 1);

    /// <summary>
    ///     Creates a new Settings Instance
    /// </summary>
    public BadTaskRunnerSettings() : base("Runtime.Task") { }

    /// <summary>
    ///     The Iteration Time of the Task Runner(how many iterations per runner step)
    /// </summary>
    public int TaskIterationTime
    {
        get => m_TaskIterationTime.GetValue();
        set => m_TaskIterationTime.Set(value);
    }
}