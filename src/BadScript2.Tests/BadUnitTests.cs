using BadScript2.ConsoleCore.Debugging.Scriptable;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.NUnit;
using BadScript2.IO;
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
    private static BadUnitTestContext? s_OptimizedContext;

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

            BadFileSystem.Instance.CreateDirectory(TestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(BadCommonInterop.Apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    TestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            System.Console.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, files);

            s_Context = builder.CreateContext();

            return s_Context;
        }
    }

    private static BadUnitTestContext OptimizedContext
    {
        get
        {
            if (s_OptimizedContext != null)
            {
                return s_OptimizedContext;
            }

            BadSettingsProvider.SetRootSettings(new BadSettings());
            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            BadExecutionContextOptions.Default.Apis.AddRange(BadCommonInterop.Apis);
            BadExecutionContextOptions.Default.Apis.Add(new BadIOApi());
            BadExecutionContextOptions.Default.Apis.Add(new BadJsonApi());

            BadFileSystem.Instance.CreateDirectory(TestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(BadCommonInterop.Apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    TestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            System.Console.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, files);

            s_OptimizedContext = builder.CreateContext();

            return s_OptimizedContext;
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

    public static BadNUnitTestCase[] GetOptimizedTestCases()
    {
        return OptimizedContext?.GetTestCases() ?? throw new BadRuntimeException("OptimizedContext is null");
    }

    [TestCaseSource(nameof(GetTestCases))]
    public void Test(BadNUnitTestCase testCase)
    {
        Context.Run(testCase);
    }

    [TestCaseSource(nameof(GetOptimizedTestCases))]
    public void TestOptimized(BadNUnitTestCase testCase)
    {
        OptimizedContext.Run(testCase);
    }


    [TearDown]
    public void TearDown()
    {
        Context.Teardown();
    }
}