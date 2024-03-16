using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
namespace BadScript2.Parser.Expressions.Block;

/// <summary>
/// Implements the Switch Statement Expression
/// </summary>
public class BadSwitchExpression : BadExpression
{

    /// <summary>
    /// The Value to switch on
    /// </summary>
    public BadExpression Value { get; }
    /// <summary>
    /// The Cases
    /// </summary>
    private readonly Dictionary<BadExpression, BadExpression[]> m_Cases;
    /// <summary>
    /// The (optional) Default Case
    /// </summary>
    private List<BadExpression>? m_DefaultCase;
    /// <summary>
    ///     The Cases
    /// </summary>
    public IDictionary<BadExpression, BadExpression[]> Cases => m_Cases;

    /// <summary>
    ///     The (optional) Default Case
    /// </summary>
    public IEnumerable<BadExpression>? DefaultCase => m_DefaultCase;
    
    /// <summary>
    /// Constructs a new BadSwitchExpression
    /// </summary>
    /// <param name="position">The Source Position of the Expression</param>
    /// <param name="value">The Value to switch on</param>
    /// <param name="cases">The Cases</param>
    /// <param name="defaultCase">The (optional) Default Case</param>
    public BadSwitchExpression(BadSourcePosition position, BadExpression value, Dictionary<BadExpression, BadExpression[]> cases, List<BadExpression>? defaultCase) : base(false, position)
    {
        Value = value;
        m_Cases = cases;
        m_DefaultCase = defaultCase;
    }
    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        yield return Value;
        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in m_Cases)
        {
            yield return branch.Key;
            foreach (BadExpression valueExpr in branch.Value)
            {
                yield return valueExpr;
            }
        }

        if (m_DefaultCase == null)
        {
            yield break;
        }

        foreach (BadExpression e in m_DefaultCase)
        {
            yield return e;
        }
    }

    public override void Optimize()
    {
        KeyValuePair<BadExpression, BadExpression[]>[] branches = m_Cases.ToArray();
        m_Cases.Clear();

        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in branches)
        {
            m_Cases[BadConstantFoldingOptimizer.Optimize(branch.Key)] =
                BadConstantFoldingOptimizer.Optimize(branch.Value).ToArray();
        }

        if (m_DefaultCase == null)
        {
            return;
        }

        for (int i = 0; i < m_DefaultCase.Count; i++)
        {
            m_DefaultCase[i] = BadConstantFoldingOptimizer.Optimize(m_DefaultCase[i]);
        }
    }
    
    /// <summary>
    ///     Sets the Default Case
    /// </summary>
    /// <param name="defaultCase">The new Default Case</param>
    public void SetDefaultCase(IEnumerable<BadExpression>? defaultCase)
    {
        if (defaultCase == null)
        {
            m_DefaultCase = null;
        }
        else if (m_DefaultCase != null)
        {
            m_DefaultCase.Clear();
            m_DefaultCase.AddRange(defaultCase);
        }
        else
        {
            m_DefaultCase = new List<BadExpression>(defaultCase);
        }
    }
    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject valueResult = BadObject.Null;
        foreach (BadObject o in context.Execute(Value))
        {
            valueResult = o;
        }
        if (context.Scope.IsError)
        {
            yield break;
        }
        valueResult = valueResult.Dereference();

        var switchContext = new BadExecutionContext(context.Scope.CreateChild("SwitchContext", context.Scope, null, BadScopeFlags.Breakable));
        bool executeNextBlock = false;
        foreach (KeyValuePair<BadExpression,BadExpression[]> branch in m_Cases)
        {
            if (!executeNextBlock)
            {
                BadObject keyResult = BadObject.Null;
                foreach (BadObject o in switchContext.Execute(branch.Key))
                {
                    keyResult = o;
                }
            
                if (switchContext.Scope.IsError)
                {
                    yield break;
                }
            
                keyResult = keyResult.Dereference();
                var result = BadObject.Null;
                foreach (BadObject o in BadEqualityExpression.EqualWithOverride(switchContext, valueResult, keyResult, Position))
                {
                    result = o;
                }
            
                if (switchContext.Scope.IsError)
                {
                    yield break;
                }
            
                result = result.Dereference();
                if (result is not IBadBoolean b)
                {
                    throw new BadRuntimeException("Switch case result must be a boolean", Position);
                }
                executeNextBlock = b.Value;
            }
            if (executeNextBlock && branch.Value.Length > 0)
            {
                foreach (BadObject o in switchContext.Execute(branch.Value))
                {
                    yield return o;
                    if(switchContext.Scope.IsBreak)
                    {
                        yield break;
                    }
                }
                throw BadRuntimeException.Create(switchContext.Scope,"Switch Case must break", Position);
            }
        }
        if (m_DefaultCase != null)
        {
            foreach (BadObject o in switchContext.Execute(m_DefaultCase))
            {
                yield return o;
                if(switchContext.Scope.IsBreak)
                {
                    yield break;
                }
            }
            throw BadRuntimeException.Create(switchContext.Scope,"Switch Case must break", Position);
        }
    }
}