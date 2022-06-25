using BadScript2.Console.Debugging.Scriptable;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.NUnit;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;
using BadScript2.Settings;

namespace BadScript2.Tests;

public class BadUnitTests
{
    private static BadUnitTestContext? s_Context;

    private static string TestDirectory => Path.Combine(TestContext.CurrentContext.TestDirectory, "tests");

    private static BadUnitTestContext Context
    {
        get
        {
            if (s_Context != null)
            {
                return s_Context;
            }

            BadSettingsProvider.SetRootSettings(new BadSettings());
            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            BadExecutionContextOptions.Default.Apis.AddRange(BadCommonInterop.Apis);
            BadExecutionContextOptions.Default.Apis.Add(new BadIOApi());
            BadExecutionContextOptions.Default.Apis.Add(new BadJsonApi());
            
            Directory.CreateDirectory(TestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(BadCommonInterop.Apis);

            string[] files = Directory.GetFiles(
                TestDirectory,
                $"*.{BadRuntimeSettings.Instance.FileExtension}",
                SearchOption.AllDirectories
            );
            System.Console.WriteLine($"Loading Files...({files.Length})");
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