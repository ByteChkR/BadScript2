namespace BadScript2.Parser.Expressions.Access;

/// <summary>
///     Defines the interface for BadScript Access Expressions
/// </summary>
public interface IBadAccessExpression
{
    /// <summary>
    ///     indicates if the expression will be null-checked by the runtime
    /// </summary>
    bool NullChecked { get; }
}