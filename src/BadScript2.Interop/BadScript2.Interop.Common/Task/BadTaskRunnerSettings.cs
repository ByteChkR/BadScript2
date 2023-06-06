using BadScript2.Settings;

namespace BadScript2.Interop.Common.Task;

/// <summary>
/// Settings of the Task Runner
/// </summary>
public class BadTaskRunnerSettings : BadSettingsProvider<BadTaskRunnerSettings>
{
	/// <summary>
	/// The Iteration Time of the Task Runner(how many iterations per runner step)
	/// </summary>
	private BadSettings? m_TaskIterationTimeObj;

	/// <summary>
	/// Creates a new Settings Instance
	/// </summary>
	public BadTaskRunnerSettings() : base("Runtime.Task") { }
	/// <summary>
	/// The Iteration Time of the Task Runner(how many iterations per runner step)
	/// </summary>
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
	/// <summary>
	/// The Iteration Time of the Task Runner(how many iterations per runner step)
	/// </summary>
	public int TaskIterationTime
	{
		get => m_TaskIterationTimeObj?.GetValue<int>() ?? 1;
		set => m_TaskIterationTimeObj?.SetValue(value);
	}
}
