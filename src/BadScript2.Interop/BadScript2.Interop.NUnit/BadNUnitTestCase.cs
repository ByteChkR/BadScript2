using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.NUnit;

public class BadNUnitTestCase
{
    public BadNUnitTestCase(BadFunction function, string? testName, bool allowCompile)
    {
        Function = function;
        TestName = testName ?? Function.ToString();
        AllowCompile = allowCompile;
    }

    public string TestName { get; }
    public BadFunction? Function { get; }

    public bool AllowCompile { get; }

    public override string ToString()
    {
        return TestName;
    }
}