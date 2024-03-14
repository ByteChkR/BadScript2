using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

using NUnit.Framework;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Contians the BadScript NUnit Test Cases
/// </summary>
public class BadUnitTests
{
    /// <summary>
    ///     The Test Context
    /// </summary>
    private static BadUnitTestContext? s_Context;

    /// <summary>
    ///     The Optimized Folding Test Context
    /// </summary>
    private static BadUnitTestContext? s_OptimizedFoldingContext;

    /// <summary>
    ///     The Optimized Substitution Test Context
    /// </summary>
    private static BadUnitTestContext? s_OptimizedSubstitutionContext;

    /// <summary>
    ///     The Optimized Test Context
    /// </summary>
    private static BadUnitTestContext? s_OptimizedContext;

    /// <summary>
    ///     The Compiled Test Context
    /// </summary>
    private static BadUnitTestContext? s_CompiledContext;

    /// <summary>
    ///     The Compiled Optimized Folding Test Context
    /// </summary>
    private static BadUnitTestContext? s_CompiledOptimizedFoldingContext;

    /// <summary>
    ///     The Compiled Optimized Substitution Test Context
    /// </summary>
    private static BadUnitTestContext? s_CompiledOptimizedSubstitutionContext;

    /// <summary>
    ///     The Compiled Optimized Test Context
    /// </summary>
    private static BadUnitTestContext? s_CompiledOptimizedContext;


    /// <summary>
    ///     The Test Directory
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets raised if the test directory is not found</exception>
    private static string ScriptTestDirectory =>
        BadSettingsProvider.RootSettings.FindProperty<string>("Subsystems.Test.TestDirectory") ??
        throw new BadRuntimeException("Test directory not found");


    /// <summary>
    ///     The Test Context
    /// </summary>
    private static BadUnitTestContext Context
    {
        get
        {
            if (s_Context != null)
            {
                return s_Context;
            }

            BadRuntime runtime = new BadRuntime()
                .UseCommonInterop()
                .UseFileSystemApi()
                .UseJsonApi();

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);
            runtime.UseNUnitConsole(builder);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, false, files);

            s_Context = builder.CreateContext(ScriptTestDirectory);

