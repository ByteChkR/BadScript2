using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Represents a BadScript NUnit Test Case
/// </summary>
public class BadNUnitTestCase
{
	/// <summary>
	///     Constructs a new BadNUnitTestCase
	/// </summary>
	/// <param name="function">The Function to Test</param>
	/// <param name="testName">The Test Name</param>
	/// <param name="allowCompile">If Enabled, the function will be tested in compiled and uncompiled modes</param>
	public BadNUnitTestCase(BadFunction function, string? testName, bool allowCompile)
    {
        Function = function;
        TestName = testName ?? Function.ToString();
        AllowCompile = allowCompile;
    }

	/// <summary>
	///     The Test Name
	/// </summary>
	public string TestName { get; }

	/// <summary>
	///     The Function to Test
	/// </summary>
	public BadFunction? Function { get; }

	/// <summary>
	///     True if the Function should be tested in compiled and uncompiled mode
	/// </summary>
	public bool AllowCompile { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return TestName;
    }
}