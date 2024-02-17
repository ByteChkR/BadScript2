namespace BadScript2.Parser.Expressions;

/// <summary>
/// Gets inherited by all Expressions that have a Name(e.g. Variable Definitions, Function Definitions, etc.)
/// </summary>
public interface IBadNamedExpression
{
    /// <summary>
    /// Returns the Name of the Expression
    /// </summary>
    /// <returns>The Name of the Expression</returns>
    string? GetName();
}