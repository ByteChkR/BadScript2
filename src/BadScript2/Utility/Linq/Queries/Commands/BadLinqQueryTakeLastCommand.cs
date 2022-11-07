using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

internal class BadLinqQueryTakeLastCommand : BadLinqQueryCommand
{
    public BadLinqQueryTakeLastCommand() : base(true, false, "TakeLast") { }

    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.TakeLast(int.Parse(data.Argument!));
    }
}