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
	public BadTableExpression(Dictionary<BadWordToken, BadExpression> table, BadSourcePosition position) : base(
        false,
        position
    )
    {
        m_Table = table;
    }

    public int Length => m_Table.Count;

    /// <summary>
    ///     The Initializer List of the Table
    /// </summary>
    public IDictionary<BadWordToken, BadExpression> Table => m_Table;

    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (KeyValuePair<BadWordToken, BadExpression> kvp in m_Table)
        {
            foreach (BadExpression expression in kvp.Value.GetDescendantsAndSelf())
            {
                yield return expression;
            }
        }
    }

    public override void Optimize()
    {
        KeyValuePair<BadWordToken, BadExpression>[] branches = m_Table.ToArray();
        m_Table.Clear();

        foreach (KeyValuePair<BadWordToken, BadExpression> branch in branches)
        {
            m_Table[branch.Key] = BadConstantFoldingOptimizer.Optimize(branch.Value);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        Dictionary<BadObject, BadObject> table = new Dictionary<BadObject, BadObject>();

        foreach (KeyValuePair<BadWordToken, BadExpression> entry in m_Table)
        {
            BadObject key = BadObject.Wrap(entry.Key.Text);

            BadObject value = BadObject.Null;

            foreach (BadObject o in entry.Value.Execute(context))
            {
                value = o;

                yield return o;
            }

            value = value.Dereference();

            table[key] = value;
        }

        yield return new BadTable(table);
    }
}