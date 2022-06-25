using BadScript2.Parser.Operators.Binary;
using BadScript2.Parser.Operators.Binary.Comparison;
using BadScript2.Parser.Operators.Binary.Logic;
using BadScript2.Parser.Operators.Binary.Logic.Assign;
using BadScript2.Parser.Operators.Binary.Math;
using BadScript2.Parser.Operators.Binary.Math.Assign;
using BadScript2.Parser.Operators.Binary.Math.Atomic;

namespace BadScript2.Parser.Operators;

public class BadOperatorTable
{
    private readonly List<BadBinaryOperator> m_Operators = new List<BadBinaryOperator>
    {
        new BadPostIncrementOperator(),
        new BadPostDecrementOperator(),
        new BadAddAssignOperator(),
        new BadModulusAssignOperator(),
        new BadSubtractAssignOperator(),
        new BadMultiplyAssignOperator(),
        new BadDivideAssignOperator(),
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
    };

    private readonly List<BadUnaryPrefixOperator> m_UnaryPrefixOperators = new List<BadUnaryPrefixOperator>
    {
        new BadLogicNotOperator(),
        new BadPreDecrementOperator(),
        new BadPreIncrementOperator(),
    };

    private readonly List<BadValueParser> m_ValueParsers = new List<BadValueParser>();
    private BadOperatorTable() { }

    public static BadOperatorTable Instance { get; } = new BadOperatorTable();

    public IEnumerable<string> BinarySymbols => m_Operators.Select(x => x.Symbol);

    public IEnumerable<string> UnaryPrefixSymbols => m_UnaryPrefixOperators.Select(x => x.Symbol);

    public BadValueParser? GetValueParser(BadSourceParser parser)
    {
        return m_ValueParsers.FirstOrDefault(x => x.IsValue(parser));
    }

    public void AddValueParser(BadValueParser parser)
    {
        m_ValueParsers.Add(parser);
    }

    public void AddOperator(BadBinaryOperator op)
    {
        m_Operators.Add(op);
    }

    public void AddUnaryPrefixOperator(BadUnaryPrefixOperator op)
    {
        m_UnaryPrefixOperators.Add(op);
    }

    public BadBinaryOperator? FindBinaryOperator(string symbol)
    {
        return m_Operators.FirstOrDefault(x => x.Symbol == symbol);
    }

    public BadUnaryPrefixOperator? FindUnaryPrefixOperator(string symbol)
    {
        return m_UnaryPrefixOperators.FirstOrDefault(x => x.Symbol == symbol);
    }
}