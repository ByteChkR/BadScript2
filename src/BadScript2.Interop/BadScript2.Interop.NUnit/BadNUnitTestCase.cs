using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.NUnit;

public class BadNUnitTestCase
{
    public BadNUnitTestCase(BadFunction function, string? testName)
    {
        Function = function;
        TestName = testName ?? Function.ToString();
    }

    public string TestName { get; }
    public BadFunction Function { get; }

    public override string ToString()
    {
        return TestName;
    }
}