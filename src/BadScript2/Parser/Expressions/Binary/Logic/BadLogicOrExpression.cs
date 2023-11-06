using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Logic;

/// <summary>
///     Implements the Logic Or Expression
/// </summary>
public class BadLogicOrExpression : BadBinaryExpression
{
	/// <summary>
	///     Constructor of the Logic Or Expression
	/// </summary>
	/// <param name="left">Left side of the Expression</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">Source Position of the Expression</param>
	public BadLogicOrExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();

        if (left is IBadBoolean lBool)
        {
            if (lBool.Value)
            {
                yield return BadObject.True;

                yield break;
            }

            BadObject right = BadObject.Null;

            foreach (BadObject o in Right.Execute(context))
            {
                right = o;

                yield return o;
            }

            right = right.Dereference();

            if (right is IBadBoolean rBool)
            {
                if (rBool.Value)
                {
                    yield return BadObject.True;
                }
                else
                {
                    yield return BadObject.False;
                }

                yield break;
            }

            throw new BadRuntimeException($"Can not apply operator '{GetSymbol()}' to {left} and {right}", Position);
        }

        throw new BadRuntimeException($"Can not apply operator '{GetSymbol()}' to {left}. expected boolean", Position);
    }

    protected override string GetSymbol()
    {
        return "||";
    }
}