using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Reader;
namespace BadScript2.Parser.Operators;

/// <summary>
///     Implements the Delete Expression Parser
/// </summary>
public class BadDeleteExpressionParser : BadValueParser
{
    /// <inheritdoc cref="BadValueParser.IsValue" />
    public override bool IsValue(BadSourceParser parser)
    {
        return parser.Reader.IsKey(BadStaticKeys.DELETE_KEY);
    }

    /// <inheritdoc cref="BadValueParser.ParseValue" />
    public override BadExpression ParseValue(BadSourceParser parser)
    {
        BadSourcePosition pos = parser.Reader.Eat(BadStaticKeys.DELETE_KEY);
        BadExpression expr = parser.ParseExpression(null, 3);

        return new BadDeleteExpression(expr, pos.Combine(expr.Position));
    }
}