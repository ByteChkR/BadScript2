using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Module;
using BadScript2.Reader;
using BadScript2.Runtime.Error;

namespace BadScript2.Parser.Operators.Module;

/// <summary>
///     Parses the Export Expression
/// </summary>
public class BadExportExpressionParser : BadValueParser
{
    /// <inheritdoc cref="BadValueParser.IsValue" />
    public override bool IsValue(BadSourceParser parser)
    {
        return parser.Reader.IsKey(BadStaticKeys.EXPORT_KEY);
    }

    /// <summary>
    ///     Returns true if the specified expression is a named expression
    /// </summary>
    /// <param name="expr">The Expression</param>
    /// <param name="name">The Name of the expression</param>
    /// <returns>true if the expression is a named expression</returns>
    public static bool IsNamed(BadExpression expr, out string? name)
    {
        if (expr is IBadNamedExpression namedExpr)
        {
            name = namedExpr.GetName();

            return true;
        }

        if (expr is BadAssignExpression assign)
        {
            if (assign.Left is IBadNamedExpression named)
            {
                name = named.GetName();

                return true;
            }
        }

        name = null;

        return false;
    }

    /// <inheritdoc cref="BadValueParser.ParseValue" />
    public override BadExpression ParseValue(BadSourceParser parser)
    {
        BadSourcePosition pos = parser.Reader.Eat(BadStaticKeys.EXPORT_KEY);
        parser.Reader.SkipNonToken();
        bool isDefault = parser.Reader.IsKey(BadStaticKeys.DEFAULT_KEY);
        if (isDefault)
        {
            parser.Reader.Eat(BadStaticKeys.DEFAULT_KEY);
            parser.Reader.SkipNonToken();
        }

        BadExpression expr = parser.ParseExpression();

        bool isNamed = IsNamed(expr, out string? name);

        if (!isDefault && isNamed)
        {
            return new BadNamedExportExpression(name, expr, pos.Combine(expr.Position));
        }

        if (isDefault && !isNamed)
        {
            throw new BadRuntimeException("Default Export must be a named expression", pos);
        }

        return new BadDefaultExportExpression(expr, pos.Combine(expr.Position));
    }
}