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
///     Implements the "NUnit" Api
/// </summary>
public class BadNUnitApi : BadInteropApi
{
    /// <summary>
    ///     Public Constructor
    /// </summary>
    public BadNUnitApi() : base("NUnit") { }

    /// <summary>
    ///     Creates the Assert Api
    /// </summary>
    /// <returns>Table containing all assert functions</returns>
    private static BadTable MakeAssert()
    {
        BadTable assert = new BadTable();
        assert.SetFunction<BadNullable<BadObject>, BadNullable<BadObject>, string>(
            "AreEqual",
            (a, b, c) => Assert_AreEqual(a!, b!, c)
        );
        assert.SetFunction<BadNullable<BadObject>, BadNullable<BadObject>, string>(
            "AreNotEqual",
            (a, b, c) => Assert_AreNotEqual(a!, b!, c)
        );
        assert.SetFunction<BadNullable<BadObject>, BadNullable<BadObject>, string>(
            "AreSame",
            (a, b, c) => Assert_AreSame(a!, b!, c)
        );
        assert.SetFunction<BadNullable<BadObject>, BadNullable<BadObject>, string>(
            "AreNotSame",
            (a, b, c) => Assert_AreNotSame(a!, b!, c)
        );
        assert.SetFunction<BadNullable<BadObject>, string>("IsTrue", (a, b) => Assert_IsTrue(a!, b));
        assert.SetFunction<BadNullable<BadObject>, string>("IsFalse", (a, b) => Assert_IsFalse(a!, b));
        assert.SetFunction<BadNullable<BadObject>, string>("IsNull", (a, b) => Assert_IsNull(a!, b));
        assert.SetFunction<BadNullable<BadObject>, string>("IsNotNull", (a, b) => Assert_IsNotNull(a!, b));
        assert.SetFunction<string>("Fail", Assert_Fail);
        assert.SetFunction<string>("Inconclusive", Assert_Inconclusive);
        assert.SetFunction<string>("Ignore", Assert_Ignore);
        assert.SetFunction<string>("Pass", Assert_Pass);
        assert.SetFunction<decimal, decimal, string>("Greater", Assert_Greater);
        assert.SetFunction<decimal, decimal, string>("GreaterOrEqual", Assert_GreaterOrEqual);
        assert.SetFunction<decimal, decimal, string>("Less", Assert_Less);
        assert.SetFunction<decimal, decimal, string>("LessOrEqual", Assert_LessOrEqual);
        assert.SetFunction<BadArray, BadNullable<BadObject>, string>(
            "Contains",
            (a, b, c) => Assert_Contains(a, b!, c)
        );
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
        Assert.Throws<BadRuntimeException>(
            () =>
            {
                foreach (BadObject _ in func.Invoke(Array.Empty<BadObject>(), ctx))
                {
                    //Do Nothing
                }
            },
            message
        );
    }

    private static void Assert_AreEqual(
        BadObject expected,
        BadObject actual,
        string message)
    {
        Assert.That(actual, Is.EqualTo(expected), message);
    }


    private static void Assert_AreNotEqual(
        BadObject expected,
        BadObject actual,
        string message)
    {
        Assert.That(actual, Is.Not.EqualTo(expected), message);
    }

    private static void Assert_AreSame(
        BadObject expected,
        BadObject actual,
        string message)
    {
        Assert.That(actual, Is.SameAs(expected), message);
    }

    private static void Assert_AreNotSame(
        BadObject expected,
        BadObject actual,
        string message)
    {
        Assert.That(actual, Is.Not.SameAs(expected), message);
    }

    private static void Assert_IsTrue(BadObject condition, string message)
    {
        Assert.That(
            condition is IBadBoolean
            {
                Value: true,
            },
            Is.True,
            message
        );
    }

    private static void Assert_IsFalse(BadObject condition, string message)
    {
        Assert.That(condition is not IBadBoolean b || b.Value, Is.False, message);
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
        Assert.That(obj, Is.Null, message);
    }

    private static void Assert_IsNotNull(BadObject obj, string message)
    {
        Assert.That(obj, Is.Not.Null, message);
    }

    private static void Assert_Greater(decimal a, decimal b, string message)
    {
        Assert.That(a, Is.GreaterThan(b), message);
    }


    private static void Assert_GreaterOrEqual(decimal a, decimal b, string message)
    {
        Assert.That(a, Is.GreaterThanOrEqualTo(b), message);
    }


    private static void Assert_Less(decimal a, decimal b, string message)
    {
        Assert.That(a, Is.LessThan(b), message);
    }


    private static void Assert_LessOrEqual(decimal a, decimal b, string message)
    {
        Assert.That(a, Is.LessThanOrEqualTo(b), message);
    }

    private static void Assert_Contains(
        BadArray collection,
        BadObject obj,
        string message)
    {
        Assert.That(collection.InnerArray, Contains.Value(obj), message);
    }


    private static void Assert_Positive(decimal d, string message)
    {
        Assert.That(d, Is.Positive);
    }

    private static void Assert_Negative(decimal d, string message)
    {
        Assert.That(d, Is.Negative);
    }

    private static void Assert_Zero(decimal d, string message)
    {
        Assert.That(d, Is.Zero);
    }

    private static void Assert_NotZero(decimal d, string message)
    {
        Assert.That(d, Is.Not.Zero);
    }

    private static void Assert_Pass(string message)
    {
        Assert.Pass(message);
    }

    private static void Assert_IsEmpty(BadArray collection, string message)
    {
        Assert.That(collection.InnerArray, Is.Empty, message);
    }
}