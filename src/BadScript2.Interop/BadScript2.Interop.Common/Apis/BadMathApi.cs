using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Apis;

/// <summary>
///     Implements the "Math" API
/// </summary>
[BadInteropApi("Math")]
internal partial class BadMathApi
{
    /// <summary>
    ///     Random Number Generator
    /// </summary>
    private static readonly Random s_Random = new Random();

    protected override void AdditionalData(BadTable target)
    {
        target.SetProperty("PI", (decimal)Math.PI);
        target.SetProperty("E", (decimal)Math.E);
        target.SetProperty("Tau", (decimal)Math.PI * 2);
    }

    [BadMethod(description: "Returns the absolute value of a number")]
    [return: BadReturn("A decimal number, x, such that 0 \u2264 x \u2264MaxValue.")]
    private decimal Abs([BadParameter(description: "A number that is greater than or equal to MinValue, but less than or equal to MaxValue.")] decimal x)
    {
        return Math.Abs(x);
    }

    [BadMethod(description: "Returns the angle whose sine is the specified number.")]
    [return: BadReturn("An angle, θ, measured in radians, such that -π/2 \u2264θ\u2264π/2 -or- NaN if x < -1 or x > 1 or x equals NaN.")]
    private decimal Asin([BadParameter(description: "A number representing a sine, where x must be greater than or equal to -1, but less than or equal to 1")] decimal x)
    {
        return (decimal)Math.Asin((double)x);
    }

    [BadMethod(description: "Returns the angle whose cosine is the specified number.")]
    [return: BadReturn("An angle, θ, measured in radians, such that 0 \u2264θ\u2264π -or- NaN if x < -1 or x > 1 or x equals NaN.")]
    private decimal Acos([BadParameter(description: "A number representing a cosine, where x must be greater than or equal to -1, but less than or equal to 1")] decimal x)
    {
        return (decimal)Math.Acos((double)x);
    }

    [BadMethod(description: "Returns the angle whose tangent is the specified number.")]
    [return:
        BadReturn(
            "An angle, θ, measured in radians, such that -π/2 \u2264θ\u2264π/2. -or- NaN if x equals NaN, -π/2 rounded to double precision (-1.5707963267949) if x equals NegativeInfinity, or π/2 rounded to double precision (1.5707963267949) if x equals PositiveInfinity."
        )]
    private decimal Atan([BadParameter(description: "A number representing a tangent")] decimal x)
    {
        return (decimal)Math.Atan((double)x);
    }

    [BadMethod(description: "Returns the angle whose tangent is the quotient of two specified numbers.")]
    [return:
        BadReturn(
            "An angle, θ, measured in radians, such that -π\u2264θ\u2264π, and tan(θ) = y / x, where (x, y) is a point in the Cartesian plane. Observe the following: For (x, y) in quadrant 1, 0 < θ < π/2. For (x, y) in quadrant 2, π/2 < θ\u2264π. For (x, y) in quadrant 3, -π < θ < -π/2. For (x, y) in quadrant 4, -π/2 < θ < 0. For points on the boundaries of the quadrants, the return value is the following: If y is 0 and x is not negative, θ = 0. If y is 0 and x is negative, θ = π. If y is positive and x is 0, θ = π/2. If y is negative and x is 0, θ = -π/2. If y is 0 and x is 0, θ = 0. If x or y is NaN, or if x and y are either PositiveInfinity or NegativeInfinity, the method returns NaN"
        )]
    private decimal Atan2([BadParameter(description: "The y coordinate of a point.")] decimal y, [BadParameter(description: "The x coordinate of a point.")] decimal x)
    {
        return (decimal)Math.Atan2((double)y, (double)x);
    }

    [BadMethod(description: "Returns the smallest integer greater than or equal to the specified number.")]
    [return: BadReturn("The smallest integral value that is greater than or equal to x. Note that this method returns a Decimal instead of an integral type")]
    private decimal Ceiling([BadParameter(description: "A decimal number.")] decimal x)
    {
        return Math.Ceiling(x);
    }

