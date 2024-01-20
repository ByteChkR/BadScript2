using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the OrderByDescending Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQueryOrderByDescendingCommand : BadLinqQueryCommand
{
    /// <inheritdoc />
    public BadLinqQueryOrderByDescendingCommand() : base(true, false, "OrderByDescending") { }

    /// <inheritdoc />
    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.OrderByDescending(data.Argument!);
    }
}