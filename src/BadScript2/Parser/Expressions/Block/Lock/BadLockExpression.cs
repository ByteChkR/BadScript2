using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

/// <summary>
/// Contains the Locking Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions.Block.Lock;

/// <summary>
///     Implements the Lock Expression
/// </summary>
public class BadLockExpression : BadExpression
{
    /// <summary>
    ///     The Expression to lock on
    /// </summary>
    public readonly BadExpression LockExpression;

    /// <summary>
    ///     The Block Body
    /// </summary>
    private readonly BadExpression[] m_Block;

    /// <summary>
    ///     Constructor of the Lock Expression
    /// </summary>
    /// <param name="position">Source Position of the Expression</param>
    /// <param name="lockExpression">The expression to lock on</param>
    /// <param name="block">The Block Body</param>
    public BadLockExpression(BadSourcePosition position,
                             BadExpression lockExpression,
                             BadExpression[] block) : base(false, position)
    {
        LockExpression = lockExpression;
        m_Block = block;
    }

    /// <summary>
    ///     The Block Body
    /// </summary>
    public IEnumerable<BadExpression> Block => m_Block;


    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression expression in LockExpression.GetDescendantsAndSelf())
        {
            yield return expression;
        }

        foreach (BadExpression expression in m_Block)
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
        BadObject lockObj = BadObject.Null;

        foreach (BadObject o in LockExpression.Execute(context))
        {
            lockObj = o;

            yield return o;
        }

        lockObj = lockObj.Dereference();

        if (lockObj is not BadArray && lockObj is not BadTable && lockObj is BadClass)
        {
            throw new BadRuntimeException("Lock object must be of type Array, Object or Class", Position);
        }

        while (!BadLockList.Instance.TryAquire(lockObj))
        {
            yield return BadObject.Null;
        }

        if (m_Block.Length != 0)
        {
            foreach (BadObject o in context.Execute(m_Block))
            {
                yield return o;
            }
        }

        BadLockList.Instance.Release(lockObj);
    }
}