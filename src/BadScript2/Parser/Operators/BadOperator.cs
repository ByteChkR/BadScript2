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
	/// <param name="isLeftAssociative">Set to true if the Operator needs to be evaluated left to right</param>
	protected BadOperator(int precedence, string symbol, bool isLeftAssociative = true)
	{
		Precedence = precedence;
		Symbol = symbol;
		IsLeftAssociative = isLeftAssociative;
	}

	/// <summary>
	///     The Precedence of the Operator
	/// </summary>
	public int Precedence { get; }

	/// <summary>
	///     Set to true if the Operator needs to be evaluated left to right
	/// </summary>
	public bool IsLeftAssociative { get; }

	/// <summary>
	///     The Operator Symbol
	/// </summary>
	public string Symbol { get; }
}
