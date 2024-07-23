using System.Collections;
namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the Where Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQueryWhereCommand : BadLinqQueryCommand
{
    /// <inheritdoc />
    public BadLinqQueryWhereCommand() : base(true, false, "Where") { }

    /// <inheritdoc />
    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.Where(data.Argument!);
    }
}