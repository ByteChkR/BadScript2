using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the TakeLast Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQueryTakeLastCommand : BadLinqQueryCommand
{
	public BadLinqQueryTakeLastCommand() : base(true, false, "TakeLast") { }

	public override IEnumerable Run(BadLinqQueryCommandData data)
	{
		return data.Data.TakeLast(int.Parse(data.Argument!));
	}
}
