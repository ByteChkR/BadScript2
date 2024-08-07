using System.Collections;
namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the TakeWhile Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQueryTakeWhileCommand : BadLinqQueryCommand
{
    /// <inheritdoc />
    public BadLinqQueryTakeWhileCommand() : base(true, false, "TakeWhile") { }

    /// <inheritdoc />
    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.TakeWhile(data.Argument!);
    }
}