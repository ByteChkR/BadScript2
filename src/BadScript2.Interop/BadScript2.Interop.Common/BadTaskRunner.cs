namespace BadScript2.Interop.Common;

public class BadTaskRunner
{
    private readonly List<BadTask> m_TaskList = new List<BadTask>();

    public BadTask? Current { get; private set; }
    public bool IsIdle => m_TaskList.Count == 0;

    public void RunStep()
    {
        for (int i = m_TaskList.Count - 1; i >= 0; i--)
        {
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

                    if (t.IsFinished || !t.Enumerator.MoveNext())
                    {
                        t.Stop();

                        break;
                    }
                }
            }

            if (t.IsFinished)
            {
                m_TaskList.Remove(t);
                t.ContinuationTasks.ForEach(
                    x =>
                    {
                        m_TaskList.Add(x);
                        x.Start();
                    }
                );
            }
        }
    }

    public void AddTask(BadTask task, bool runImmediately = false)
    {
        m_TaskList.Add(task);
        if (runImmediately)
        {
            task.Start();
        }
    }
}