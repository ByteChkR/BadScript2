using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;
/// <summary>
/// Implements the SkipLast Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQuerySkipLastCommand : BadLinqQueryCommand
{
	public BadLinqQuerySkipLastCommand() : base(true, false, "SkipLast") { }

	public override IEnumerable Run(BadLinqQueryCommandData data)
	{
		return data.Data.SkipLast(int.Parse(data.Argument!));
	}
}
