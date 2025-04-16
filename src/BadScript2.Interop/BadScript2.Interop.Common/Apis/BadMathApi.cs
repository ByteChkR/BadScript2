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

    /// <inheritdoc />
    protected override void AdditionalData(BadTable target)
    {
        target.SetProperty("PI", (decimal)Math.PI);
        target.SetProperty("E", (decimal)Math.E);
        target.SetProperty("Tau", (decimal)Math.PI * 2);
    }

    /// <summary>
    /// Wrapper for Math.Abs
    /// </summary>
    /// <param name="x">A number that is greater than or equal to MinValue, but less than or equal to MaxValue.</param>
    /// <returns>A decimal number, x, such that 0 ≤ x ≤MaxValue.</returns>
    [BadMethod(description: "Returns the absolute value of a number")]
    [return: BadReturn("A decimal number, x, such that 0 \u2264 x \u2264MaxValue.")]
    private decimal Abs(
        [BadParameter(description:
                         "A number that is greater than or equal to MinValue, but less than or equal to MaxValue."
                     )]
        decimal x)
    {
        return Math.Abs(x);
    }

    /// <summary>
    /// Wrapper for Math.Asin
    /// </summary>
    /// <param name="x">A number representing a sine, where x must be greater than or equal to -1, but less than or equal to 1</param>
    /// <returns>An angle, θ, measured in radians, such that -π/2 ≤θ≤π/2 -or- NaN if x < -1 or x > 1 or x equals NaN.</returns>
    [BadMethod(description: "Returns the angle whose sine is the specified number.")]
    [return:
        BadReturn("""An angle, θ, measured in radians, such that -π/2 ≤θ≤π/2 -or- NaN if x < -1 or x > 1 or x equals NaN."""
                 )]
    private decimal Asin(
        [BadParameter(description:
                         "A number representing a sine, where x must be greater than or equal to -1, but less than or equal to 1"
                     )]
        decimal x)
    {
        return (decimal)Math.Asin((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Acos
    /// </summary>
    /// <param name="x">A number representing a cosine, where x must be greater than or equal to -1, but less than or equal to 1</param>
    /// <returns>An angle, θ, measured in radians, such that 0 ≤θ≤π -or- NaN if x < -1 or x > 1 or x equals NaN.</returns>
    [BadMethod(description: "Returns the angle whose cosine is the specified number.")]
    [return:
        BadReturn("An angle, θ, measured in radians, such that 0 \u2264θ\u2264π -or- NaN if x < -1 or x > 1 or x equals NaN."
                 )]
    private decimal Acos(
        [BadParameter(description:
                         "A number representing a cosine, where x must be greater than or equal to -1, but less than or equal to 1"
                     )]
        decimal x)
    {
        return (decimal)Math.Acos((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Atan
    /// </summary>
    /// <param name="x">A number representing a tangent</param>
    /// <returns>An angle, θ, measured in radians, such that -π/2 ≤θ≤π/2 -or- NaN if x equals NaN, -π/2 rounded to double precision (-1.5707963267949) if x equals NegativeInfinity, or π/2 rounded to double precision (1.5707963267949) if x equals PositiveInfinity.</returns>
    [BadMethod(description: "Returns the angle whose tangent is the specified number.")]
    [return:
        BadReturn("An angle, θ, measured in radians, such that -π/2 \u2264θ\u2264π/2. -or- NaN if x equals NaN, -π/2 rounded to double precision (-1.5707963267949) if x equals NegativeInfinity, or π/2 rounded to double precision (1.5707963267949) if x equals PositiveInfinity."
                 )]
    private decimal Atan([BadParameter(description: "A number representing a tangent")] decimal x)
    {
        return (decimal)Math.Atan((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Atan2
    /// </summary>
    /// <param name="y">The y coordinate of a point.</param>
    /// <param name="x">The x coordinate of a point.</param>
    /// <returns>An angle, θ, measured in radians, such that -π ≤θ≤π, and tan(θ) = y / x, where (x, y) is a point in the Cartesian plane. Observe the following: For (x, y) in quadrant 1, 0 < θ < π/2. For (x, y) in quadrant 2, π/2 < θ≤π. For (x, y) in quadrant 3, -π < θ < -π/2. For (x, y) in quadrant 4, -π/2 < θ < 0. For points on the boundaries of the quadrants, the return value is the following: If y is 0 and x is not negative, θ = 0. If y is 0 and x is negative, θ = π. If y is positive and x is 0, θ = π/2. If y is negative and x is 0, θ = -π/2. If y is 0 and x is 0, θ = 0. If x or y is NaN, or if x and y are either PositiveInfinity or NegativeInfinity, the method returns NaN</returns>
    [BadMethod(description: "Returns the angle whose tangent is the quotient of two specified numbers.")]
    [return:
        BadReturn(
                     "An angle, θ, measured in radians, such that -π\u2264θ\u2264π, and tan(θ) = y / x, where (x, y) is a point in the Cartesian plane. Observe the following: For (x, y) in quadrant 1, 0 < θ < π/2. For (x, y) in quadrant 2, π/2 < θ\u2264π. For (x, y) in quadrant 3, -π < θ < -π/2. For (x, y) in quadrant 4, -π/2 < θ < 0. For points on the boundaries of the quadrants, the return value is the following: If y is 0 and x is not negative, θ = 0. If y is 0 and x is negative, θ = π. If y is positive and x is 0, θ = π/2. If y is negative and x is 0, θ = -π/2. If y is 0 and x is 0, θ = 0. If x or y is NaN, or if x and y are either PositiveInfinity or NegativeInfinity, the method returns NaN"
                 )]
    private decimal Atan2([BadParameter(description: "The y coordinate of a point.")] decimal y,
                          [BadParameter(description: "The x coordinate of a point.")] decimal x)
    {
        return (decimal)Math.Atan2((double)y, (double)x);
    }

    /// <summary>
    /// Wrapper for Math.Ceiling
    /// </summary>
    /// <param name="x">A decimal number.</param>
    /// <returns>The smallest integral value that is greater than or equal to x. Note that this method returns a Decimal instead of an integral type.</returns>
    [BadMethod(description: "Returns the smallest integer greater than or equal to the specified number.")]
    [return:
        BadReturn("The smallest integral value that is greater than or equal to x. Note that this method returns a Decimal instead of an integral type"
                 )]
    private decimal Ceiling([BadParameter(description: "A decimal number.")] decimal x)
    {
        return Math.Ceiling(x);
    }

    /// <summary>
    /// Wrapper for Math.Cos
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>The cosine of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.</returns>
    [BadMethod(description: "Returns the cosine of the specified angle.")]
    [return:
        BadReturn("The cosine of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN."
                 )]
    private decimal Cos([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Cos((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Cosh
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>The hyperbolic cosine of x. If x is equal to NegativeInfinity or PositiveInfinity, PositiveInfinity is returned. If x is equal to NaN, NaN is returned.</returns>
    [BadMethod(description: "Returns the hyperbolic cosine of the specified angle.")]
    [return:
        BadReturn("The hyperbolic cosine of x. If x is equal to NegativeInfinity or PositiveInfinity, PositiveInfinity is returned. If x is equal to NaN, NaN is returned"
                 )]
    private decimal Cosh([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Cosh((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Exp
    /// </summary>
    /// <param name="x">A number specifying a power.</param>
    /// <returns>The number e raised to the power x. If x equals NaN or PositiveInfinity, that value is returned. If x equals NegativeInfinity, 0 is returned.</returns>
    [BadMethod(description: "Returns the value of e raised to the specified power.")]
    [return:
        BadReturn("The number e raised to the power x. If x equals NaN or PositiveInfinity, that value is returned. If x equals NegativeInfinity, 0 is returned."
                 )]
    private decimal Exp([BadParameter(description: "A number specifying a power")] decimal x)
    {
        return (decimal)Math.Exp((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Floor
    /// </summary>
    /// <param name="x">A decimal number.</param>
    /// <returns>The largest integral value that is less than or equal to x. Note that this method returns a Decimal instead of an integral type.</returns>
    [BadMethod(description: "Returns the largest integer less than or equal to the specified number.")]
    [return:
        BadReturn("The largest integral value that is less than or equal to x. Note that this method returns a Decimal instead of an integral type"
                 )]
    private decimal Floor([BadParameter(description: "A decimal number.")] decimal x)
    {
        return Math.Floor(x);
    }

    /// <summary>
    /// Wrapper for Math.Log
    /// </summary>
    /// <param name="x">A decimal number.</param>
    /// <returns>The natural logarithm of x; that is, ln x, where e is approximately equal to 2.71828182845904. If x is equal to NaN or NegativeInfinity, that value is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned.</returns>
    [BadMethod(description: "Returns the natural (base e) logarithm of a specified number.")]
    [return:
        BadReturn("The natural logarithm of x; that is, ln x, where e is approximately equal to 2.71828182845904. If x is equal to NaN or NegativeInfinity, that value is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned."
                 )]
    private decimal Log([BadParameter(description: "A decimal number.")] decimal x)
    {
        return (decimal)Math.Log((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Log10
    /// </summary>
    /// <param name="x">A decimal number.</param>
    /// <returns>The base 10 logarithm of x; that is, log10 x. If x is equal to NaN or NegativeInfinity, that value is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned.</returns>
    [BadMethod(description: "Returns the logarithm of a specified number with base 10.")]
    [return:
        BadReturn("The base 10 logarithm of x; that is, log10 x. If x is equal to NaN or NegativeInfinity, that value is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned."
                 )]
    private decimal Log10([BadParameter(description: "A decimal number.")] decimal x)
    {
        return (decimal)Math.Log10((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Max
    /// </summary>
    /// <param name="x">The first of two decimal numbers to compare.</param>
    /// <param name="y">The second of two decimal numbers to compare.</param>
    /// <returns>The larger of x or y. If x, or y, or both x and y are equal to NaN, NaN is returned.</returns>
    [BadMethod(description: "Returns the larger of two numbers.")]
    [return: BadReturn("The larger of x or y. If x, or y, or both x and y are equal to NaN, NaN is returned.")]
    private decimal Max([BadParameter(description: "The first of two decimal numbers to compare.")] decimal x,
                        [BadParameter(description: "The second of two decimal numbers to compare.")]
                        decimal y)
    {
        return Math.Max(x, y);
    }

    /// <summary>
    /// Wrapper for Math.Min
    /// </summary>
    /// <param name="x">The first of two decimal numbers to compare.</param>
    /// <param name="y">The second of two decimal numbers to compare.</param>
    /// <returns>The smaller of x or y. If x, or y, or both x and y are equal to NaN, NaN is returned.</returns>
    [BadMethod(description: "Returns the smaller of two numbers.")]
    [return: BadReturn("The smaller of x or y. If x, or y, or both x and y are equal to NaN, NaN is returned.")]
    private decimal Min([BadParameter(description: "The first of two decimal numbers to compare.")] decimal x,
                        [BadParameter(description: "The second of two decimal numbers to compare.")]
                        decimal y)
    {
        return Math.Min(x, y);
    }

    /// <summary>
    /// Wrapper for Math.Pow
    /// </summary>
    /// <param name="x">A double-precision floating-point number to be raised to a power.</param>
    /// <param name="y">A double-precision floating-point number that specifies a power.</param>
    /// <returns>The number x raised to the power y.</returns>
    [BadMethod(description: "Returns a specified number raised to the specified power.")]
    [return: BadReturn("The number x raised to the power y.")]
    private decimal Pow(
        [BadParameter(description: "A double-precision floating-point number to be raised to a power")] decimal x,
        [BadParameter(description: "A double-precision floating-point number that specifies a power")]
        decimal y)
    {
        return (decimal)Math.Pow((double)x, (double)y);
    }

    /// <summary>
    /// Wrapper for Math.Round
    /// </summary>
    /// <param name="x">A decimal number to be rounded.</param>
    /// <param name="y">The number nearest to x that contains a number of fractional digits equal to decimals.</param>
    /// <returns>The number nearest to x that contains a number of fractional digits equal to decimals.</returns>
    [BadMethod(description: "Rounds a value to the nearest integer or to the specified number of decimal places.")]
    [return: BadReturn("The number nearest to x that contains a number of fractional digits equal to decimals.")]
    private decimal Round([BadParameter(description: "A decimal number to be rounded")] decimal x,
                          [BadParameter(description:
                                           "The number nearest to x that contains a number of fractional digits equal to decimals"
                                       )]
                          int y)
    {
        return Math.Round(x, y);
    }

    /// <summary>
    /// Wrapper for Math.Sign
    /// </summary>
    /// <param name="x">A decimal number.</param>
    /// <returns>A number that indicates the sign of x, as shown in the following table. Value Meaning -1 x is less than zero. 0 x is equal to zero. 1 x is greater than zero.</returns>
    [BadMethod(description: "Returns a value indicating the sign of a number.")]
    [return:
        BadReturn("A number that indicates the sign of x, as shown in the following table. Value Meaning -1 x is less than zero. 0 x is equal to zero. 1 x is greater than zero."
                 )]
    private decimal Sign([BadParameter(description: "A decimal number.")] decimal x)
    {
        return Math.Sign(x);
    }

    /// <summary>
    /// Wrapper for Math.Sin
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>The sine of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.</returns>
    [BadMethod(description: "Returns the sine of the specified angle.")]
    [return:
        BadReturn("The sine of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN."
                 )]
    private decimal Sin([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Sin((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Sinh
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>The hyperbolic sine of x. If x is equal to NegativeInfinity, NegativeInfinity is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned. If x is equal to NaN, NaN is returned.</returns>
    [BadMethod(description: "Returns the hyperbolic sine of the specified angle.")]
    [return:
        BadReturn("The hyperbolic sine of x. If x is equal to NegativeInfinity, NegativeInfinity is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned. If x is equal to NaN, NaN is returned."
                 )]
    private decimal Sinh([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Sinh((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Sqrt
    /// </summary>
    /// <param name="x">A decimal number.</param>
    /// <returns>The positive square root of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, that value is returned.</returns>
    [BadMethod(description: "Returns the square root of a specified number.")]
    [return:
        BadReturn("The positive square root of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, that value is returned."
                 )]
    private decimal Sqrt([BadParameter(description: "A decimal number.")] decimal x)
    {
        return (decimal)Math.Sqrt((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Tan
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>The tangent of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.</returns>
    [BadMethod(description: "Returns the tangent of the specified angle.")]
    [return:
        BadReturn("The tangent of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN."
                 )]
    private decimal Tan([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Tan((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Tanh
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>The hyperbolic tangent of x. If x is equal to NegativeInfinity, -1 is returned. If x is equal to PositiveInfinity, 1 is returned. If x is equal to NaN, NaN is returned.</returns>
    [BadMethod(description: "Returns the hyperbolic tangent of the specified angle.")]
    [return:
        BadReturn("The hyperbolic tangent of x. If x is equal to NegativeInfinity, -1 is returned. If x is equal to PositiveInfinity, 1 is returned. If x is equal to NaN, NaN is returned."
                 )]
    private decimal Tanh([BadParameter(description: "An angle, measured in radians")] decimal x)
    {
        return (decimal)Math.Tanh((double)x);
    }

    /// <summary>
    /// Wrapper for Math.Truncate
    /// </summary>
    /// <param name="x">A decimal number.</param>
    /// <returns>The integral part of x; that is, the number that remains after any fractional digits have been discarded.</returns>
    [BadMethod(description: "Returns the integral part of a specified decimal number.")]
    [return:
        BadReturn("The integral part of d; that is, the number that remains after any fractional digits have been discarded."
                 )]
    private decimal Truncate([BadParameter(description: "A number to truncate")] decimal x)
    {
        return Math.Truncate(x);
    }

    /// <summary>
    /// Wrapper for Math.NextRandom
    /// </summary>
    /// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
    [BadMethod(description: "Returns a random number between 0.0 and 1.0.")]
    [return: BadReturn("A double-precision floating point number greater than or equal to 0.0, and less than 1.0.")]
    private decimal NextRandom()
    {
        return (decimal)s_Random.NextDouble();
    }
}