using BadScript2.Settings;

namespace BadScript2.Interop.Common.Task;

public class BadTaskRunnerSettings : BadSettingsProvider<BadTaskRunnerSettings>
{
    private BadSettings? m_TaskIterationTimeObj;

    public BadTaskRunnerSettings() : base("Runtime.Task") { }

    private BadSettings? TaskIterationTimeObj
    {
        get
        {
            if (m_TaskIterationTimeObj == null && Settings != null && Settings.HasProperty(nameof(TaskIterationTime)))
            {
                m_TaskIterationTimeObj = Settings.GetProperty(nameof(TaskIterationTime));
            }

            return m_TaskIterationTimeObj;
        }
    }

    public int TaskIterationTime
    {
        get => m_TaskIterationTimeObj?.GetValue<int>() ?? 1;
        set => m_TaskIterationTimeObj?.SetValue(value);
    }
}