            return s_Context;
        }
    }

    /// <summary>
    ///     The Optimized Folding Test Context
    /// </summary>
    private static BadUnitTestContext OptimizedFoldingContext
    {
        get
        {
            if (s_OptimizedFoldingContext != null)
            {
                return s_OptimizedFoldingContext;
            }

            BadRuntime runtime = new BadRuntime()
                .UseCommonInterop()
                .UseFileSystemApi()
                .UseJsonApi();

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);
            runtime.UseNUnitConsole(builder);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, false, files);

            s_OptimizedFoldingContext = builder.CreateContext(ScriptTestDirectory);

            return s_OptimizedFoldingContext;
        }
    }

    /// <summary>
    ///     The Optimized Substitution Test Context
    /// </summary>
    private static BadUnitTestContext OptimizedSubstitutionContext
    {
        get
        {
            if (s_OptimizedSubstitutionContext != null)
            {
                return s_OptimizedSubstitutionContext;
            }

            BadRuntime runtime = new BadRuntime()
                .UseCommonInterop()
                .UseFileSystemApi()
                .UseJsonApi();

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);
            runtime.UseNUnitConsole(builder);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, true, files);

            s_OptimizedSubstitutionContext = builder.CreateContext(ScriptTestDirectory);

            return s_OptimizedSubstitutionContext;
        }
    }

    /// <summary>
    ///     The Optimized Test Context
    /// </summary>
    private static BadUnitTestContext OptimizedContext
    {
        get
        {
            if (s_OptimizedContext != null)
            {
                return s_OptimizedContext;
            }

            BadRuntime runtime = new BadRuntime()
                .UseCommonInterop()
                .UseFileSystemApi()
                .UseJsonApi();

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);
            runtime.UseNUnitConsole(builder);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, true, files);

            s_OptimizedContext = builder.CreateContext(ScriptTestDirectory);

            return s_OptimizedContext;
        }
    }

    /// <summary>
    ///     The Compiled Test Context
    /// </summary>
    private static BadUnitTestContext CompiledContext
    {
        get
        {
            if (s_CompiledContext != null)
            {
                return s_CompiledContext;
            }

            BadRuntime runtime = new BadRuntime()
                .UseCommonInterop()
                .UseFileSystemApi()
                .UseJsonApi();

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);
            runtime.UseNUnitConsole(builder);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, false, files);

            s_CompiledContext = builder.CreateContext(ScriptTestDirectory);

            return s_CompiledContext;
        }
    }

    /// <summary>
    ///     The Compiled Optimized Folding Test Context
    /// </summary>
    private static BadUnitTestContext CompiledOptimizedFoldingContext
    {
        get
        {
            if (s_CompiledOptimizedFoldingContext != null)
            {
                return s_CompiledOptimizedFoldingContext;
            }

            BadRuntime runtime = new BadRuntime()
                .UseCommonInterop()
                .UseFileSystemApi()
                .UseJsonApi();

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);
            runtime.UseNUnitConsole(builder);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(true, false, files);

            s_CompiledOptimizedFoldingContext = builder.CreateContext(ScriptTestDirectory);

            return s_CompiledOptimizedFoldingContext;
        }
    }


    /// <summary>
    ///     The Compiled Optimized Substitution Test Context
    /// </summary>
    private static BadUnitTestContext CompiledOptimizedSubstitutionContext
    {
        get
        {
            if (s_CompiledOptimizedSubstitutionContext != null)
            {
                return s_CompiledOptimizedSubstitutionContext;
            }

            BadRuntime runtime = new BadRuntime()
                .UseCommonInterop()
                .UseFileSystemApi()
                .UseJsonApi();

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);
            runtime.UseNUnitConsole(builder);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, true, files);

            s_CompiledOptimizedSubstitutionContext = builder.CreateContext(ScriptTestDirectory);

            return s_CompiledOptimizedSubstitutionContext;
        }
    }


    /// <summary>
    ///     The Compiled Optimized Test Context
    /// </summary>
    private static BadUnitTestContext CompiledOptimizedContext
    {
        get
        {
            if (s_CompiledOptimizedContext != null)
            {
                return s_CompiledOptimizedContext;
            }

            BadRuntime runtime = new BadRuntime()
                .UseCommonInterop()
                .UseFileSystemApi()
                .UseJsonApi();

            BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
            BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);
            runtime.UseNUnitConsole(builder);

            string[] files = BadFileSystem.Instance.GetFiles(
                    ScriptTestDirectory,
                    $".{BadRuntimeSettings.Instance.FileExtension}",
                    true
                )
                .ToArray();
            BadConsole.WriteLine($"Loading Files...({files.Length})");
            builder.Register(false, true, files);

            s_CompiledOptimizedContext = builder.CreateContext(ScriptTestDirectory);

            return s_CompiledOptimizedContext;
        }
    }


    /// <summary>
    ///     Setup the Test Contexts
    /// </summary>
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

    /// <summary>
    ///     Gets all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Context is null</exception>
    public static BadNUnitTestCase[] GetTestCases()
    {
        return Context.GetTestCases() ?? throw new BadRuntimeException("Context is null");
    }

    /// <summary>
    ///     Gets all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Context is null</exception>
    public static BadNUnitTestCase[] GetOptimizedFoldingTestCases()
    {
        return OptimizedFoldingContext.GetTestCases() ??
               throw new BadRuntimeException("OptimizedFoldingContext is null");
    }

    /// <summary>
    ///     Gets all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Context is null</exception>
    public static BadNUnitTestCase[] GetOptimizedSubstitutionTestCases()
    {
        return OptimizedSubstitutionContext.GetTestCases() ??
               throw new BadRuntimeException("OptimizedSubstitutionContext is null");
    }

    /// <summary>
    ///     Gets all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Context is null</exception>
    public static BadNUnitTestCase[] GetOptimizedTestCases()
    {
        return OptimizedContext.GetTestCases() ?? throw new BadRuntimeException("OptimizedContext is null");
    }

    /// <summary>
    ///     Gets all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Context is null</exception>
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
                    BadCompiledFunction compiled = BadCompilerApi.CompileFunction(func, true);

                    return new BadNUnitTestCase(compiled, x.TestName, true);
                }
            )
            .ToArray();
    }

    /// <summary>
    ///     Gets all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Context is null</exception>
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
                    BadCompiledFunction compiled = BadCompilerApi.CompileFunction(func, true);

                    return new BadNUnitTestCase(compiled, x.TestName, true);
                }
            )
            .ToArray();
    }

    /// <summary>
    ///     Gets all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Context is null</exception>
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
                    BadCompiledFunction compiled = BadCompilerApi.CompileFunction(func, true);

                    return new BadNUnitTestCase(compiled, x.TestName, true);
                }
            )
            .ToArray();
    }

    /// <summary>
    ///     Gets all Test Cases
    /// </summary>
    /// <returns>Array of BadNUnitTestCase</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the Context is null</exception>
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
                    BadCompiledFunction compiled = BadCompilerApi.CompileFunction(func, true);

                    return new BadNUnitTestCase(compiled, x.TestName, true);
                }
            )
            .ToArray();
    }


    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="testCase">The Test Case</param>
    [TestCaseSource(nameof(GetTestCases))]
    public void Test(BadNUnitTestCase testCase)
    {
        Context.Run(testCase);
    }

    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="testCase">The Test Case</param>
    [TestCaseSource(nameof(GetOptimizedFoldingTestCases))]
    public void TestOptimizedFolding(BadNUnitTestCase testCase)
    {
        OptimizedFoldingContext.Run(testCase);
    }

    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="testCase">The Test Case</param>
    [TestCaseSource(nameof(GetOptimizedSubstitutionTestCases))]
    public void TestOptimizedSubstitution(BadNUnitTestCase testCase)
    {
        OptimizedSubstitutionContext.Run(testCase);
    }

    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="testCase">The Test Case</param>
    [TestCaseSource(nameof(GetOptimizedTestCases))]
    public void TestOptimized(BadNUnitTestCase testCase)
    {
        OptimizedContext.Run(testCase);
    }

    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="testCase">The Test Case</param>
    [TestCaseSource(nameof(GetCompiledTestCases))]
    public void TestCompiled(BadNUnitTestCase testCase)
    {
        CompiledContext.Run(testCase);
    }

    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="testCase">The Test Case</param>
    [TestCaseSource(nameof(GetCompiledOptimizedFoldingTestCases))]
    public void TestCompiledOptimizedFolding(BadNUnitTestCase testCase)
    {
        CompiledOptimizedFoldingContext.Run(testCase);
    }

    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="testCase">The Test Case</param>
    [TestCaseSource(nameof(GetCompiledOptimizedSubstitutionTestCases))]
    public void TestCompiledOptimizedSubstitution(BadNUnitTestCase testCase)
    {
        CompiledOptimizedFoldingContext.Run(testCase);
    }

    /// <summary>
    ///     Runs a Test Case
    /// </summary>
    /// <param name="testCase">The Test Case</param>
    [TestCaseSource(nameof(GetCompiledOptimizedTestCases))]
    public void TestCompiledOptimized(BadNUnitTestCase testCase)
    {
        CompiledOptimizedContext.Run(testCase);
    }


    /// <summary>
    ///     Teardown the Test Contexts
    /// </summary>
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