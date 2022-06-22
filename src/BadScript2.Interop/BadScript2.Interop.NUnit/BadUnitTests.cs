using BadScript2.Interop.Common;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Settings;
using BadScript2.Settings;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

public class BadUnitTests
{
    private static BadUnitTestContext? s_Context;

    private static string TestDirectory =>
        BadSettingsProvider.RootSettings.FindProperty<string>("Subsystems.Test.TestDirectory") ??
        throw new BadRuntimeException("Test directory not found");

    private static BadUnitTestContext Context
    {
        get
        {
            if (s_Context != null)
            {
                return s_Context;
            }

            Directory.CreateDirectory(TestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(BadCommonInterop.Apis);

            string[] files = Directory.GetFiles(TestDirectory, $"*.{BadRuntimeSettings.Instance.DefaultExtension}", SearchOption.AllDirectories);
            Console.WriteLine($"Loading Files...({files.Length})");
            builder.Register(files);

            s_Context = builder.CreateContext();

            return s_Context;
        }
    }

    [SetUp]
    public void Setup()
    {
        Context.Setup();
    }

    public static BadNUnitTestCase[] GetTestCases()
    {
        return Context?.GetTestCases() ?? throw new BadRuntimeException("Context is null");
    }

    [TestCaseSource(nameof(GetTestCases))]
    public void Test(BadNUnitTestCase testCase)
    {
        Context.Run(testCase);
    }

    [TearDown]
    public void TearDown()
    {
        Context.Teardown();
    }
}