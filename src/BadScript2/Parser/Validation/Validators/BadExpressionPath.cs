using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Validation.Validators;

public class BadExpressionPath
{
	private readonly List<BadExpressionPath> m_ChildPaths = new List<BadExpressionPath>();
	public readonly BadExpression Parent;
	private bool m_HasReturnStatement;

	public BadExpressionPath(BadExpression parent)
	{
		Parent = parent;
	}

	public bool IsValid => m_HasReturnStatement || (m_ChildPaths.Count > 0 && m_ChildPaths.All(p => p.IsValid));

	public IEnumerable<BadExpressionPath> GetInvalidPaths()
	{
		if (!m_HasReturnStatement && m_ChildPaths.Count == 0)
		{
			yield return this;
		}
		else
		{
			foreach (BadExpressionPath childPath in m_ChildPaths)
			{
				foreach (BadExpressionPath invalidPath in childPath.GetInvalidPaths())
				{
					yield return invalidPath;
				}
			}
		}
	}

	public void AddChildPath(BadExpressionPath path)
	{
		m_ChildPaths.Add(path);
	}

	public void SetHasReturnStatement()
	{
		m_HasReturnStatement = true;
		m_ChildPaths.Clear();
	}
}