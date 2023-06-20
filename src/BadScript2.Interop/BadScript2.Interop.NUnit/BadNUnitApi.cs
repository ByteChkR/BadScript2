using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

/// <summary>
/// Implements the "NUnit" Api
/// </summary>
public class BadNUnitApi : BadInteropApi
{
	/// <summary>
	/// Public Constructor
	/// </summary>
	public BadNUnitApi() : base("NUnit") { }

	/// <summary>
	/// Creates the Assert Api
	/// </summary>
	/// <returns>Table containing all assert functions</returns>
	private static BadTable MakeAssert()
	{
		BadTable assert = new BadTable();
		assert.SetFunction<BadObject, BadObject, string>("AreEqual", Assert_AreEqual);
		assert.SetFunction<BadObject, BadObject, string>("AreNotEqual", Assert_AreNotEqual);
		assert.SetFunction<BadObject, BadObject, string>("AreSame", Assert_AreSame);
		assert.SetFunction<BadObject, BadObject, string>("AreNotSame", Assert_AreNotSame);
		assert.SetFunction<BadObject, string>("IsTrue", Assert_IsTrue);
		assert.SetFunction<BadObject, string>("IsFalse", Assert_IsFalse);
		assert.SetFunction<BadObject, string>("IsNull", Assert_IsNull);
		assert.SetFunction<BadObject, string>("IsNotNull", Assert_IsNotNull);
		assert.SetFunction<string>("Fail", Assert_Fail);
		assert.SetFunction<string>("Inconclusive", Assert_Inconclusive);
		assert.SetFunction<string>("Ignore", Assert_Ignore);
		assert.SetFunction<string>("Pass", Assert_Pass);
		assert.SetFunction<decimal, decimal, string>("Greater", Assert_Greater);
		assert.SetFunction<decimal, decimal, string>("GreaterOrEqual", Assert_GreaterOrEqual);
		assert.SetFunction<decimal, decimal, string>("Less", Assert_Less);
		assert.SetFunction<decimal, decimal, string>("LessOrEqual", Assert_LessOrEqual);
		assert.SetFunction<BadArray, BadObject, string>("Contains", Assert_Contains);
		assert.SetFunction<BadArray, string>("IsEmpty", Assert_IsEmpty);
		assert.SetFunction<decimal, string>("Positive", Assert_Positive);
		assert.SetFunction<decimal, string>("Negative", Assert_Negative);
		assert.SetFunction<decimal, string>("Zero", Assert_Zero);
		assert.SetFunction<decimal, string>("NotZero", Assert_NotZero);
		assert.SetFunction<BadFunction, string>("Throws", Assert_Throws);

		return assert;
	}

	protected override void LoadApi(BadTable target)
	{
		target.SetProperty("Assert", MakeAssert());
	}

	
	private static void Assert_Throws(BadExecutionContext ctx, BadFunction func, string message)
	{
		Assert.Throws<BadRuntimeException>(() =>
			{
				foreach (BadObject o in func.Invoke(Array.Empty<BadObject>(), ctx))
				{
					//Do Nothing
				}
			},
			message);
	}

	private static void Assert_AreEqual(
		BadObject expected,
		BadObject actual,
		string message)
	{
		Assert.AreEqual(expected, actual, message);
	}


	private static void Assert_AreNotEqual(
		BadObject expected,
		BadObject actual,
		string message)
	{
		Assert.AreNotEqual(expected, actual, message);
	}

	private static void Assert_AreSame(
		BadObject expected,
		BadObject actual,
		string message)
	{
		Assert.AreSame(expected, actual, message);
	}

	private static void Assert_AreNotSame(
		BadObject expected,
		BadObject actual,
		string message)
	{
		Assert.AreNotSame(expected, actual, message);
	}

	private static void Assert_IsTrue(BadObject condition, string message)
	{
		Assert.IsTrue(condition is IBadBoolean b && b.Value, message);
	}

	private static void Assert_IsFalse(BadObject condition, string message)
	{
		Assert.IsFalse(condition is not IBadBoolean b || b.Value, message);
	}


	private static void Assert_Fail(string message)
	{
		Assert.Fail(message);
	}

	private static void Assert_Inconclusive(string message)
	{
		Assert.Inconclusive(message);
	}

	private static void Assert_Ignore(string message)
	{
		Assert.Ignore(message);
	}


	private static void Assert_IsNull(BadObject obj, string message)
	{
		Assert.IsNull(obj, message);
	}

	private static void Assert_IsNotNull(BadObject obj, string message)
	{
		Assert.IsNotNull(obj, message);
	}

	private static void Assert_Greater(decimal a, decimal b, string message)
	{
		Assert.Greater(a, b, message);
	}


	private static void Assert_GreaterOrEqual(decimal a, decimal b, string message)
	{
		Assert.GreaterOrEqual(a, b, message);
	}


	private static void Assert_Less(decimal a, decimal b, string message)
	{
		Assert.Less(a, b, message);
	}


	private static void Assert_LessOrEqual(decimal a, decimal b, string message)
	{
		Assert.LessOrEqual(a, b, message);
	}

	private static void Assert_Contains(
		BadArray collection,
		BadObject obj,
		string message)
	{
		Assert.Contains(obj, collection.InnerArray, message);
	}


	private static void Assert_Positive(decimal d, string message)
	{
		Assert.Positive(d, message);
	}

	private static void Assert_Negative(decimal d, string message)
	{
		Assert.Negative(d, message);
	}

	private static void Assert_Zero(decimal d, string message)
	{
		Assert.Zero(d, message);
	}

	private static void Assert_NotZero(decimal d, string message)
	{
		Assert.NotZero(d, message);
	}

	private static void Assert_Pass(string message)
	{
		Assert.Pass(message);
	}

	private static void Assert_IsEmpty(BadArray collection, string message)
	{
		Assert.IsEmpty(collection.InnerArray, message);
	}
}
