namespace BadScript2.Utility;

/// <summary>
///     Implements Enum functions that are missing in .NET Standard 2.0
/// </summary>
public static class BadEnum
{
	/// <summary>
	///     Parses a string to an enum
	/// </summary>
	/// <param name="value">The String Value</param>
	/// <param name="ignoreCase">Should we ignore case?</param>
	/// <typeparam name="T">Enum Type</typeparam>
	/// <returns>The Enum Value</returns>
	public static T Parse<T>(string value, bool ignoreCase) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

	/// <summary>
	///     Parses a string to an enum
	/// </summary>
	/// <param name="value">The String Value</param>
	/// <typeparam name="T">Enum Type</typeparam>
	/// <returns>The Enum Value</returns>
	public static T Parse<T>(string value) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), value);
    }
}