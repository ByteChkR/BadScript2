using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the OrderBy Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQueryOrderByCommand : BadLinqQueryCommand
{
    /// <inheritdoc />
    public BadLinqQueryOrderByCommand() : base(true, false, "OrderBy") { }

    /// <inheritdoc />
    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.OrderBy(data.Argument!);
    }
}