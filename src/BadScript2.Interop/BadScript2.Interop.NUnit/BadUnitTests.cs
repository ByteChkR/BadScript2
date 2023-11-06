using BadScript2.ConsoleAbstraction;
using BadScript2.Debugger.Scriptable;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.Common.Versioning;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

public class BadUnitTests
{
    private static BadUnitTestContext? s_Context;
    private static BadUnitTestContext? s_OptimizedFoldingContext;
    private static BadUnitTestContext? s_OptimizedSubstitutionContext;
    private static BadUnitTestContext? s_OptimizedContext;
    private static BadUnitTestContext? s_CompiledContext;
    private static BadUnitTestContext? s_CompiledOptimizedFoldingContext;
    private static BadUnitTestContext? s_CompiledOptimizedSubstitutionContext;
    private static BadUnitTestContext? s_CompiledOptimizedContext;


    private static string ScriptTestDirectory =>
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

            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
            BadInteropExtension.AddExtension<BadLinqExtensions>();


            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

            apis.Add(new BadIOApi());
            apis.Add(new BadJsonApi());
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, false, files);

            s_Context = builder.CreateContext();

            return s_Context;
        }
    }

    private static BadUnitTestContext OptimizedFoldingContext
    {
        get
        {
            if (s_OptimizedFoldingContext != null)
            {
                return s_OptimizedFoldingContext;
            }

            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadLinqExtensions>();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

            apis.Add(new BadIOApi());
            apis.Add(new BadJsonApi());
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, false, files);

            s_OptimizedFoldingContext = builder.CreateContext();

            return s_OptimizedFoldingContext;
        }
    }

    private static BadUnitTestContext OptimizedSubstitutionContext
    {
        get
        {
            if (s_OptimizedSubstitutionContext != null)
            {
                return s_OptimizedSubstitutionContext;
            }

            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadLinqExtensions>();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

            apis.Add(new BadIOApi());
            apis.Add(new BadJsonApi());
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, true, files);

            s_OptimizedSubstitutionContext = builder.CreateContext();

            return s_OptimizedSubstitutionContext;
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

            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadLinqExtensions>();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

            apis.Add(new BadIOApi());
            apis.Add(new BadJsonApi());
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, true, files);

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

            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadLinqExtensions>();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

            apis.Add(new BadIOApi());
            apis.Add(new BadJsonApi());
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, false, files);

            s_CompiledContext = builder.CreateContext();

            return s_CompiledContext;
        }
    }

    private static BadUnitTestContext CompiledOptimizedFoldingContext
    {
        get
        {
            if (s_CompiledOptimizedFoldingContext != null)
            {
                return s_CompiledOptimizedFoldingContext;
            }

            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadLinqExtensions>();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

            apis.Add(new BadIOApi());
            apis.Add(new BadJsonApi());
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, false, files);

            s_CompiledOptimizedFoldingContext = builder.CreateContext();

            return s_CompiledOptimizedFoldingContext;
        }
    }


    private static BadUnitTestContext CompiledOptimizedSubstitutionContext
    {
        get
        {
            if (s_CompiledOptimizedSubstitutionContext != null)
            {
                return s_CompiledOptimizedSubstitutionContext;
            }

            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadLinqExtensions>();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

            apis.Add(new BadIOApi());
            apis.Add(new BadJsonApi());
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, true, files);

            s_CompiledOptimizedSubstitutionContext = builder.CreateContext();

            return s_CompiledOptimizedSubstitutionContext;
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

            BadNativeClassBuilder.AddNative(BadTask.Prototype);
            BadNativeClassBuilder.AddNative(BadVersion.Prototype);
            BadCommonInterop.AddExtensions();
            BadInteropExtension.AddExtension<BadLinqExtensions>();
            BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

            List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

            apis.Add(new BadIOApi());
            apis.Add(new BadJsonApi());
            apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, true, files);

            s_CompiledOptimizedContext = builder.CreateContext();

            return s_CompiledOptimizedContext;
        }
    }


    [SetUp]
    public void Setup()
    {
        Context.Setup();
        OptimizedFoldingContext.Setup();
        OptimizedSubstitutionContext.Setup();
        OptimizedContext.Setup();
        CompiledContext.Setup();
        CompiledOptimizedFoldingContext.Setup();
        CompiledOptimizedSubstitutionContext.Setup();
        CompiledOptimizedContext.Setup();

        //Required to get the raw Assert.Pass exception instead of the runtime exception that is caught
        BadRuntimeSettings.Instance.CatchRuntimeExceptions = false;
    }

    public static BadNUnitTestCase[] GetTestCases()
    {
        return Context?.GetTestCases() ?? throw new BadRuntimeException("Context is null");
    }


    public static BadNUnitTestCase[] GetOptimizedFoldingTestCases()
    {
        return OptimizedFoldingContext?.GetTestCases() ??
               throw new BadRuntimeException("OptimizedFoldingContext is null");
    }

    public static BadNUnitTestCase[] GetOptimizedSubstitutionTestCases()
    {
        return OptimizedSubstitutionContext?.GetTestCases() ??
               throw new BadRuntimeException("OptimizedSubstitutionContext is null");
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

    public static BadNUnitTestCase[] GetCompiledOptimizedFoldingTestCases()
    {
        if (CompiledOptimizedFoldingContext == null)
        {
            throw new BadRuntimeException("CompiledOptimizedFoldingContext is null");
        }


        return CompiledOptimizedFoldingContext.GetTestCases()
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


    public static BadNUnitTestCase[] GetCompiledOptimizedSubstitutionTestCases()
    {
        if (CompiledOptimizedSubstitutionContext == null)
        {
            throw new BadRuntimeException("CompiledOptimizedSubstitutionContext is null");
        }


        return CompiledOptimizedSubstitutionContext.GetTestCases()
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

    [TestCaseSource(nameof(GetOptimizedFoldingTestCases))]
    public void TestOptimizedFolding(BadNUnitTestCase testCase)
    {
        OptimizedFoldingContext.Run(testCase);
    }

    [TestCaseSource(nameof(GetOptimizedSubstitutionTestCases))]
    public void TestOptimizedSubstitution(BadNUnitTestCase testCase)
    {
        OptimizedSubstitutionContext.Run(testCase);
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

    [TestCaseSource(nameof(GetCompiledOptimizedFoldingTestCases))]
    public void TestCompiledOptimizedFolding(BadNUnitTestCase testCase)
    {
        CompiledOptimizedFoldingContext.Run(testCase);
    }

    [TestCaseSource(nameof(GetCompiledOptimizedSubstitutionTestCases))]
    public void TestCompiledOptimizedSubstitution(BadNUnitTestCase testCase)
    {
        CompiledOptimizedFoldingContext.Run(testCase);
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
        OptimizedFoldingContext.Teardown();
        OptimizedSubstitutionContext.Teardown();
        OptimizedContext.Teardown();
        CompiledContext.Teardown();
        CompiledOptimizedFoldingContext.Teardown();
        CompiledOptimizedSubstitutionContext.Teardown();
        CompiledOptimizedContext.Teardown();
    }
}