using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

internal class BadLinqQueryThenByCommand : BadLinqQueryCommand
{
    public BadLinqQueryThenByCommand() : base(true, false, "ThenBy")
    {
    }

    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        if(data.Data is not IOrderedEnumerable<object> o)
            throw new Exception("ThenBy can only be used after OrderBy");
        return o.ThenBy(data.Argument!);
    }
}