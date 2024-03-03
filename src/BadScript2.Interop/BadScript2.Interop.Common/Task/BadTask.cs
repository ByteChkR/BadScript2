using System.Text;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Implements a Task Object
/// </summary>
public class BadTask : BadObject
{
    /// <summary>
    ///     The BadTask Prototype
    /// </summary>
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

    /// <summary>
    ///     The List of Continuation Tasks
    /// </summary>
    private readonly List<BadTask> m_ContinuationTasks = new List<BadTask>();

    /// <summary>
    ///     The Task Properties
    /// </summary>
    private readonly Dictionary<string, BadObjectReference> m_Properties =
        new Dictionary<string, BadObjectReference>();

    /// <summary>
    ///     Constructs a new Task
    /// </summary>
    /// <param name="runnable">Runnable</param>
    /// <param name="name">Task Name</param>
    public BadTask(BadRunnable runnable, string name)
    {
        Name = name;
        Runnable = runnable;
        m_Properties.Add(
            "Name",
            BadObjectReference.Make("Task.Name", () => Name, (o, _) => Name = o is IBadString s ? s.Value : Name)
        );
        m_Properties.Add("IsCompleted", BadObjectReference.Make("Task.IsCompleted", () => IsFinished));
        m_Properties.Add("IsInactive", BadObjectReference.Make("Task.IsInactive", () => IsInactive));
        m_Properties.Add("IsPaused", BadObjectReference.Make("Task.IsPaused", () => IsPaused));
        m_Properties.Add("IsRunning", BadObjectReference.Make("Task.IsRunning", () => IsRunning));

        BadFunction continueFunc =
            new BadDynamicInteropFunction<BadTask>("ContinueWith", (_, t) => ContinueWith(t), BadAnyPrototype.Instance);
        BadFunction pauseFunc = new BadDynamicInteropFunction("Pause", Pause, BadAnyPrototype.Instance);
        BadFunction resumeFunc = new BadDynamicInteropFunction("Resume", Resume, BadAnyPrototype.Instance);
        BadFunction cancelFunc = new BadDynamicInteropFunction("Cancel", Cancel, BadAnyPrototype.Instance);
        m_Properties.Add("ContinueWith", BadObjectReference.Make("Task.ContinueWith", () => continueFunc));
        m_Properties.Add("Pause", BadObjectReference.Make("Task.Pause", () => pauseFunc));
        m_Properties.Add("Resume", BadObjectReference.Make("Task.Resume", () => resumeFunc));
        m_Properties.Add("Cancel", BadObjectReference.Make("Task.Cancel", () => cancelFunc));
        m_Properties.Add(
            "RunSynchronously",
            BadObjectReference.Make(
                "Task.RunSynchronously",
                () => new BadEnumerableInteropFunction("RunSynchronously", (_, _) => Enumerate(runnable), false, BadAnyPrototype.Instance)
            )
        );
        m_Properties.Add(
            "Run",
            BadObjectReference.Make(
                "Task.Run",
                () => new BadDynamicInteropFunction("Run", Run, BadAnyPrototype.Instance)
            )
        );
    }


    /// <summary>
    ///     Runnable of the Task
    /// </summary>
    public BadRunnable Runnable { get; }

    /// <summary>
    ///     Enumeration of Continuation Tasks
    /// </summary>
    public IEnumerable<BadTask> ContinuationTasks => m_ContinuationTasks;

    /// <summary>
    ///     The Creator of this Task
    /// </summary>
    public BadTask? Creator { get; private set; }

    /// <summary>
    ///     The Name of the Task
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Is True if the Task is not running, finished or paused
    /// </summary>
    public bool IsInactive => !IsRunning && !IsFinished && !IsPaused;

    /// <summary>
    ///     Is true if the task is running
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    ///     Is true if the task is finished
    /// </summary>
    public bool IsFinished { get; private set; }

    /// <summary>
    ///     Is true if the task is running
    /// </summary>
    public bool IsPaused { get; private set; }

    private void Run(BadExecutionContext context)
    {
        if (IsFinished)
        {
            throw BadRuntimeException.Create(context.Scope, "Task is already finished");
        }

        if (IsRunning)
        {
            throw BadRuntimeException.Create(context.Scope, "Task is already running");
        }

        BadTaskRunner runner = context.Scope.GetSingleton<BadTaskRunner>() ?? throw BadRuntimeException.Create(context.Scope, "Task Runner not found");
        if (IsPaused)
        {
            IsPaused = false;
        }

        if (!runner.IsTaskAdded(this))
        {
            runner.AddTask(this, true);
        }
    }

    private IEnumerable<BadObject> Enumerate(BadRunnable runnable)
    {
        IEnumerator<BadObject> e = runnable.Enumerator;
        while (e.MoveNext())
        {
            yield return e.Current ?? Null;
        }
    }

    /// <summary>
    ///     Adds a Continuation Task
    /// </summary>
    /// <param name="task"></param>
    public void AddContinuation(BadTask task)
    {
        m_ContinuationTasks.Add(task);
    }

    /// <summary>
    ///     Sets the Creator of this Task
    /// </summary>
    /// <param name="task">Creator</param>
    public void SetCreator(BadTask? task)
    {
        Creator = task;
    }

    /// <summary>
    ///     Starts the Task
    /// </summary>
    public void Start()
    {
        IsRunning = true;
    }

    /// <summary>
    ///     Stops the Task
    /// </summary>
    public void Stop()
    {
        IsFinished = true;
        IsRunning = false;
        IsPaused = false;
    }

    /// <summary>
    ///     Cancels the Task
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets raised if the task is not running</exception>
    public void Cancel()
    {
        if (!IsRunning)
        {
            throw new BadRuntimeException("Task is not running");
        }

        IsPaused = false;
        IsFinished = true;
    }


    /// <summary>
    ///     Resumes the Task
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets raised if the task is not running or paused</exception>
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


    /// <summary>
    ///     Pauses the Task
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets raised if the task is not running or is paused</exception>
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


    /// <summary>
    ///     Continues the Task with another Task
    /// </summary>
    /// <param name="task">Continuation</param>
    /// <returns>NULL</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the task is already finished</exception>
    private BadObject ContinueWith(BadTask task)
    {
        if (IsFinished)
        {
            throw new BadRuntimeException("Task is already finished");
        }

        AddContinuation(task);

        return Null;
    }

    /// <summary>
    ///     Creates a new Task from a Function
    /// </summary>
    /// <param name="f">Function</param>
    /// <param name="caller">Caller Context</param>
    /// <param name="name">Name</param>
    /// <param name="args">Function Arguments</param>
    /// <returns>Bad Task Instance</returns>
    public static BadTask Create(BadFunction f, BadExecutionContext caller, string? name, params BadObject[] args)
    {
        return new BadTask(BadRunnable.Create(f, caller, args), name ?? f.ToString());
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        return m_Properties.TryGetValue(propName, out BadObjectReference? property) ? property : base.GetProperty(propName, caller);
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");

        foreach (KeyValuePair<string, BadObjectReference> props in m_Properties)
        {
            sb.AppendLine($"\t{props.Key}: {props.Value.Dereference()}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return m_Properties.ContainsKey(propName) ||
               base.HasProperty(propName, caller);
    }
}