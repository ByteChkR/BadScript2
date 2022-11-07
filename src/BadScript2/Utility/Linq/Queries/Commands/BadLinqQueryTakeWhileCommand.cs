using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

internal class BadLinqQueryTakeWhileCommand : BadLinqQueryCommand
{
    public BadLinqQueryTakeWhileCommand() : base(true, false, "TakeWhile") { }

    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.TakeWhile(data.Argument!);
    }
}