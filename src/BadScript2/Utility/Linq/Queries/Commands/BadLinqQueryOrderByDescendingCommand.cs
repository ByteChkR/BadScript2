using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

internal class BadLinqQueryOrderByDescendingCommand : BadLinqQueryCommand
{
    public BadLinqQueryOrderByDescendingCommand() : base(true, false, "OrderByDescending") { }

    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.OrderByDescending(data.Argument!);
    }
}