using BadScript2.Parser.Operators;

namespace BadScript2.Interop.Common.Task;

/// <summary>
///     The BadScript Task Runner
/// </summary>
public class BadTaskRunner
{
    /// <summary>
    ///     The Task Runner Instance
    /// </summary>
    public static readonly BadTaskRunner Instance = new BadTaskRunner();

    /// <summary>
    ///     The Task List
    /// </summary>
    private readonly List<BadTask> m_TaskList = new List<BadTask>();

    /// <summary>
    ///     Static Constructor
    /// </summary>
    static BadTaskRunner()
    {
        BadOperatorTable.Instance.AddValueParser(new BadAwaitValueParser());
    }


    /// <summary>
    ///     the Current task
    /// </summary>
    public BadTask? Current { get; private set; }

    /// <summary>
    ///     Is true if there are no tasks to run
    /// </summary>
    public bool IsIdle => m_TaskList.Count == 0;

    /// <summary>
    ///     Runs a single step of the Task Runner
    /// </summary>
    public void RunStep()
    {
        for (int i = m_TaskList.Count - 1; i >= 0; i--)
        {
            if (i >= m_TaskList.Count)
            {
                continue;
            }

            BadTask t = m_TaskList[i];
            Current = t;

            if (t.IsPaused)
            {
                continue;
            }

            if (!t.IsFinished)
            {
                for (int j = 0; j < BadTaskRunnerSettings.Instance.TaskIterationTime; j++)
                {
                    if (t.IsPaused)
                    {
                        break;
                    }

                    if (t.IsFinished || !t.Runnable.Enumerator.MoveNext())
                    {
                        t.Stop();

                        break;
                    }
                }
            }

            if (t.IsFinished)
            {
                m_TaskList.Remove(t);

                foreach (BadTask x in t.ContinuationTasks)
                {
                    if (x.IsFinished)
                    {
                        continue;
                    }

                    if (m_TaskList.Contains(x))
                    {
                        x.Resume();
                    }
                    else
                    {
                        m_TaskList.Add(x);
                        x.Start();
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Clears all Tasks
    /// </summary>
    public void Clear()
    {
        m_TaskList.Clear();
    }

    /// <summary>
    ///     Clears all Tasks from the given Creator
    /// </summary>
    /// <param name="creator">Creator</param>
    public void ClearTasksFrom(BadTask creator)
    {
        foreach (BadTask task in m_TaskList)
        {
            if (task.Creator == creator)
            {
                task.Cancel();
                ClearTasksFrom(task);
            }
        }
    }

    /// <summary>
    ///     Adds a Task to the Task Runner
    /// </summary>
    /// <param name="task">Task</param>
    /// <param name="runImmediately">Task starts immediately if true</param>
    public void AddTask(BadTask task, bool runImmediately = false)
    {
        m_TaskList.Add(task);
        task.SetCreator(Current);

        if (runImmediately)
        {
            task.Start();
        }
    }
}