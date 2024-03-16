using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Block;

/// <summary>
///     Implements the Try Catch Statement Expression
/// </summary>
public class BadTryCatchExpression : BadExpression
{
    /// <summary>
    ///     The Variable name of the Exception inside the catch block
    /// </summary>
    public readonly string ErrorName;

    /// <summary>
    ///     The Catch Block
    /// </summary>
    private readonly BadExpression[] m_CatchExpressions;

    /// <summary>
    ///     The Try Block
    /// </summary>
    private readonly BadExpression[] m_Expressions;

    /// <summary>
    ///     The Finally Block
    /// </summary>
    private readonly BadExpression[] m_FinallyExpressions;

    /// <summary>
    ///     Constructor for the Try Catch Expression
    /// </summary>
    /// <param name="position">Source position of the Expression</param>
    /// <param name="expressions">The Try Block</param>
    /// <param name="catchExpressions">The Catch Block</param>
    /// <param name="errorName">The Variable name of the Exception inside the Catch block</param>
    public BadTryCatchExpression(
        BadSourcePosition position,
        BadExpression[] expressions,
        BadExpression[] catchExpressions,
        BadExpression[] finallyExpressions,
        string errorName) : base(false, position)
    {
        m_Expressions = expressions;
        m_CatchExpressions = catchExpressions;
        ErrorName = errorName;
        m_FinallyExpressions = finallyExpressions;
    }

    /// <summary>
    ///     The Catch Block
    /// </summary>
    public IEnumerable<BadExpression> CatchExpressions => m_CatchExpressions;

    /// <summary>
    ///     The Try Block
    /// </summary>
    public IEnumerable<BadExpression> TryExpressions => m_Expressions;

    /// <summary>
    ///     The Finally Block
    /// </summary>
    public IEnumerable<BadExpression> FinallyExpressions => m_FinallyExpressions;

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        for (int i = 0; i < m_CatchExpressions.Length; i++)
        {
            m_CatchExpressions[i] = BadConstantFoldingOptimizer.Optimize(m_CatchExpressions[i]);
        }

        for (int i = 0; i < m_Expressions.Length; i++)
        {
            m_Expressions[i] = BadConstantFoldingOptimizer.Optimize(m_Expressions[i]);
        }

        for (int i = 0; i < m_FinallyExpressions.Length; i++)
        {
            m_FinallyExpressions[i] = BadConstantFoldingOptimizer.Optimize(m_FinallyExpressions[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression expression in m_Expressions)
        {
            foreach (BadExpression e in expression.GetDescendantsAndSelf())
            {
                yield return e;
            }
        }

        foreach (BadExpression expression in m_CatchExpressions)
        {
            foreach (BadExpression e in expression.GetDescendantsAndSelf())
            {
                yield return e;
            }
        }

        foreach (BadExpression expression in m_FinallyExpressions)
        {
            foreach (BadExpression e in expression.GetDescendantsAndSelf())
            {
                yield return e;
            }
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        if (m_Expressions.Length != 0) //If the expressions are empty, then we cant possibly throw an error, nor do we need to catch one
        {
            IEnumerable<BadObject>? catchEnumerable = null;
            using (BadExecutionContext tryContext = new BadExecutionContext(
                       context.Scope.CreateChild("TryBlock", context.Scope, null, BadScopeFlags.CaptureThrow)
                   ))
            {
                using var enumerator = tryContext.Execute(m_Expressions).GetEnumerator();
                while (true)
                {
                    BadObject o;
                    
                    try {
                        if (!enumerator.MoveNext())
                            break;
                        o = enumerator.Current ?? BadObject.Null;
                    }
                    catch (Exception e)
                    {
                        if(m_CatchExpressions.Length != 0)
                        {
                            using BadExecutionContext catchContext = new BadExecutionContext(
                                context.Scope.CreateChild("CatchBlock", context.Scope, null)
                            );
                            
                            
                            BadRuntimeError error;
                            if (e is BadRuntimeErrorException bre) error = bre.Error;
                            else error = BadRuntimeError.FromException(e, context.Scope.GetStackTrace());
                            catchContext.Scope.DefineVariable(ErrorName, error);

                            catchEnumerable = catchContext.Execute(m_CatchExpressions);
                        }
                        break;
                    }
                    yield return o;
                }
            }

            if (catchEnumerable != null)
            {
                foreach (BadObject e in catchEnumerable)
                {
                    yield return e;
                }
            }
        }

        if (m_FinallyExpressions.Length != 0)
        {
            using BadExecutionContext finallyContext = new BadExecutionContext(
                context.Scope.CreateChild("FinallyBlock", context.Scope, null)
            );

            foreach (BadObject e in finallyContext.Execute(m_FinallyExpressions))
            {
                yield return e;
            }
        }
    }
}