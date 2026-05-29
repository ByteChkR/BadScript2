using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Implements a path in a BadExpression Syntax Tree
/// </summary>
public class BadExpressionPath
{
    /// <summary>
    ///     The Child Paths of this Path
    /// </summary>
    private readonly List<BadExpressionPath> m_ChildPaths = new List<BadExpressionPath>();

    /// <summary>
    ///     The Parent of this Path
    /// </summary>
    public readonly BadExpression Parent;

    /// <summary>
    ///     Indicates whether this Path has a Return Statement
    /// </summary>
    private bool m_HasReturnStatement;

    /// <summary>
    ///     Cached result of IsValid calculation
    /// </summary>
    private bool? m_IsValidCache;

    /// <summary>
    ///     Creates a new Path for the given Parent
    /// </summary>
    /// <param name="parent">The Parent of this Path</param>
    public BadExpressionPath(BadExpression parent)
    {
        Parent = parent;
    }

    /// <summary>
    ///     Indicates whether this Path is valid(e.g. all paths have a return statement)
    /// </summary>
    public bool IsValid
    {
        get
        {
            if (m_IsValidCache == null)
            {
                m_IsValidCache = m_HasReturnStatement || (m_ChildPaths.Count > 0 && m_ChildPaths.All(p => p.IsValid));
            }

            return m_IsValidCache.Value;
        }
    }

    /// <summary>
    ///     Returns all invalid paths in this Path
    /// </summary>
    /// <returns>All invalid paths in this Path</returns>
    public IEnumerable<BadExpressionPath> GetInvalidPaths()
    {
        if (!m_HasReturnStatement && m_ChildPaths.Count == 0)
        {
            yield return this;
        }
        else
        {
            foreach (BadExpressionPath invalidPath in m_ChildPaths.SelectMany(childPath => childPath.GetInvalidPaths()))
            {
                yield return invalidPath;
            }
        }
    }

    /// <summary>
    ///     Adds a Child Path to this Path
    /// </summary>
    /// <param name="path"></param>
    public void AddChildPath(BadExpressionPath path)
    {
        m_ChildPaths.Add(path);
        m_IsValidCache = null; // Invalidate cache
    }

    /// <summary>
    ///     Sets this Path to have a Return Statement
    /// </summary>
    public void SetHasReturnStatement()
    {
        m_HasReturnStatement = true;
        m_ChildPaths.Clear();
        m_IsValidCache = null; // Invalidate cache
    }
}