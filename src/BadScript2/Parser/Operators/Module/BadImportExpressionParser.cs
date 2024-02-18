using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Module;
using BadScript2.Reader;
using BadScript2.Reader.Token.Primitive;

namespace BadScript2.Parser.Operators.Module;

/// <summary>
/// Parses the Import Expression
/// </summary>
public class BadImportExpressionParser : BadValueParser
{
    /// <inheritdoc cref="BadValueParser.IsValue" />
    public override bool IsValue(BadSourceParser parser)
    {
        return parser.Reader.IsKey(BadStaticKeys.IMPORT_KEY);
    }

    /// <inheritdoc cref="BadValueParser.ParseValue" />
    public override BadExpression ParseValue(BadSourceParser parser)
    {
        BadSourcePosition pos = parser.Reader.Eat(BadStaticKeys.IMPORT_KEY);
        parser.Reader.SkipNonToken();
        string name = parser.Reader.ParseWord().Text;
        parser.Reader.SkipNonToken();
        parser.Reader.Eat(BadStaticKeys.FROM_KEY);
        parser.Reader.SkipNonToken();
        BadStringToken pathResult = parser.Reader.ParseString();
        string path = pathResult.Value.Substring(1, pathResult.Value.Length - 2);

        return new BadImportExpression(name, path, pos.Combine(pathResult.SourcePosition));
    }
}