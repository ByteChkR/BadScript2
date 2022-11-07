using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Access;

/// <summary>
///     Implements the Ternary Expression
///     <Left> ? <TrueRet> : <FalseRet>
/// </summary>
public class BadTernaryExpression : BadExpression
{
    /// <summary>
    ///     Constructor for the Ternary Expression
    /// </summary>
    /// <param name="left">Left side that will be evaluated</param>
    /// <param name="trueRet">Expression that is executed if left evaluates to true</param>
    /// <param name="falseRet">Expression that is executed if left evaluates to false</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadTernaryExpression(
        BadExpression left,
        BadExpression trueRet,
        BadExpression falseRet,
        BadSourcePosition position) : base(left.IsConstant, position)
    {
        Left = left;
        TrueRet = trueRet;
        FalseRet = falseRet;
    }

    /// <summary>
    ///     Expression that is executed if left evaluates to false
    /// </summary>
    public BadExpression FalseRet { get; private set; }

    /// <summary>
    ///     Left side that will be evaluated
    /// </summary>
    public BadExpression Left { get; private set; }

    /// <summary>
    ///     Expression that is executed if left evaluates to true
    /// </summary>
    public BadExpression TrueRet { get; private set; }

    public override void Optimize()
    {
        FalseRet = BadExpressionOptimizer.Optimize(FalseRet);
        Left = BadExpressionOptimizer.Optimize(Left);
        TrueRet = BadExpressionOptimizer.Optimize(TrueRet);
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        foreach (BadObject o in Left.Execute(context))
        {
            left = o;
        }

        left = left.Dereference();

        if (left is not IBadBoolean lBool)
        {
            throw new BadRuntimeException("Ternary operator requires a boolean value on the left side.", Position);
        }

        if (lBool.Value)
        {
            foreach (BadObject o in TrueRet.Execute(context))
            {
                yield return o;
            }
        }
        else
        {
            foreach (BadObject o in FalseRet.Execute(context))
            {
                yield return o;
            }
        }
    }

    public override string ToString()
    {
        return $"({Left} ? {TrueRet} : {FalseRet})";
    }
}