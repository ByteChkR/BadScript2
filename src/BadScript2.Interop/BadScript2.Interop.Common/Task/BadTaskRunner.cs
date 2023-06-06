using BadScript2.Parser.Operators;

namespace BadScript2.Interop.Common.Task;

public class BadTaskRunner
{
	public static readonly BadTaskRunner Instance = new BadTaskRunner();
	private readonly List<BadTask> m_TaskList = new List<BadTask>();

	static BadTaskRunner()
	{
		BadOperatorTable.Instance.AddValueParser(new BadAwaitValueParser());
	}


	public BadTask? Current { get; private set; }

	public bool IsIdle => m_TaskList.Count == 0;

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

	public void Clear()
	{
		m_TaskList.Clear();
	}

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
