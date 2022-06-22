using BadScript2.Settings;

namespace BadScript2.Interop.Common;

public class BadTaskRunnerSettings : BadSettingsProvider<BadTaskRunnerSettings>
{
    public BadTaskRunnerSettings() : base("Runtime.Task")
    {
        
    }

    private BadSettings? m_TaskIterationTimeObj;

    private BadSettings? TaskIterationTimeObj =>
        m_TaskIterationTimeObj ??= Settings?.GetProperty(nameof(TaskIterationTime));
    public int TaskIterationTime
    {
        get => m_TaskIterationTimeObj?.GetValue<int>() ?? 1;
        set => m_TaskIterationTimeObj?.SetValue(value);
    }
}