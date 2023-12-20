using System.Collections;

namespace BadScript2.Utility.Linq.Queries;

/// <summary>
///     Implements the Command Data for the BadLinqQuery.
/// </summary>
public class BadLinqQueryCommandData
{
	/// <summary>
	///     Creates a new Command Data Instance.
	/// </summary>
	/// <param name="data">The Data</param>
	/// <param name="argument">The Argument</param>
	public BadLinqQueryCommandData(IEnumerable data, string? argument = null)
	{
		Data = data;
		Argument = argument;
	}

	/// <summary>
	///     The Data.
	/// </summary>
	public IEnumerable Data { get; set; }

	/// <summary>
	///     The Argument.
	/// </summary>
	public string? Argument { get; set; }
}
