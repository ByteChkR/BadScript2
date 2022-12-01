using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

public class BadUnitTests
{
    private static BadUnitTestContext? s_Context;
    private static BadUnitTestContext? s_OptimizedContext;
    private static BadUnitTestContext? s_CompiledContext;
    private static BadUnitTestContext? s_CompiledOptimizedContext;

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

            BadFileSystem.Instance.CreateDirectory(TestDirectory);
            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    TestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
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

            BadFileSystem.Instance.CreateDirectory(TestDirectory);
            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    TestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, files);

            s_OptimizedContext = builder.CreateContext();

            return s_OptimizedContext;
        }
    }
    
    private static BadUnitTestContext CompiledContext
    {
        get
        {
            if (s_CompiledContext != null)
            {
                return s_CompiledContext;
            }

            BadFileSystem.Instance.CreateDirectory(TestDirectory);
            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    TestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, files);

            s_CompiledContext = builder.CreateContext();

            return s_CompiledContext;
        }
    }

    private static BadUnitTestContext CompiledOptimizedContext
    {
        get
        {
            if (s_CompiledOptimizedContext != null)
            {
                return s_CompiledOptimizedContext;
            }

            BadFileSystem.Instance.CreateDirectory(TestDirectory);
            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    TestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, files);

            s_CompiledOptimizedContext = builder.CreateContext();

            return s_CompiledOptimizedContext;
        }
    }

    [SetUp]
    public void Setup()
    {
        //Required to get the raw Assert.Pass exception instead of the runtime exception that is caught
        BadRuntimeSettings.Instance.CatchRuntimeExceptions = false;
        Context.Setup();
        OptimizedContext.Setup();
        CompiledContext.Setup();
        CompiledOptimizedContext.Setup();
    }

    public static BadNUnitTestCase[] GetTestCases()
    {
        return Context?.GetTestCases() ?? throw new BadRuntimeException("Context is null");
    }


    public static BadNUnitTestCase[] GetOptimizedTestCases()
    {
        return OptimizedContext?.GetTestCases() ?? throw new BadRuntimeException("OptimizedContext is null");
    }

    public static BadNUnitTestCase[] GetCompiledTestCases()
    {
        if (CompiledContext == null)
        {
            throw new BadRuntimeException("CompiledContext is null");
        }


        return CompiledContext.GetTestCases()
            .Where(x => x.AllowCompile)
            .Select(
                x =>
                {
                    BadExpressionFunction func = (BadExpressionFunction)x.Function!;
                    BadCompiledFunction compiled = BadCompilerApi.CompileFunction(BadCompiler.Instance, func, true);

                    return new BadNUnitTestCase(compiled, x.TestName, true);
                }
            )
            .ToArray();
    }

    public static BadNUnitTestCase[] GetCompiledOptimizedTestCases()
    {
        if (CompiledOptimizedContext == null)
        {
            throw new BadRuntimeException("CompiledOptimizedContext is null");
        }


        return CompiledOptimizedContext.GetTestCases()
            .Where(x => x.AllowCompile)
            .Select(
                x =>
                {
                    BadExpressionFunction func = (BadExpressionFunction)x.Function!;
                    BadCompiledFunction compiled = BadCompilerApi.CompileFunction(BadCompiler.Instance, func, true);

                    return new BadNUnitTestCase(compiled, x.TestName, true);
                }
            )
            .ToArray();
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

    [TestCaseSource(nameof(GetCompiledTestCases))]
    public void TestCompiled(BadNUnitTestCase testCase)
    {
        CompiledContext.Run(testCase);
    }

    [TestCaseSource(nameof(GetCompiledOptimizedTestCases))]
    public void TestCompiledOptimized(BadNUnitTestCase testCase)
    {
        CompiledOptimizedContext.Run(testCase);
    }


    [TearDown]
    public void TearDown()
    {
        Context.Teardown();
        OptimizedContext.Teardown();
        CompiledContext.Teardown();
        CompiledOptimizedContext.Teardown();
    }
}