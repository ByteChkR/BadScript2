using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the Take Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQueryTakeCommand : BadLinqQueryCommand
{
    /// <inheritdoc />
    public BadLinqQueryTakeCommand() : base(true, false, "Take") { }

    /// <inheritdoc />
    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.Take(int.Parse(data.Argument!));
    }
}