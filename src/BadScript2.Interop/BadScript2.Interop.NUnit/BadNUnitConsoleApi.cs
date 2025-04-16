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

    /// <summary>
    /// The Console Context
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets thrown if the Console Context is not set</exception>
    private BadUnitTestContextBuilder Console => m_Console ?? throw new BadRuntimeException("Console Context not set");

    /// <summary>
    /// Sets the Console Context
    /// </summary>
    /// <param name="console">The Console Context</param>
    internal void SetContext(BadUnitTestContextBuilder console)
    {
        m_Console = console;
    }

    /// <summary>
    /// Loads a Test File into the Unit Test Context
    /// </summary>
    /// <param name="file">The file to be loaded</param>
    [BadMethod(description: "Loads a Test File into the Unit Test Context")]
    private void Load([BadParameter(description: "The file to be loaded.")] string file)
    {
        Console.Register(false, false, file);
    }

    /// <summary>
    /// Resets the Unit Test Context
    /// </summary>
    [BadMethod(description: "Resets the Unit Test Context")]
    private void Reset()
    {
        Console.Reset();
    }

    /// <summary>
    /// Adds a Test to the Unit Test Context
    /// </summary>
    /// <param name="func">The Test Function</param>
    /// <param name="name">The Unit Test Name</param>
    /// <param name="allowCompile">Specifies if the runtime is allowed to compile the Function</param>
    [BadMethod(description: "Adds a Test to the Unit Test Context")]
    private void AddTest([BadParameter(description: "The Test Function")] BadFunction func,
                         [BadParameter(description: "The Unit Test Name")]
                         string name,
                         [BadParameter(description: "Specifies if the runtime is allowed to compile the Function")]
                         bool allowCompile = true)
    {
        Console.AddTest(func, name, allowCompile);
    }

    /// <summary>
    /// Adds a Setup Function to the Unit Test Context
    /// </summary>
    /// <param name="func">The Setup Function</param>
    [BadMethod(description: "Adds a Setup Function to the Unit Test Context")]
    private void AddSetup([BadParameter(description: "The Setup Function")] BadFunction func)
    {
        Console.AddSetup(func);
    }

    /// <summary>
    /// Adds a Teardown Function to the Unit Test Context
    /// </summary>
    /// <param name="func"></param>
    [BadMethod(description: "Adds a Teardown Function to the Unit Test Context")]
    private void AddTeardown([BadParameter(description: "The Teardown Function")] BadFunction func)
    {
        Console.AddTeardown(func);
    }
}