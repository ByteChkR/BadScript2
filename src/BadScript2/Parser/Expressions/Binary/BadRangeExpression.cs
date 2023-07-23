using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary;

public class BadUnaryUnpackExpression : BadExpression
{
	public readonly BadExpression Right;

	public BadUnaryUnpackExpression(BadExpression right) : base(right.IsConstant, right.Position)
	{
		Right = right;
	}


	public override IEnumerable<BadExpression> GetDescendants()
	{
		foreach (BadExpression? expression in Right.GetDescendantsAndSelf())
		{
			yield return expression;
		}
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadTable result = context.Scope.GetTable();
		BadObject right = BadObject.Null;

		foreach (BadObject o in Right.Execute(context))
		{
			right = o;

			yield return o;
		}

		right = right.Dereference();

		if (right is not BadTable rightT)
		{
			throw new BadRuntimeException("Unpack operator requires 1 table", Position);
		}

		foreach (KeyValuePair<BadObject, BadObject> o in rightT.InnerTable)
		{
			result.InnerTable[o.Key] = o.Value;
			result.PropertyInfos[o.Key] = rightT.PropertyInfos[o.Key];
		}

		yield return result;
	}
}

public class BadBinaryUnpackExpression : BadBinaryExpression
{
	public BadBinaryUnpackExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
		right,
		position) { }

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadTable result = new BadTable();
		BadObject left = BadObject.Null;
		BadObject right = BadObject.Null;

		foreach (BadObject o in Left.Execute(context))
		{
			left = o;

			yield return o;
		}

		foreach (BadObject o in Right.Execute(context))
		{
			right = o;

			yield return o;
		}

		left = left.Dereference();
		right = right.Dereference();

		if (left is not BadTable leftT || right is not BadTable rightT)
		{
			throw new BadRuntimeException("Unpack operator requires 2 tables", Position);
		}

		foreach (KeyValuePair<BadObject, BadObject> o in leftT.InnerTable)
		{
			result.InnerTable[o.Key] = o.Value;
			result.PropertyInfos[o.Key] = leftT.PropertyInfos[o.Key];
		}

		foreach (KeyValuePair<BadObject, BadObject> o in rightT.InnerTable)
		{
			result.InnerTable[o.Key] = o.Value;
			result.PropertyInfos[o.Key] = rightT.PropertyInfos[o.Key];
		}

		yield return result;
	}

	protected override string GetSymbol()
	{
		return "...";
	}
}

/// <summary>
///     Implements the Range Expression
///     <Start>..<End>
/// </summary>
public class BadRangeExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Range Expression
    /// </summary>
    /// <param name="left">Start of the Range</param>
    /// <param name="right">End of the Range</param>
    /// <param name="position">Source position of the Expression</param>
    public BadRangeExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
		right,
		position) { }

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject left = BadObject.Null;
		BadObject right = BadObject.Null;

		foreach (BadObject o in Left.Execute(context))
		{
			left = o;

			yield return o;
		}

		foreach (BadObject o in Right.Execute(context))
		{
			right = o;

			yield return o;
		}

		left = left.Dereference();
		right = right.Dereference();

		if (left is not IBadNumber lNum)
		{
			throw new BadRuntimeException("Left side of range operator is not a number", Position);
		}

		if (right is not IBadNumber rNum)
		{
			throw new BadRuntimeException("Right side of range operator is not a number", Position);
		}

		if (lNum.Value > rNum.Value)
		{
			throw new BadRuntimeException("Left side of range operator is greater than right side", Position);
		}

		yield return new BadInteropEnumerator(Range(lNum.Value, rNum.Value).GetEnumerator());
	}

    /// <summary>
    ///     Returns a range of numbers
    /// </summary>
    /// <param name="from">Start of the Range</param>
    /// <param name="to">End of the Range(Exclusive></param>
    /// <returns></returns>
    public static IEnumerable<BadObject> Range(decimal from, decimal to)
	{
		for (decimal i = from; i < to; i++)
		{
			yield return BadObject.Wrap(i);
		}
	}

	protected override string GetSymbol()
	{
		return "..";
	}
}
