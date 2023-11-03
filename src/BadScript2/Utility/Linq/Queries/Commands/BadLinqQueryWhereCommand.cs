using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

internal class BadLinqQueryWhereCommand : BadLinqQueryCommand
{
	public BadLinqQueryWhereCommand() : base(true, false, "Where") { }

	public override IEnumerable Run(BadLinqQueryCommandData data)
	{
		return data.Data.Where(data.Argument!);
	}
}