    [BadMethod(description: "Returns the cosine of the specified angle.")]
    [return: BadReturn("The cosine of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.")]
    private decimal Cos([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Cos((double)x);
    }

    [BadMethod(description: "Returns the hyperbolic cosine of the specified angle.")]
    [return: BadReturn("The hyperbolic cosine of x. If x is equal to NegativeInfinity or PositiveInfinity, PositiveInfinity is returned. If x is equal to NaN, NaN is returned")]
    private decimal Cosh([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Cosh((double)x);
    }

    [BadMethod(description: "Returns the value of e raised to the specified power.")]
    [return: BadReturn("The number e raised to the power x. If x equals NaN or PositiveInfinity, that value is returned. If x equals NegativeInfinity, 0 is returned.")]
    private decimal Exp([BadParameter(description: "A number specifying a power")] decimal x)
    {
        return (decimal)Math.Exp((double)x);
    }

    [BadMethod(description: "Returns the largest integer less than or equal to the specified number.")]
    [return: BadReturn("The largest integral value that is less than or equal to x. Note that this method returns a Decimal instead of an integral type")]
    private decimal Floor([BadParameter(description: "A decimal number.")] decimal x)
    {
        return Math.Floor(x);
    }

    [BadMethod(description: "Returns the natural (base e) logarithm of a specified number.")]
    [return:
        BadReturn(
            "The natural logarithm of x; that is, ln x, where e is approximately equal to 2.71828182845904. If x is equal to NaN or NegativeInfinity, that value is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned."
        )]
    private decimal Log([BadParameter(description: "A decimal number.")] decimal x)
    {
        return (decimal)Math.Log((double)x);
    }

    [BadMethod(description: "Returns the logarithm of a specified number with base 10.")]
    [return:
        BadReturn(
            "The base 10 logarithm of x; that is, log10 x. If x is equal to NaN or NegativeInfinity, that value is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned."
        )]
    private decimal Log10([BadParameter(description: "A decimal number.")] decimal x)
    {
        return (decimal)Math.Log10((double)x);
    }

    [BadMethod(description: "Returns the larger of two numbers.")]
    [return: BadReturn("The larger of x or y. If x, or y, or both x and y are equal to NaN, NaN is returned.")]
    private decimal Max(
        [BadParameter(description: "The first of two decimal numbers to compare.")]
        decimal x,
        [BadParameter(description: "The second of two decimal numbers to compare.")]
        decimal y)
    {
        return Math.Max(x, y);
    }

    [BadMethod(description: "Returns the smaller of two numbers.")]
    [return: BadReturn("The smaller of x or y. If x, or y, or both x and y are equal to NaN, NaN is returned.")]
    private decimal Min(
        [BadParameter(description: "The first of two decimal numbers to compare.")]
        decimal x,
        [BadParameter(description: "The second of two decimal numbers to compare.")]
        decimal y)
    {
        return Math.Min(x, y);
    }

    [BadMethod(description: "Returns a specified number raised to the specified power.")]
    [return: BadReturn("The number x raised to the power y.")]
    private decimal Pow(
        [BadParameter(description: "A double-precision floating-point number to be raised to a power")]
        decimal x,
        [BadParameter(description: "A double-precision floating-point number that specifies a power")]
        decimal y)
    {
        return (decimal)Math.Pow((double)x, (double)y);
    }

    [BadMethod(description: "Rounds a value to the nearest integer or to the specified number of decimal places.")]
    [return: BadReturn("The number nearest to x that contains a number of fractional digits equal to decimals.")]
    private decimal Round(
        [BadParameter(description: "A decimal number to be rounded")]
        decimal x,
        [BadParameter(description: "The number nearest to x that contains a number of fractional digits equal to decimals")]
        int y)
    {
        return Math.Round(x, y);
    }

    [BadMethod(description: "Returns a value indicating the sign of a number.")]
    [return:
        BadReturn("A number that indicates the sign of x, as shown in the following table. Value Meaning -1 x is less than zero. 0 x is equal to zero. 1 x is greater than zero.")]
    private decimal Sign([BadParameter(description: "A decimal number.")] decimal x)
    {
        return Math.Sign(x);
    }

    [BadMethod(description: "Returns the sine of the specified angle.")]
    [return: BadReturn("The sine of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.")]
    private decimal Sin([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Sin((double)x);
    }

    [BadMethod(description: "Returns the hyperbolic sine of the specified angle.")]
    [return:
        BadReturn(
            "The hyperbolic sine of x. If x is equal to NegativeInfinity, NegativeInfinity is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned. If x is equal to NaN, NaN is returned."
        )]
    private decimal Sinh([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Sinh((double)x);
    }

    [BadMethod(description: "Returns the square root of a specified number.")]
    [return: BadReturn("The positive square root of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, that value is returned.")]
    private decimal Sqrt([BadParameter(description: "A decimal number.")] decimal x)
    {
        return (decimal)Math.Sqrt((double)x);
    }

    [BadMethod(description: "Returns the tangent of the specified angle.")]
    [return: BadReturn("The tangent of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.")]
    private decimal Tan([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Tan((double)x);
    }

    [BadMethod(description: "Returns the hyperbolic tangent of the specified angle.")]
    [return:
        BadReturn(
            "The hyperbolic tangent of x. If x is equal to NegativeInfinity, -1 is returned. If x is equal to PositiveInfinity, 1 is returned. If x is equal to NaN, NaN is returned."
        )]
    private decimal Tanh([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Tanh((double)x);
    }

    [BadMethod(description: "Returns the integral part of a specified decimal number.")]
    [return: BadReturn("The integral part of d; that is, the number that remains after any fractional digits have been discarded.")]
    private decimal Truncate([BadParameter(description: "A number to truncate")] decimal x)
    {
        return Math.Truncate(x);
    }

    [BadMethod(description: "Returns a random number between 0.0 and 1.0.")]
    [return: BadReturn("A double-precision floating point number greater than or equal to 0.0, and less than 1.0.")]
    private decimal NextRandom()
    {
        return (decimal)s_Random.NextDouble();
    }
}