using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the Select Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQuerySelectCommand : BadLinqQueryCommand
{
	public BadLinqQuerySelectCommand() : base(true, false, "Select") { }

	public override IEnumerable Run(BadLinqQueryCommandData data)
	{
		return data.Data.Select(data.Argument!);
	}
}
