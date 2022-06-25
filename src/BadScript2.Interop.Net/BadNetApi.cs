using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Net;

public class BadNetApi : BadInteropApi
{
    public BadNetApi() : base("Net") { }

    public override void Load(BadTable target)
    {
        target.SetFunction<string>("Get", Get);
    }

    public static BadInteropRunnable WaitForTask<T>(Task<T> t, Func<T, BadObject> onComplete)
    {
        BadInteropRunnable? runnable = null;
        IEnumerator<BadObject> InnerWaitForTask()
        {
            while (!t.IsCanceled && !t.IsCompleted && !t.IsFaulted)
            {
                yield return BadObject.Null;
            }

            if (t.IsCompletedSuccessfully)
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

    public static BadInteropRunnable WaitForTask<T>( Task<T> t)
    {
        BadInteropRunnable? runnable = null;
        IEnumerator<BadObject> InnerWaitForTask()
        {
            while (!t.IsCanceled && !t.IsCompleted && !t.IsFaulted)
            {
                yield return BadObject.Null;
            }

            if (t.IsCompletedSuccessfully)
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
    // public static IEnumerator<BadObject> WaitForTask<T>(BadExecutionContext caller, Task<T> t, BadFunction onComplete)
    // {
    //     while (!t.IsCanceled && !t.IsCompleted && !t.IsFaulted)
    //     {
    //         yield return BadObject.Null;
    //     }
    //
    //     if (t.IsCompletedSuccessfully)
    //     {
    //         foreach (BadObject o in onComplete.Invoke(new[] { BadObject.Wrap(t.Result) }, caller))
    //         {
    //             yield return o;
    //         }
    //     }
    //     else
    //     {
    //         throw new BadRuntimeException("Task Failed");
    //     }
    // }

    private BadTask Get(BadExecutionContext context, string url)
    {
        HttpClient cl = new HttpClient();
        Task<HttpResponseMessage>? task = cl.GetAsync(url);

        return new BadTask(WaitForTask(task), $"Net.Get(\"{url}\")");
    }
}