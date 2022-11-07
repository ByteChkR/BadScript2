using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

internal class BadLinqQuerySkipWhileCommand : BadLinqQueryCommand
{
    public BadLinqQuerySkipWhileCommand() : base(true, false, "SkipWhile") { }

    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.SkipWhile(data.Argument!);
    }
}