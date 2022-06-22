using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant
{
    public class BadTableExpression : BadExpression
    {
        public readonly Dictionary<BadWordToken, BadExpression> Table;

        public BadTableExpression(Dictionary<BadWordToken, BadExpression> table, BadSourcePosition position) : base(
            false,
            false,
            position
        )
        {
            Table = table;
        }

        public override void Optimize()
        {
            KeyValuePair<BadWordToken, BadExpression>[] branches = Table.ToArray();
            Table.Clear();
            foreach (KeyValuePair<BadWordToken, BadExpression> branch in branches)
            {
                Table[branch.Key] = BadExpressionOptimizer.Optimize(branch.Value);
            }
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            Dictionary<BadObject, BadObject> table = new Dictionary<BadObject, BadObject>();
            foreach (KeyValuePair<BadWordToken, BadExpression> entry in Table)
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
}