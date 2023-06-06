using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Task;

/// <summary>
/// Implements the 'Concurrent' API
/// </summary>
public class BadTaskRunnerApi : BadInteropApi
{
	/// <summary>
	/// The Runner Instance
	/// </summary>
	private readonly BadTaskRunner m_Runner;

	/// <summary>
	/// Creates a new API Instance
	/// </summary>
	/// <param name="runner">Task Runner Instance</param>
	public BadTaskRunnerApi(BadTaskRunner runner) : base("Concurrent")
	{
		m_Runner = runner;
	}

	protected override void LoadApi(BadTable target)
	{
		target.SetFunction<BadTask>("Run", AddTask);
		target.SetFunction("GetCurrent", GetCurrentTask);
		target.SetFunction<BadFunction>("Create", CreateTask);
	}

	/// <summary>
	/// Returns the Current Task
	/// </summary>
	/// <returns>Task</returns>
	private BadObject GetCurrentTask()
	{
		return m_Runner.Current ?? BadObject.Null;
	}

	/// <summary>
	/// Adds a Task to the Runner
	/// </summary>
	/// <param name="task">Task</param>
	private void AddTask(BadTask task)
	{
		m_Runner.AddTask(task, true);
	}

	/// <summary>
	/// Creates a new Task
	/// </summary>
	/// <param name="caller">Caller Context</param>
	/// <param name="func">Function</param>
	/// <returns>Task</returns>
	private BadObject CreateTask(BadExecutionContext caller, BadFunction func)
	{
		return BadTask.Create(func, caller, null);
	}
}
