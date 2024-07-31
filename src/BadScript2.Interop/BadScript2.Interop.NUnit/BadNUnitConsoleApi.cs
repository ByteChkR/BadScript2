using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Implements the "NUnit" Api(Console Version)
/// </summary>
[BadInteropApi("NUnit")]
internal partial class BadNUnitConsoleApi
{
    /// <summary>
    ///     The Console Context
    /// </summary>
    private BadUnitTestContextBuilder? m_Console;

    private BadUnitTestContextBuilder Console => m_Console ?? throw new BadRuntimeException("Console Context not set");

    internal void SetContext(BadUnitTestContextBuilder console)
    {
        m_Console = console;
    }

    [BadMethod(description: "Loads a Test File into the Unit Test Context")]
    private void Load([BadParameter(description: "The file to be loaded.")] string file)
    {
        Console.Register(false, false, file);
    }

    [BadMethod(description: "Resets the Unit Test Context")]
    private void Reset()
    {
        Console.Reset();
    }

    [BadMethod(description: "Adds a Test to the Unit Test Context")]
    private void AddTest([BadParameter(description: "The Test Function")] BadFunction func,
                         [BadParameter(description: "The Unit Test Name")]
                         string name,
                         [BadParameter(description: "Specifies if the runtime is allowed to compile the Function")]
                         bool allowCompile = true)
    {
        Console.AddTest(func, name, allowCompile);
    }

    [BadMethod(description: "Adds a Setup Function to the Unit Test Context")]
    private void AddSetup([BadParameter(description: "The Setup Function")] BadFunction func)
    {
        Console.AddSetup(func);
    }

    private void AddTeardown([BadParameter(description: "The Teardown Function")] BadFunction func)
    {
        Console.AddTeardown(func);
    }
}