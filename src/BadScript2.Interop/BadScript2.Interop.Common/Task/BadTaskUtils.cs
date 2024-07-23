using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Utility;
namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Utilities for Tasks
/// </summary>
public static class BadTaskUtils
{
    /// <summary>
    ///     Waits for a C# Task to complete and returns the result as a BadObject
    /// </summary>
    /// <param name="t">Task</param>
    /// <param name="onComplete">On Complete Callback</param>
    /// <typeparam name="T">Task Return</typeparam>
    /// <returns>Awaitable Runnable</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the task fails</exception>
    public static BadInteropRunnable WaitForTask<T>(Task<T> t, Func<T, BadObject> onComplete)
    {
        BadInteropRunnable? runnable = null;

        runnable = new BadInteropRunnable(InnerWaitForTask());

        return runnable;

        IEnumerator<BadObject> InnerWaitForTask()
        {
            while (t is
                   {
                       IsCanceled: false, IsCompleted: false, IsFaulted: false,
                   })
            {
                yield return BadObject.Null;
            }

            if (t.IsCompletedSuccessfully())
            {
                // ReSharper disable once AccessToModifiedClosure
                runnable!.SetReturn(onComplete(t.Result));
            }
            else
            {
                throw new BadRuntimeException("Task Failed");
            }
        }
    }


    /// <param name="t">Task</param>
    /// <returns>Awaitable Runnable</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the task fails</exception>
    public static BadInteropRunnable WaitForTask(System.Threading.Tasks.Task t)
    {
        BadInteropRunnable? runnable = null;

        runnable = new BadInteropRunnable(InnerWaitForTask());

        return runnable;

        IEnumerator<BadObject> InnerWaitForTask()
        {
            while (t is
                   {
                       IsCanceled: false, IsCompleted: false, IsFaulted: false,
                   })
            {
                yield return BadObject.Null;
            }

            if (t.IsCompletedSuccessfully())
            {
                // ReSharper disable once AccessToModifiedClosure
                runnable!.SetReturn(BadObject.Null);
            }
            else
            {
                throw new BadRuntimeException("Task Failed");
            }
        }
    }

    /// <summary>
    ///     Waits for a C# Task to complete and returns the result as a BadObject
    /// </summary>
    /// <param name="t">Task</param>
    /// <typeparam name="T">Task Return</typeparam>
    /// <returns>Awaitable Runnable</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the task fails</exception>
    public static BadInteropRunnable WaitForTask<T>(Task<T> t)
    {
        BadInteropRunnable? runnable = null;

        runnable = new BadInteropRunnable(InnerWaitForTask());

        return runnable;

        IEnumerator<BadObject> InnerWaitForTask()
        {
            while (t is
                   {
                       IsCanceled: false, IsCompleted: false, IsFaulted: false,
                   })
            {
                yield return BadObject.Null;
            }

            if (t.IsCompletedSuccessfully())
            {
                // ReSharper disable once AccessToModifiedClosure
                runnable!.SetReturn(BadObject.Wrap(t.Result));
            }
            else
            {
                throw new BadRuntimeException("Task Failed");
            }
        }
    }
}