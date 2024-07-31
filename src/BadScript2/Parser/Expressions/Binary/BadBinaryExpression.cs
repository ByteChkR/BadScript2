using BadScript2.Common;
using BadScript2.Optimizations.Folding;

namespace BadScript2.Parser.Expressions.Binary;

/// <summary>
///     Base Implementation of all Binary Expressions
/// </summary>
public abstract class BadBinaryExpression : BadExpression
{
    /// <summary>
    ///     Constructor for Binary Expressions
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source position of the Expression</param>
    protected BadBinaryExpression(BadExpression left, BadExpression right, BadSourcePosition position) :
        base(left.IsConstant && right.IsConstant,
             position
            )
    {
        Left = left;
        Right = right;
    }

    /// <summary>
    ///     Left side of the Expression
    /// </summary>
    public BadExpression Left { get; private set; }

    /// <summary>
    ///     Right side of the Expression
    /// </summary>
    public BadExpression Right { get; private set; }

    /// <inheritdoc cref="!:BadObject.Optimize" />
    public override void Optimize()
    {
        Left = BadConstantFoldingOptimizer.Optimize(Left);
        Right = BadConstantFoldingOptimizer.Optimize(Right);
    }

    /// <summary>
    ///     Sets the Left side of the Expression
    /// </summary>
    /// <param name="expr">The new Left side of the Expression</param>
    public void SetLeft(BadExpression expr)
    {
        Left = expr;
    }

    /// <summary>
    ///     Sets the Right side of the Expression
    /// </summary>
    /// <param name="expr">The new Right side of the Expression</param>
    public void SetRight(BadExpression expr)
    {
        Right = expr;
    }

    /// <summary>
    ///     Returns the Symbol of the Operator
    /// </summary>
    /// <returns>The Symbol of the Operator</returns>
    protected abstract string GetSymbol();

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression expression in Left.GetDescendantsAndSelf())
        {
            yield return expression;
        }

        foreach (BadExpression? expression in Right.GetDescendantsAndSelf())
        {
            yield return expression;
        }
    }

    /// <summary>
    ///     The String Representation of the Expression
    /// </summary>
    /// <returns>String Representation of the Binary Expression</returns>
    public override string ToString()
    {
        return $"({Left} {GetSymbol()} {Right})";
    }
}