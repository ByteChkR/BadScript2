using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Implements the "NUnit" Api
/// </summary>
[BadInteropApi("Assert")]
internal partial class BadNUnitAssertApi
{
    /// <summary>
    ///     Asserts that the given function throws a BadRuntimeException
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="func">The Function to execute</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("Throws", "Asserts that the given function throws a BadRuntimeException")]
    private static void Assert_Throws(
        BadExecutionContext ctx,
        [BadParameter(description: "The Function that is expected to throw")] BadFunction func,
        [BadParameter(description: "The Message")] string message)
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

    /// <summary>
    ///     Asserts that the given objects are equal
    /// </summary>
    /// <param name="expected">The Expected Value</param>
    /// <param name="actual">The Actual Value</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("AreEqual", "Asserts that the given objects are equal")]
    private static void Assert_AreEqual(
        [BadParameter(description: "The Expected Value")]
        BadObject? expected,
        [BadParameter(description: "The Actual Value")]
        BadObject? actual,
        [BadParameter(description: "The Message")]
        string message)
    {
        Assert.That(actual, Is.EqualTo(expected), message);
    }

    /// <summary>
    ///     Asserts that the given objects are not equal
    /// </summary>
    /// <param name="expected">The Expected Value</param>
    /// <param name="actual">The Actual Value</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("AreNotEqual", "Asserts that the given objects are not equal")]
    private static void Assert_AreNotEqual(
        [BadParameter(description: "The Expected Value")]
        BadObject? expected,
        [BadParameter(description: "The Actual Value")]
        BadObject? actual,
        [BadParameter(description: "The Message")]
        string message)
    {
        Assert.That(actual, Is.Not.EqualTo(expected), message);
    }

    /// <summary>
    ///     Asserts that the given objects are the same
    /// </summary>
    /// <param name="expected">The Expected Value</param>
    /// <param name="actual">The Actual Value</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("AreSame", "Asserts that the given objects are the same")]
    private static void Assert_AreSame(
        [BadParameter(description: "The Expected Value")]
        BadObject? expected,
        [BadParameter(description: "The Actual Value")]
        BadObject? actual,
        [BadParameter(description: "The Message")]
        string message)
    {
        Assert.That(actual, Is.SameAs(expected), message);
    }

    /// <summary>
    ///     Asserts that the given objects are not the same
    /// </summary>
    /// <param name="expected">The Expected Value</param>
    /// <param name="actual">The Actual Value</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("AreNotSame", "Asserts that the given objects are not the same")]
    private static void Assert_AreNotSame(
        [BadParameter(description: "The Expected Value")]
        BadObject? expected,
        [BadParameter(description: "The Actual Value")]
        BadObject? actual,
        [BadParameter(description: "The Message")]
        string message)
    {
        Assert.That(actual, Is.Not.SameAs(expected), message);
    }

    /// <summary>
    ///     Asserts that the given condition is true
    /// </summary>
    /// <param name="condition">The Condition to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("IsTrue", "Asserts that the given condition is true")]
    private static void Assert_IsTrue([BadParameter(description: "The Condition to check")] BadObject? condition, [BadParameter(description: "The Message")] string message)
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

    /// <summary>
    ///     Asserts that the given condition is false
    /// </summary>
    /// <param name="condition">The Condition to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("IsFalse", "Asserts that the given condition is false")]
    private static void Assert_IsFalse([BadParameter(description: "The Condition to check")] BadObject? condition, [BadParameter(description: "The Message")] string message)
    {
        Assert.That(condition is not IBadBoolean b || b.Value, Is.False, message);
    }


    /// <summary>
    ///     Fails the test with the given message
    /// </summary>
    /// <param name="message">The Message to display</param>
    [BadMethod("Fail", "Fails the test with the given message")]
    private static void Assert_Fail([BadParameter(description: "The Message")] string message)
    {
        Assert.Fail(message);
    }

    /// <summary>
    ///     Marks the test as inconclusive with the given message
    /// </summary>
    /// <param name="message">The Message to display</param>
    [BadMethod("Inconclusive", "Marks the test as inconclusive with the given message")]
    private static void Assert_Inconclusive([BadParameter(description: "The Message")] string message)
    {
        Assert.Inconclusive(message);
    }

    /// <summary>
    ///     Ignore the test with the given message
    /// </summary>
    /// <param name="message">The Message to display</param>
    [BadMethod("Ignore", "Ignore the test with the given message")]
    private static void Assert_Ignore([BadParameter(description: "The Message")] string message)
    {
        Assert.Ignore(message);
    }


    /// <summary>
    ///     Asserts that the given object is null
    /// </summary>
    /// <param name="obj">The Object to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("IsNull", "Asserts that the given object is null")]
    private static void Assert_IsNull([BadParameter(description: "The Object to check")] BadObject? obj, [BadParameter(description: "The Message")] string message)
    {
        Assert.That(obj, Is.Null, message);
    }

    /// <summary>
    ///     Asserts that the given object is not null
    /// </summary>
    /// <param name="obj">The Object to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("IsNotNull", "Asserts that the given object is not null")]
    private static void Assert_IsNotNull([BadParameter(description: "The Object to check")] BadObject? obj, [BadParameter(description: "The Message")] string message)
    {
        Assert.That(obj, Is.Not.Null, message);
    }

    /// <summary>
    ///     Asserts that the given object is greater than the other object
    /// </summary>
    /// <param name="a">actual</param>
    /// <param name="b">expected</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("Greater", "Asserts that the given object is greater than the other object")]
    private static void Assert_Greater(
        [BadParameter(description: "Actual")] decimal a,
        [BadParameter(description: "Expected")] decimal b,
        [BadParameter(description: "The Message")] string message)
    {
        Assert.That(a, Is.GreaterThan(b), message);
    }

    /// <summary>
    ///     Asserts that the given object is greater or equal the other object
    /// </summary>
    /// <param name="a">actual</param>
    /// <param name="b">expected</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("GreaterOrEqual", "Asserts that the given object is greater or equal the other object")]
    private static void Assert_GreaterOrEqual(
        [BadParameter(description: "Actual")] decimal a,
        [BadParameter(description: "Expected")] decimal b,
        [BadParameter(description: "The Message")] string message)
    {
        Assert.That(a, Is.GreaterThanOrEqualTo(b), message);
    }

    /// <summary>
    ///     Asserts that the given object is less than the other object
    /// </summary>
    /// <param name="a">actual</param>
    /// <param name="b">expected</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("Less", "Asserts that the given object is less than the other object")]
    private static void Assert_Less(
        [BadParameter(description: "Actual")] decimal a,
        [BadParameter(description: "Expected")] decimal b,
        [BadParameter(description: "The Message")] string message)
    {
        Assert.That(a, Is.LessThan(b), message);
    }

    /// <summary>
    ///     Asserts that the given object is less or equal the other object
    /// </summary>
    /// <param name="a">actual</param>
    /// <param name="b">expected</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("LessOrEqual", "Asserts that the given object is less or equal the other object")]
    private static void Assert_LessOrEqual(
        [BadParameter(description: "Actual")] decimal a,
        [BadParameter(description: "Expected")] decimal b,
        [BadParameter(description: "The Message")] string message)
    {
        Assert.That(a, Is.LessThanOrEqualTo(b), message);
    }

    /// <summary>
    ///     Asserts that the given collection contains the given object
    /// </summary>
    /// <param name="collection">The Collection to check</param>
    /// <param name="obj">The Object to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("Contains", "Asserts that the given collection contains the given object")]
    private static void Assert_Contains(
        [BadParameter(description: "The Collection to Check")]
        BadArray collection,
        [BadParameter(description: "The Object to Check")]
        BadObject? obj,
        [BadParameter(description: "The Message")]
        string message)
    {
        Assert.That(collection.InnerArray, Contains.Value(obj!), message);
    }


    /// <summary>
    ///     Asserts that the given object is positive
    /// </summary>
    /// <param name="d">The Object to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("Positive", "Asserts that the given object is positive")]
    private static void Assert_Positive([BadParameter(description: "The Object to Check")] decimal d, [BadParameter(description: "The Message")] string message)
    {
        Assert.That(d, Is.Positive);
    }

    /// <summary>
    ///     Asserts that the given object is negative
    /// </summary>
    /// <param name="d">The Object to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("Negative", "Asserts that the given object is negative")]
    private static void Assert_Negative([BadParameter(description: "The Object to Check")] decimal d, [BadParameter(description: "The Message")] string message)
    {
        Assert.That(d, Is.Negative);
    }

    /// <summary>
    ///     Asserts that the given object is zero
    /// </summary>
    /// <param name="d">The Object to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("Zero", "Asserts that the given object is zero")]
    private static void Assert_Zero([BadParameter(description: "The Object to Check")] decimal d, [BadParameter(description: "The Message")] string message)
    {
        Assert.That(d, Is.Zero);
    }

    /// <summary>
    ///     Asserts that the given object is not zero
    /// </summary>
    /// <param name="d">The Object to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("NotZero", "Asserts that the given object is not zero")]
    private static void Assert_NotZero([BadParameter(description: "The Object to Check")] decimal d, [BadParameter(description: "The Message")] string message)
    {
        Assert.That(d, Is.Not.Zero);
    }

    /// <summary>
    ///     Passes the test with the given message
    /// </summary>
    /// <param name="message">The Message to display</param>
    [BadMethod("Pass", "Passes the test with the given message")]
    private static void Assert_Pass([BadParameter(description: "The Message")] string message)
    {
        Assert.Pass(message);
    }

    /// <summary>
    ///     Asserts that the given collection is empty
    /// </summary>
    /// <param name="collection">The Collection to check</param>
    /// <param name="message">The Message to display</param>
    [BadMethod("IsEmpty", "Asserts that the given collection is empty")]
    private static void Assert_IsEmpty([BadParameter(description: "The Collection to Check")] BadArray collection, [BadParameter(description: "The Message")] string message)
    {
        Assert.That(collection.InnerArray, Is.Empty, message);
    }
}