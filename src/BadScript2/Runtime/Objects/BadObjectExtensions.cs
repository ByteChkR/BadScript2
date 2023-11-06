namespace BadScript2.Runtime.Objects;

/// <summary>
///     Object Extensions for the BadScript Runtime
/// </summary>
public static class BadObjectExtensions
{
	/// <summary>
	///     Returns a String Representation of this Object. This function is recursion proof and supports circular references
	/// </summary>
	/// <param name="obj">The Object instance</param>
	/// <returns>String Representation</returns>
	public static string ToSafeString(this BadObject obj)
    {
        return obj.ToSafeString(new List<BadObject>());
    }

	/// <summary>
	///     Dereferences the Object and returns the underlying value
	/// </summary>
	/// <param name="obj">The Object</param>
	/// <returns>
	///     The Dereferenced Value. Returns value of <paramref name="obj" /> if obj is not of type
	///     <see cref="BadObjectReference" />
	/// </returns>
	public static BadObject Dereference(this BadObject obj)
    {
        while (obj is BadObjectReference r)
        {
            obj = r.Resolve();
        }

        return obj;
    }
}