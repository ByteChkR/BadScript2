namespace BadScript2.Utility;

public static class BadExtensions
{
	public static bool IsCompletedSuccessfully(this Task task)
	{
		return task.Status == TaskStatus.RanToCompletion;
	}

	public static bool EndsWith(this string str, char c)
	{
		return str[str.Length - 1] == c;
	}

	public static bool StartsWith(this string str, char c)
	{
		return str[0] == c;
	}

	public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> e, int count)
	{
		Queue<T> q = new Queue<T>(count + 1);

		foreach (T item in e)
		{
			if (q.Count == count + 1)
			{
				yield return q.Dequeue();
			}

			q.Enqueue(item);
		}

		if (q.Count == count + 1)
		{
			yield return q.Dequeue();
		}
	}
}
