using System.Collections;
namespace BadScript2.Utility.Linq.Queries.Commands;

/// <summary>
///     Implements the Select Command for the BadLinqQuery.
/// </summary>
internal class BadLinqQuerySkipCommand : BadLinqQueryCommand
{
    /// <inheritdoc />
    public BadLinqQuerySkipCommand() : base(true, false, "Skip") { }

    /// <inheritdoc />
    public override IEnumerable Run(BadLinqQueryCommandData data)
    {
        return data.Data.Skip(int.Parse(data.Argument!));
    }
}