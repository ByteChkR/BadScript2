using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Implements the 'Concurrent' API
/// </summary>
[BadInteropApi("Concurrent", true)]
internal partial class BadTaskRunnerApi
{
    /// <summary>
    ///     The Runner Instance
    /// </summary>
    private readonly BadTaskRunner m_Runner = BadTaskRunner.Instance;

    /// <summary>
    ///     Creates a new API Instance
    /// </summary>
    /// <param name="runner">Task Runner Instance</param>
    public BadTaskRunnerApi(BadTaskRunner runner) : this()
    {
        m_Runner = runner;
    }


    /// <summary>
    ///     Returns the Current Task
    /// </summary>
    /// <returns>Task</returns>
    [BadMethod("GetCurrent", "Returns the Current Task")]
    [return: BadReturn("The Current Task")]
    private BadObject GetCurrentTask()
    {
        return m_Runner.Current ?? BadObject.Null;
    }

    /// <summary>
    ///     Adds a Task to the Runner
    /// </summary>
    /// <param name="task">Task</param>
    [BadMethod("Run", "Runs a Task")]
    private void AddTask([BadParameter(description: "The Task that will be executed")] BadTask task)
    {
        m_Runner.AddTask(task, true);
    }

    /// <summary>
    ///     Creates a new Task
    /// </summary>
    /// <param name="caller">Caller Context</param>
    /// <param name="func">Function</param>
    /// <returns>Task</returns>
    [BadMethod("Create", "Creates a new Task")]
    [return: BadReturn("The Created Task")]
    private static BadTask CreateTask(BadExecutionContext caller,
                                      [BadParameter(description: "The Function that will be executed within the task.")]
                                      BadFunction func)
    {
        return BadTask.Create(func, caller, null);
    }
}