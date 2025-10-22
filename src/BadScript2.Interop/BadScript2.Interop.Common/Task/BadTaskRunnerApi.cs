using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Implements the 'Concurrent' API
/// </summary>
[BadInteropApi("Concurrent")]
internal partial class BadTaskRunnerApi
{
    /// <summary>
    ///     Returns the Current Task
    /// </summary>
    /// <returns>Task</returns>
    [BadMethod("GetCurrent", "Returns the Current Task")]
    [return: BadReturn("The Current Task")]
    private BadObject GetCurrentTask(BadExecutionContext context)
    {
        var runner = context.Scope.GetSingleton<BadTaskRunner>();
        if (runner == null)
        {
            throw BadRuntimeException.Create(context.Scope, "No Task Runner found");
        }
        return runner.Current ?? BadObject.Null;
    }

    /// <summary>
    ///     Adds a Task to the Runner
    /// </summary>
    /// <param name="context">The Execution Context</param>
    /// <param name="task">Task</param>
    [BadMethod("Run", "Runs a Task")]
    private void AddTask(BadExecutionContext context, [BadParameter(description: "The Task that will be executed")] BadTask task)
    {
        
        var runner = context.Scope.GetSingleton<BadTaskRunner>();
        if (runner == null)
        {
            throw BadRuntimeException.Create(context.Scope, "No Task Runner found");
        }
        runner.AddTask(task, true);
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