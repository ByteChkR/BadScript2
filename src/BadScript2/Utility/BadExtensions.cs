using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Utility;

/// <summary>
///     Implements Extensions that aim to fix api differences between .Net Standard and .Net/.Net Framework
/// </summary>
public static class BadExtensions
{
	/// <summary>
	///     Returns true if the Task is completed successfully
	/// </summary>
	/// <param name="task">The Task</param>
	/// <returns>True if completed successfully</returns>
	public static bool IsCompletedSuccessfully(this Task task)
    {
        return task.Status == TaskStatus.RanToCompletion;
    }

	/// <summary>
	///     Returns true if the string ends with the given character
	/// </summary>
	/// <param name="str">The String</param>
	/// <param name="c">The Character</param>
	/// <returns>True if string ends with character</returns>
	public static bool EndsWith(this string str, char c)
    {
        return str[str.Length - 1] == c;
    }

	/// <summary>
	///     Returns true if the string starts with the given character
	/// </summary>
	/// <param name="str">The String</param>
	/// <param name="c">The Character</param>
	/// <returns>True if string starts with character</returns>
	public static bool StartsWith(this string str, char c)
    {
        return str[0] == c;
    }

	/// <summary>
	///     Skips the last n elements of an IEnumerable
	/// </summary>
	/// <param name="e">The Enumerable</param>
	/// <param name="count">The amount of elements to skip</param>
	/// <typeparam name="T">Element Type</typeparam>
	/// <returns>Enumerable with the last 'count' elements removed</returns>
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

	/// <summary>
	/// Returns a Property, unwrapped to the specified type, from the given object
	/// </summary>
	/// <param name="obj">The Object</param>
	/// <param name="propName">The Property Name</param>
	/// <typeparam name="T">The Property Type</typeparam>
	/// <returns>The Property Value</returns>
    public static T GetProperty<T>(this BadObject obj, BadObject propName)
    {
        BadObjectReference reference = obj.GetProperty(propName);

        return reference.Dereference().Unwrap<T>();
    }
}