using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains the Constant Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions.Constant;

/// <summary>
///     Implements the Array Expression
/// </summary>
public class BadArrayExpression : BadExpression
{
    /// <summary>
    ///     The Initializer List
    /// </summary>
    private readonly BadExpression[] m_InitExpressions;

    /// <summary>
    ///     Constructor of the Array Expression
    /// </summary>
    /// <param name="initExpressions">The initializer list of the Array</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadArrayExpression(BadExpression[] initExpressions, BadSourcePosition position) : base(
        false,
        position
    )
    {
        m_InitExpressions = initExpressions;
    }

    /// <summary>
    ///     Length of the Initializer List
    /// </summary>
    public int Length => m_InitExpressions.Length;

    /// <summary>
    ///     The Initializer List
    /// </summary>
    public IEnumerable<BadExpression> InitExpressions => m_InitExpressions;

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        for (int i = 0; i < m_InitExpressions.Length; i++)
        {
            m_InitExpressions[i] = BadConstantFoldingOptimizer.Optimize(m_InitExpressions[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        List<BadObject> array = new List<BadObject>();

        foreach (BadExpression expression in m_InitExpressions)
        {
            BadObject o = BadObject.Null;

            foreach (BadObject obj in expression.Execute(context))
            {
                o = obj;

                yield return o;
            }

            array.Add(o.Dereference());
        }

        yield return new BadArray(array);
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return m_InitExpressions.SelectMany(expression => expression.GetDescendantsAndSelf());
    }
}