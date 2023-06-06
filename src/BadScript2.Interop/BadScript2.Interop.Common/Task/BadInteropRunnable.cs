using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Task;

/// <summary>
/// Implements a Runnable that can return a value
/// </summary>
public class BadInteropRunnable : BadRunnable
{
	/// <summary>
	/// The Return Value
	/// </summary>
	private BadObject m_ReturnValue = BadObject.Null;


	/// <summary>
	/// Creates a new Runnable
	/// </summary>
	/// <param name="enumerator">Enumeration</param>
	/// <param name="setLastAsReturn">Set Last object as return</param>
	public BadInteropRunnable(IEnumerator<BadObject> enumerator, bool setLastAsReturn = false)
	{
		Enumerator = setLastAsReturn ? CreateEnumerator(enumerator) : enumerator;
	}

	public override IEnumerator<BadObject> Enumerator { get; }

	/// <summary>
	/// Creates the Enumerator
	/// </summary>
	/// <param name="enumerator">Enumeration</param>
	/// <returns>Enumeration</returns>
	private IEnumerator<BadObject> CreateEnumerator(IEnumerator<BadObject> enumerator)
	{
		BadObject last = enumerator.Current ?? BadObject.Null;

		while (enumerator.MoveNext())
		{
			last = enumerator.Current ?? BadObject.Null;

			yield return last;
		}

		SetReturn(last);
	}

	/// <summary>
	/// Sets the Return Value
	/// </summary>
	/// <param name="obj">The Return Value</param>
	public void SetReturn(BadObject obj)
	{
		m_ReturnValue = obj;
	}

	public override BadObject GetReturn()
	{
		return m_ReturnValue;
	}
}
