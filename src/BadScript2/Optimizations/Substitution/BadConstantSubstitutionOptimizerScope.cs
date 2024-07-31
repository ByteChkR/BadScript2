using BadScript2.Parser.Expressions;

namespace BadScript2.Optimizations.Substitution;

/// <summary>
///     The scope of the Constant Substitution Optimizer
/// </summary>
public class BadConstantSubstitutionOptimizerScope
{
    /// <summary>
    ///     The constants in this scope
    /// </summary>
    private readonly Dictionary<string, BadExpression> m_Constants = new Dictionary<string, BadExpression>();

    /// <summary>
    ///     The parent scope
    /// </summary>
    private readonly BadConstantSubstitutionOptimizerScope? m_Parent;

    /// <summary>
    ///     Creates a new scope
    /// </summary>
    public BadConstantSubstitutionOptimizerScope()
    {
        m_Parent = null;
    }

    /// <summary>
    ///     Creates a new scope with a parent scope
    /// </summary>
    /// <param name="parent">The parent scope</param>
    private BadConstantSubstitutionOptimizerScope(BadConstantSubstitutionOptimizerScope? parent)
    {
        m_Parent = parent;
    }

    /// <summary>
    ///     Returns a constant from this scope or a parent scope
    /// </summary>
    /// <param name="name">The name of the constant</param>
    /// <returns>The constant</returns>
    /// <exception cref="Exception">Thrown if the constant is not found</exception>
    public BadExpression GetConstant(string name)
    {
        if (m_Constants.TryGetValue(name, out BadExpression? value))
        {
            return value;
        }

        if (m_Parent != null && m_Parent.IsConstant(name))
        {
            return m_Parent.GetConstant(name);
        }

        throw new Exception("Constant not found");
    }

    /// <summary>
    ///     Returns true if the constant is defined in this scope or a parent scope
    /// </summary>
    /// <param name="name">The name of the constant</param>
    /// <returns>True if the constant is defined in this scope or a parent scope</returns>
    public bool IsConstant(string name)
    {
        return m_Constants.ContainsKey(name) || (m_Parent != null && m_Parent.IsConstant(name));
    }

    /// <summary>
    ///     Adds a constant to this scope
    /// </summary>
    /// <param name="name">The name of the constant</param>
    /// <param name="expr">The expression of the constant</param>
    public void AddConstant(string name, BadExpression expr)
    {
        m_Constants.Add(name, expr);
    }

    /// <summary>
    ///     Creates a child scope
    /// </summary>
    /// <returns>The child scope</returns>
    public BadConstantSubstitutionOptimizerScope CreateChildScope()
    {
        return new BadConstantSubstitutionOptimizerScope(this);
    }
}