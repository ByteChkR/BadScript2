using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math;

/// <summary>
///     Implements the Divide Expression
/// </summary>
public class BadDivideExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Divide Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadDivideExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
                                                                                                           right,
                                                                                                           position
                                                                                                          ) { }

    /// <inheritdoc cref="BadBinaryExpression.GetSymbol" />
    protected override string GetSymbol()
    {
        return "/";
    }

    /// <summary>
    ///     Divides left by right
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="pos">Source position that is used to generate an Exception if left or right are not a number</param>
    /// <returns>The result of the division of left by right</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side are not inheriting from IBadNumber</exception>
    public static BadObject Div(BadObject left, BadObject right, BadSourcePosition pos)
    {
        if (left is not IBadNumber lNum)
        {
            throw new BadRuntimeException($"Can not apply operator '/' to {left} and {right}", pos);
        }

        if (right is IBadNumber rNum)
        {
            return BadObject.Wrap(lNum.Value / rNum.Value);
        }

        throw new BadRuntimeException($"Can not apply operator '/' to {left} and {right}", pos);
    }

    /// <summary>
    ///     Executes the Operator with operator overrides
    /// </summary>
    /// <param name="context">The current Script Execution Context</param>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source position that is used to generate an Exception if the operator can not be computed</param>
    /// <returns>The result of the operator</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the operator can not be executed</exception>
    public static IEnumerable<BadObject> DivWithOverride(BadExecutionContext? context,
                                                         BadObject left,
                                                         BadObject right,
                                                         BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.DIVIDE_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(left,
                                                            right,
                                                            context!,
                                                            BadStaticKeys.DIVIDE_OPERATOR_NAME,
                                                            position
                                                           ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Div(left, right, position);
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference(Position);
        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference(Position);

        foreach (BadObject? o in DivWithOverride(context, left, right, Position))
        {
            yield return o;
        }
    }
}