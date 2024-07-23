using System.Collections;
namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the SelectMany Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQuerySelectManyCommand : BadLinqQueryCommand
{
    /// <inheritdoc />
    public BadLinqQuerySelectManyCommand() : base(true, false, "SelectMany") { }

    /// <inheritdoc />
    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.SelectMany(data.Argument!);
    }
}