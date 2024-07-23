using BadScript2.Parser.Operators.Binary;
using BadScript2.Parser.Operators.Binary.Comparison;
using BadScript2.Parser.Operators.Binary.Logic;
using BadScript2.Parser.Operators.Binary.Logic.Assign;
using BadScript2.Parser.Operators.Binary.Math;
using BadScript2.Parser.Operators.Binary.Math.Assign;
using BadScript2.Parser.Operators.Binary.Math.Atomic;
using BadScript2.Parser.Operators.Module;
namespace BadScript2.Parser.Operators;

/// <summary>
///     Implements the Operator Table used by the Parser
/// </summary>
public class BadOperatorTable
{
	/// <summary>
	///     List of Binary operators
	/// </summary>
	private readonly List<BadBinaryOperator> m_Operators = new List<BadBinaryOperator>
    {
        new BadPostIncrementOperator(),
        new BadPostDecrementOperator(),
        new BadAddAssignOperator(),
        new BadModulusAssignOperator(),
        new BadSubtractAssignOperator(),
        new BadExponentiationAssignOperator(),
        new BadMultiplyAssignOperator(),
        new BadDivideAssignOperator(),
        new BadBinaryUnpackOperator(),
        new BadRangeOperator(),
        new BadMemberAccessOperator(),
        new BadEqualityOperator(),
        new BadInequalityOperator(),
        new BadLessOrEqualOperator(),
        new BadLessThanOperator(),
        new BadGreaterOrEqualOperator(),
        new BadGreaterThanOperator(),
        new BadAddOperator(),
        new BadModulusOperator(),
        new BadSubtractOperator(),
        new BadExponentiationOperator(),
        new BadMultiplyOperator(),
        new BadDivideOperator(),
        new BadAssignOperator(),
        new BadLogicAssignAndOperator(),
        new BadLogicAssignOrOperator(),
        new BadLogicAssignXOrOperator(),
        new BadLogicAndOperator(),
        new BadLogicOrOperator(),
        new BadLogicXOrOperator(),
        new BadNullCheckedMemberAccessOperator(),
        new BadNullCoalescingAssignOperator(),
        new BadNullCoalescingOperator(),
        new BadTernaryOperator(),
        new BadInstanceOfExpressionOperator(),
        new BadInOperator(),
    };

	/// <summary>
	///     List of Unary Prefix Operators
	/// </summary>
	private readonly List<BadUnaryPrefixOperator> m_UnaryPrefixOperators = new List<BadUnaryPrefixOperator>
    {
        new BadLogicNotOperator(),
        new BadPreDecrementOperator(),
        new BadPreIncrementOperator(),
        new BadUnaryUnpackOperator(),
        new BadNegateOperator(),
    };

	/// <summary>
	///     List of Value Parsers
	/// </summary>
	private readonly List<BadValueParser> m_ValueParsers = new List<BadValueParser>
    {
        new BadDeleteExpressionParser(),
        new BadTypeOfExpressionParser(),
        new BadExportExpressionParser(),
        new BadImportExpressionParser(),
    };

	/// <summary>
	///     Private Constructor
	/// </summary>
	private BadOperatorTable() { }

	/// <summary>
	///     The Operator Table Instance.
	/// </summary>
	public static BadOperatorTable Instance { get; } = new BadOperatorTable();

	/// <summary>
	///     Enumeration of all Binary Operator Symbols
	/// </summary>
	public IEnumerable<string> BinarySymbols => m_Operators.Select(x => x.Symbol);

	/// <summary>
	///     Enumeration of all Unary Prefix Operator Symbols
	/// </summary>
	public IEnumerable<string> UnaryPrefixSymbols => m_UnaryPrefixOperators.Select(x => x.Symbol);

	/// <summary>
	///     Returns a Value Parser that is able to parse the given Token
	/// </summary>
	/// <param name="parser">The Parser Instance</param>
	/// <returns>The Value Parser</returns>
	public BadValueParser? GetValueParser(BadSourceParser parser)
    {
        return m_ValueParsers.FirstOrDefault(x => x.IsValue(parser));
    }

	/// <summary>
	///     Adds a Value parser to the List of Value Parsers
	/// </summary>
	/// <param name="parser">The Parser to be added</param>
	public void AddValueParser(BadValueParser parser)
    {
        m_ValueParsers.Add(parser);
    }

	/// <summary>
	///     Adds a Binary Operator Parser to the List of Binary Operators
	/// </summary>
	/// <param name="op">The Operator to be Added</param>
	public void AddOperator(BadBinaryOperator op)
    {
        m_Operators.Add(op);
    }

	/// <summary>
	///     Adds a Unary Prefix Operator Parser to the List of Unary Prefix Operators
	/// </summary>
	/// <param name="op">The Operator to be Added</param>
	public void AddUnaryPrefixOperator(BadUnaryPrefixOperator op)
    {
        m_UnaryPrefixOperators.Add(op);
    }

	/// <summary>
	///     Finds a Binary Operator by its Symbol
	/// </summary>
	/// <param name="symbol">The Symbol of the Parser</param>
	/// <param name="precedence">The Maximum Precedence</param>
	/// <returns>The Operator that was found. Null if none were found.</returns>
	public BadBinaryOperator? FindBinaryOperator(string symbol, int precedence)
    {
        return m_Operators.FirstOrDefault(
            op => op.Symbol == symbol &&
                  (op.IsLeftAssociative && op.Precedence < precedence ||
                   !op.IsLeftAssociative && op.Precedence <= precedence)
        );
    }

	/// <summary>
	///     Finds a Unary Prefix Operator by its Symbol
	/// </summary>
	/// <param name="symbol">Symbol of the Operator</param>
	/// <param name="precedence">The Minimum Operator precedence</param>
	/// <returns>The Operator that was found. Null if none were found.</returns>
	public BadUnaryPrefixOperator? FindUnaryPrefixOperator(string symbol, int precedence)
    {
        return m_UnaryPrefixOperators.FirstOrDefault(
            op => op.Symbol == symbol &&
                  (op.IsLeftAssociative && op.Precedence < precedence ||
                   !op.IsLeftAssociative && op.Precedence <= precedence)
        );
    }
}