using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

internal class BadLinqQuerySelectManyCommand : BadLinqQueryCommand
{
	public BadLinqQuerySelectManyCommand() : base(true, false, "SelectMany") { }

	public override IEnumerable Run(BadLinqQueryCommandData data)
	{
		return data.Data.SelectMany(data.Argument!);
	}
}
