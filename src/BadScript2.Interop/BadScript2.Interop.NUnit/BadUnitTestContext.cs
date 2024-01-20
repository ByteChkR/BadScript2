using BadScript2.Interop.Common.Task;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Implements a BadScript NUnit Test Context
/// </summary>
public class BadUnitTestContext
{
	/// <summary>
	///     The Test Cases
	/// </summary>
	private readonly List<BadNUnitTestCase> m_Cases;

    private readonly BadRuntime m_Runtime;

    /// <summary>
    ///     The Setup Functions
    /// </summary>
    private readonly List<BadFunction> m_Setup;

    /// <summary>
    ///     The Teardown Functions
    /// </summary>
    private readonly List<BadFunction> m_Teardown;

    /// <summary>
    ///     Constructs a new BadUnitTestContext
    /// </summary>
    /// <param name="cases">The Test Cases</param>
    /// <param name="setup">The Setup Functions</param>
    /// <param name="teardown">The Teardown Functions</param>
    /// <param name="runtime">The Runtime</param>
    public BadUnitTestContext(
        List<BadNUnitTestCase> cases,
        List<BadFunction> setup,
        List<BadFunction> teardown,
        BadRuntime runtime)
    {
        m_Cases = cases;
        m_Setup = setup;
        m_Teardown = teardown;
        m_Runtime = runtime;
    }

    /// <summary>
    ///     Runs an enumeration
    /// </summary>
    /// <param name="enumerable">The Enumeration</param>
    private static void Run(IEnumerable<BadObject> enumerable)
    {
        foreach (BadObject _ in enumerable)
        {
            //Execute
        }
    }

    /// <summary>
    ///     Runs all Setup Functions
    /// </summary>
    public void Setup()
    {
        Run(RunSetup());
    }

    /// <summary>
    ///     Runs all Teardown Functions
    /// </summary>
    public void Teardown()
    {
        Run(RunTeardown());
    }

    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="test"></param>
    public void Run(BadNUnitTestCase test)
    {
        Run(RunTestCase(test));
    }

    /// <summary>
    ///     Returns all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    public BadNUnitTestCase[] GetTestCases()
    {
        return m_Cases.ToArray();
    }

    /// <summary>
    ///     Runs all Setup Functions
    /// </summary>
    /// <returns>Runtime Objects</returns>
    /// <exception cref="BadRuntimeErrorException">Gets raised if the setup function failed</exception>
    public IEnumerable<BadObject> RunSetup()
    {
        for (int i = m_Setup.Count - 1; i >= 0; i--)
        {
            BadFunction function = m_Setup[i];
            BadExecutionContext caller = m_Runtime.CreateContext();

            foreach (BadObject o in function.Invoke(Array.Empty<BadObject>(), caller))
            {
                yield return o;
            }

            if (caller.Scope.IsError)
            {
                throw new BadRuntimeErrorException(caller.Scope.Error);
            }
        }
    }

    /// <summary>
    ///     Runs all Teardown Functions
    /// </summary>
    /// <returns>Runtime Objects</returns>
    /// <exception cref="BadRuntimeErrorException">Gets raised if the Teardown function failed</exception>
    public IEnumerable<BadObject> RunTeardown()
    {
        for (int i = m_Teardown.Count - 1; i >= 0; i--)
        {
            BadFunction function = m_Teardown[i];
            BadExecutionContext caller = m_Runtime.CreateContext();

            foreach (BadObject o in function.Invoke(Array.Empty<BadObject>(), caller))
            {
                yield return o;
            }

            if (caller.Scope.IsError)
            {
                throw new BadRuntimeErrorException(caller.Scope.Error);
            }
        }
    }

    /// <summary>
    ///     Runs a Testcase
    /// </summary>
    /// <param name="testCase">The test case</param>
    /// <returns>Runtime Objects</returns>
    /// <exception cref="BadRuntimeErrorException">Gets raised if the testcase failed</exception>
    private IEnumerable<BadObject> RunTestCase(BadNUnitTestCase testCase)
    {
        TestContext.WriteLine($"Running test '{testCase.TestName}'");
        BadExecutionContext caller = m_Runtime.CreateContext();

        BadTaskRunner.Instance.AddTask(
            new BadTask(
                new BadInteropRunnable(testCase.Function!.Invoke(Array.Empty<BadObject>(), caller).GetEnumerator()),
                testCase.TestName
            ),
            true
        );

        while (!BadTaskRunner.Instance.IsIdle)
        {
            BadTaskRunner.Instance.RunStep();

            yield return BadObject.Null;
        }

        if (caller.Scope.IsError)
        {
            throw new BadRuntimeErrorException(caller.Scope.Error);
        }
    }
}