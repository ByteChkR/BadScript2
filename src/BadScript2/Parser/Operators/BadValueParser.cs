using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

/// <summary>
///     Base class for all Value Parsers
/// </summary>
public abstract class BadValueParser
{
    /// <summary>
    ///     Returns true if the Parser can parse the given Token
    /// </summary>
    /// <param name="parser">The Parser Instance</param>
    /// <returns>True if the Current Token can be parsed by this BadValueParser Instance.</returns>
    public abstract bool IsValue(BadSourceParser parser);

    /// <summary>
    ///     Parses the Current Token
    /// </summary>
    /// <param name="parser">The Parser instance</param>
    /// <returns>Parsed BadExpression</returns>
    public abstract BadExpression ParseValue(BadSourceParser parser);
}
