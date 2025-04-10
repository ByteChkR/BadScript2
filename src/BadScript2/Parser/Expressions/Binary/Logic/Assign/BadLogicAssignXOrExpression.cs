using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Logic.Assign;

/// <summary>
///     Implements the Assign Logic Exclusive Or Expression
/// </summary>
public class BadLogicAssignXOrExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Assign Logic Exclusive Or Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadLogicAssignXOrExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
                                                                                                                   right,
                                                                                                                   position
                                                                                                                  ) { }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        bool hasReturn = false;
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        if (left is not BadObjectReference leftRef)
        {
            throw new BadRuntimeException($"Left side of {GetSymbol()} must be a reference", Position);
        }

        left = left.Dereference(Position);

        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference(Position);

        if (left is IBadBoolean lBool && right is IBadBoolean rBool)
        {
            hasReturn = true;

            BadObject r = BadObject.Wrap(lBool.Value ^ rBool.Value);
            leftRef.Set(r, Position);

            yield return r;
        }

        if (!hasReturn)
        {
            throw new BadRuntimeException($"Can not apply operator '{GetSymbol()}' to {left} and {right}", Position);
        }
    }

    /// <inheritdoc cref="BadBinaryExpression.GetSymbol" />
    protected override string GetSymbol()
    {
        return "^=";
    }
}