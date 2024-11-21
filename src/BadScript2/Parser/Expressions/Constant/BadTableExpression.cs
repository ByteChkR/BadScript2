using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant;

/// <summary>
///     Implements the Table Expression
/// </summary>
public class BadTableExpression : BadExpression
{
    /// <summary>
    ///     The Initializer List of the Table
    /// </summary>
    private readonly Dictionary<BadWordToken, BadExpression> m_Table;

    /// <summary>
    ///     Constructor of the Table Expression
    /// </summary>
    /// <param name="table">The Initializer List</param>
    /// <param name="position">The Source Position of the Expression</param>
    public BadTableExpression(Dictionary<BadWordToken, BadExpression> table, BadSourcePosition position) : base(false,
                                                                                                                position
                                                                                                               )
    {
        m_Table = table;
    }

    /// <summary>
    ///     The Length of the initializer list
    /// </summary>
    public int Length => m_Table.Count;

    /// <summary>
    ///     The Initializer List of the Table
    /// </summary>
    public IDictionary<BadWordToken, BadExpression> Table => m_Table;

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return m_Table.SelectMany(kvp => kvp.Value.GetDescendantsAndSelf());
    }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        KeyValuePair<BadWordToken, BadExpression>[] branches = m_Table.ToArray();
        m_Table.Clear();

        foreach (KeyValuePair<BadWordToken, BadExpression> branch in branches)
        {
            m_Table[branch.Key] = BadConstantFoldingOptimizer.Optimize(branch.Value);
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        Dictionary<string, BadObject> table = new Dictionary<string, BadObject>();

        foreach (KeyValuePair<BadWordToken, BadExpression> entry in m_Table)
        {
            BadObject value = BadObject.Null;

            foreach (BadObject o in entry.Value.Execute(context))
            {
                value = o;

                yield return o;
            }

            value = value.Dereference(Position);

            table[entry.Key.Text] = value;
        }

        yield return new BadTable(table);
    }
}