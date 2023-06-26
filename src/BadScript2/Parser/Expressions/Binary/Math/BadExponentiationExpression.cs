using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math;

public class BadExponentiationExpression : BadBinaryExpression
{
	public BadExponentiationExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left, right, position) { }

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject left = BadObject.Null;

		foreach (BadObject o in Left.Execute(context))
		{
			left = o;

			yield return o;
		}

		left = left.Dereference();
		BadObject right = BadObject.Null;

		foreach (BadObject o in Right.Execute(context))
		{
			right = o;

			yield return o;
		}

		right = right.Dereference();

		yield return Exp(left, right, Position);
	}
	
	public static BadObject Exp(BadObject left, BadObject right, BadSourcePosition pos)
	{
		if (left is IBadNumber lNum)
		{
			if (right is IBadNumber rNum)
			{
				return BadObject.Wrap(System.Math.Pow((double)lNum.Value, (double)rNum.Value));
			}
		}

		throw new BadRuntimeException($"Can not apply operator '*' to {left} and {right}", pos);
	}


	protected override string GetSymbol()
	{
		return "**";
	}
}