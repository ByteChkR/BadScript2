using BadScript2.Parser.Expressions;

namespace BadScript2.Optimizations.Substitution;

public class BadConstantSubstitutionOptimizerScope
{
	private readonly Dictionary<string, BadExpression> m_Constants = new Dictionary<string, BadExpression>();
	private readonly BadConstantSubstitutionOptimizerScope? m_Parent;

	public BadConstantSubstitutionOptimizerScope()
	{
		m_Parent = null;
	}

	private BadConstantSubstitutionOptimizerScope(BadConstantSubstitutionOptimizerScope? parent)
	{
		m_Parent = parent;
	}

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

	public bool IsConstant(string name)
	{
		return m_Constants.ContainsKey(name) || (m_Parent != null && m_Parent.IsConstant(name));
	}

	public void AddConstant(string name, BadExpression expr)
	{
		m_Constants.Add(name, expr);
	}

	public BadConstantSubstitutionOptimizerScope CreateChildScope()
	{
		return new BadConstantSubstitutionOptimizerScope(this);
	}
}
