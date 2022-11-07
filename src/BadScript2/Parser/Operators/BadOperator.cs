namespace BadScript2.Parser.Operators;

/// <summary>
///     Base Class of All Operators
/// </summary>
public abstract class BadOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    /// <param name="precedence">The Precedence of the Operator</param>
    /// <param name="symbol">The Operator Symbol</param>
    protected BadOperator(int precedence, string symbol)
    {
        Precedence = precedence;
        Symbol = symbol;
    }

    /// <summary>
    ///     The Precedence of the Operator
    /// </summary>
    public int Precedence { get; }

    /// <summary>
    ///     The Operator Symbol
    /// </summary>
    public string Symbol { get; }
}