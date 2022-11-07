using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Utility;

namespace BadScript2.Interop.Common.Task;

public static class BadTaskUtils
{
    public static BadInteropRunnable WaitForTask<T>(Task<T> t, Func<T, BadObject> onComplete)
    {
        BadInteropRunnable? runnable = null;

        IEnumerator<BadObject> InnerWaitForTask()
        {
            while (!t.IsCanceled && !t.IsCompleted && !t.IsFaulted)
            {
                yield return BadObject.Null;
            }

            if (t.IsCompletedSuccessfully())
            {
                runnable!.SetReturn(onComplete(t.Result));
            }
            else
            {
                throw new BadRuntimeException("Task Failed");
            }
        }

        runnable = new BadInteropRunnable(InnerWaitForTask());

        return runnable;
    }

    public static BadInteropRunnable WaitForTask(System.Threading.Tasks.Task t)
    {
        BadInteropRunnable? runnable = null;

        IEnumerator<BadObject> InnerWaitForTask()
        {
            while (!t.IsCanceled && !t.IsCompleted && !t.IsFaulted)
            {
                yield return BadObject.Null;
            }

            if (t.IsCompletedSuccessfully())
            {
                runnable!.SetReturn(BadObject.Null);
            }
            else
            {
                throw new BadRuntimeException("Task Failed");
            }
        }

        runnable = new BadInteropRunnable(InnerWaitForTask());

        return runnable;
    }

    public static BadInteropRunnable WaitForTask<T>(Task<T> t)
    {
        BadInteropRunnable? runnable = null;

        IEnumerator<BadObject> InnerWaitForTask()
        {
            while (!t.IsCanceled && !t.IsCompleted && !t.IsFaulted)
            {
                yield return BadObject.Null;
            }

            if (t.IsCompletedSuccessfully())
            {
                runnable!.SetReturn(BadObject.Wrap(t.Result));
            }
            else
            {
                throw new BadRuntimeException("Task Failed");
            }
        }

        runnable = new BadInteropRunnable(InnerWaitForTask());

        return runnable;
    }
}