using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains the Variable Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions.Variables;

/// <summary>
///     Implements the Formattted String Expression
/// </summary>
public class BadFormattedStringExpression : BadStringExpression
{
    /// <summary>
    ///     The Expressions that will be evaluated during the creation of the formatted string
    /// </summary>
    private readonly BadExpression[] m_Expressions;

    /// <summary>
    ///     Constructor of the Formatted String Expression
    /// </summary>
    /// <param name="exprs">The Expressions that will be evaluated during the creation of the formatted string</param>
    /// <param name="str">The Format String</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadFormattedStringExpression(BadExpression[] exprs, string str, BadSourcePosition position) : base(str,
                                                                                                              position
                                                                                                             )
    {
        m_Expressions = exprs;
    }

    /// <summary>
    ///     The Expressions that will be evaluated during the creation of the formatted string
    /// </summary>
    public IEnumerable<BadExpression> Expressions => m_Expressions;

    /// <summary>
    ///     The Expression count of the Format String
    /// </summary>
    public int ExpressionCount => m_Expressions.Length;

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        for (int i = 0; i < m_Expressions.Length; i++)
        {
            m_Expressions[i] = BadConstantFoldingOptimizer.Optimize(m_Expressions[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        if (m_Expressions.Length == 0)
        {
            yield return Value;

            yield break;
        }

        List<BadObject> objs = new List<BadObject>();

        foreach (BadExpression expr in m_Expressions)
        {
            BadObject obj = BadObject.Null;

            foreach (BadObject o in expr.Execute(context))
            {
                obj = o;

                yield return o;
            }

            objs.Add(obj.Dereference(Position));
        }

        yield return string.Format(Value,
                                   objs.Cast<object?>()
                                       .ToArray()
                                  );
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression expr in m_Expressions)
        {
            foreach (BadExpression e in expr.GetDescendantsAndSelf())
            {
                yield return e;
            }
        }

        foreach (BadExpression e in base.GetDescendants())
        {
            yield return e;
        }
    }
}