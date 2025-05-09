using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

internal class BadLinqQueryThenByDescendingCommand : BadLinqQueryCommand
{
    public BadLinqQueryThenByDescendingCommand() : base(true, false, "ThenByDescending")
    {
    }

    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        if(data.Data is not IOrderedEnumerable<object> o)
            throw new Exception("ThenBy can only be used after OrderBy");
        return o.ThenByDescending(data.Argument!);
    }
}