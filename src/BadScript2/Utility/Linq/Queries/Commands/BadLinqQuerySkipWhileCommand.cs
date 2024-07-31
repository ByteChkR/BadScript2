using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the SkipWhile Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQuerySkipWhileCommand : BadLinqQueryCommand
{
    /// <inheritdoc />
    public BadLinqQuerySkipWhileCommand() : base(true, false, "SkipWhile") { }

    /// <inheritdoc />
    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.SkipWhile(data.Argument!);
    }
}