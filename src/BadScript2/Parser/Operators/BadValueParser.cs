using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

public abstract class BadValueParser
{
    public abstract bool IsValue(BadSourceParser parser);
    public abstract BadExpression ParseValue(BadSourceParser parser);
}