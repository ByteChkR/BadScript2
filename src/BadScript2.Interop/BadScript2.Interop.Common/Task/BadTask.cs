using System.Text;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Task;

public class BadTask : BadObject
{
    public static readonly BadClassPrototype Prototype = new BadNativeClassPrototype<BadTask>(
        "Task",
        (ctx, args) =>
        {
            if (args.Length != 2)
            {
                throw new BadRuntimeException("Task constructor takes 2 arguments");
            }

            if (args[0] is not IBadString name)
            {
                throw new BadRuntimeException("Task constructor takes a string as first argument");
            }

            if (args[1] is not BadFunction f)
            {
                throw new BadRuntimeException("Task constructor takes a function as second argument");
            }

            return new BadTask(BadRunnable.Create(f, ctx), name.Value);
        }
    );

    public readonly List<BadTask> ContinuationTasks = new List<BadTask>();
    public readonly BadRunnable Runnable;

    private readonly Dictionary<BadObject, BadObjectReference> m_Properties =
        new Dictionary<BadObject, BadObjectReference>();


    public BadTask(BadRunnable runnable, string name)
    {
        Name = name;
        Runnable = runnable;
        m_Properties.Add(
            "Name",
            BadObjectReference.Make("Task.Name", () => Name, (o, t) => Name = o is IBadString s ? s.Value : Name)
        );
        m_Properties.Add("IsCompleted", BadObjectReference.Make("Task.IsCompleted", () => IsFinished));
        m_Properties.Add("IsInactive", BadObjectReference.Make("Task.IsInactive", () => IsInactive));
        m_Properties.Add("IsPaused", BadObjectReference.Make("Task.IsPaused", () => IsPaused));
        m_Properties.Add("IsRunning", BadObjectReference.Make("Task.IsRunning", () => IsRunning));

        BadFunction continueFunc = new BadDynamicInteropFunction<BadTask>("ContinueWith", ContinueWith);
        BadFunction pauseFunc = new BadDynamicInteropFunction("Pause", Pause);
        BadFunction resumeFunc = new BadDynamicInteropFunction("Resume", Resume);
        BadFunction cancelFunc = new BadDynamicInteropFunction("Cancel", Cancel);
        m_Properties.Add("ContinueWith", BadObjectReference.Make("Task.ContinueWith", () => continueFunc));
        m_Properties.Add("Pause", BadObjectReference.Make("Task.Pause", () => pauseFunc));
        m_Properties.Add("Resume", BadObjectReference.Make("Task.Resume", () => resumeFunc));
        m_Properties.Add("Cancel", BadObjectReference.Make("Task.Cancel", () => cancelFunc));
    }

    public string Name { get; set; }
    public bool IsInactive => !IsRunning && !IsFinished && !IsPaused;
    public bool IsRunning { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsPaused { get; private set; }

    public void Start()
    {
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = !(IsFinished = true);
    }

    public void Cancel()
    {
        if (!IsRunning)
        {
            throw new BadRuntimeException("Task is not running");
        }

        IsFinished = true;
    }

    private BadObject Cancel(BadExecutionContext arg)
    {
        Cancel();

        return Null;
    }

    public void Resume()
    {
        if (!IsRunning)
        {
            throw new BadRuntimeException("Task is not running");
        }

        if (!IsPaused)
        {
            throw new BadRuntimeException("Task is not paused");
        }

        IsPaused = false;

    }
    private BadObject Resume(BadExecutionContext arg)
    {
        Resume();
        return Null;
    }

    public void Pause()
    {
        if (!IsRunning)
        {
            throw new BadRuntimeException("Task is not running");
        }

        if (IsPaused)
        {
            throw new BadRuntimeException("Task is already paused");
        }

        IsPaused = true;
    }
    private BadObject Pause(BadExecutionContext arg)
    {
        Pause();

        return Null;
    }

    private BadObject ContinueWith(BadExecutionContext caller, BadTask task)
    {
        if (IsFinished)
        {
            throw new BadRuntimeException("Task is already finished");
        }

        ContinuationTasks.Add(task);

        return Null;
    }

    public static BadObject Create(BadFunction f, BadExecutionContext caller, string? name, params BadObject[] args)
    {
        return new BadTask(BadRunnable.Create(f, caller, args), name ?? f.ToString());
    }

    public override BadObjectReference GetProperty(BadObject propName)
    {
        if (m_Properties.ContainsKey(propName))
        {
            return m_Properties[propName];
        }


        return base.GetProperty(propName);
    }

    public override string ToSafeString(List<BadObject> done)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        foreach (KeyValuePair<BadObject, BadObjectReference> props in m_Properties)
        {
            sb.AppendLine($"\t{props.Key}: {props.Value.Dereference()}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    public override bool HasProperty(BadObject propName)
    {
        return m_Properties.ContainsKey(propName) ||
               base.HasProperty(propName);
    }
}