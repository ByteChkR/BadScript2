using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

public class BadUnitTestContext
{
    private readonly List<BadNUnitTestCase> m_Cases;
    private readonly List<BadFunction> m_Setup;
    private readonly List<BadFunction> m_Teardown;

    public BadUnitTestContext(List<BadNUnitTestCase> cases, List<BadFunction> setup, List<BadFunction> teardown)
    {
        m_Cases = cases;
        m_Setup = setup;
        m_Teardown = teardown;
    }

    private static void Run(IEnumerable<BadObject> enumerable)
    {
        foreach (BadObject _ in enumerable)
        {
            //Execute
        }
    }

    public void Setup()
    {
        Run(RunSetup());
    }

    public void Teardown()
    {
        Run(RunTeardown());
    }

    public void Run(BadNUnitTestCase test)
    {
        Run(RunTestCase(test));
    }

    public BadNUnitTestCase[] GetTestCases()
    {
        return m_Cases.ToArray();
    }

    public IEnumerable<BadObject> RunSetup()
    {
        for (int i = m_Setup.Count - 1; i >= 0; i--)
        {
            BadFunction function = m_Setup[i];
            BadExecutionContext caller = BadExecutionContext.Create();
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

    public IEnumerable<BadObject> RunTeardown()
    {
        for (int i = m_Teardown.Count - 1; i >= 0; i--)
        {
            BadFunction function = m_Teardown[i];
            BadExecutionContext caller = BadExecutionContext.Create();
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

    private IEnumerable<BadObject> RunTestCase(BadNUnitTestCase testCase)
    {
        TestContext.WriteLine($"Running test '{testCase.TestName}'");
        BadExecutionContext caller = BadExecutionContext.Create();

        foreach (BadObject o in testCase.Function.Invoke(Array.Empty<BadObject>(), caller))
        {
            yield return o;
        }

        if (caller.Scope.IsError)
        {
            throw new BadRuntimeErrorException(caller.Scope.Error);
        }
    }
}