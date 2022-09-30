using BadScript2.ConsoleAbstraction;
using BadScript2.Debugger.Scriptable;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Interop.NUnit;
using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;
using BadScript2.Settings;

namespace BadScript2.Tests
{
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
                BadSettingsProvider.RootSettings.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching").SetValue(true);
                BadNativeClassBuilder.AddNative(BadTask.Prototype);
                BadCommonInterop.AddExtensions();
                BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
                BadInteropExtension.AddExtension<BadLinqExtensions>();


                List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

                //apis.AddRange(BadCommonInterop.Apis);
                apis.Add(new BadIOApi());
                apis.Add(new BadJsonApi());
                apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

                BadFileSystem.Instance.CreateDirectory(TestDirectory);
                BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

                string[] files = BadFileSystem.Instance.GetFiles(
                        TestDirectory,
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

        private static BadUnitTestContext OptimizedContext
        {
            get
            {
                if (s_OptimizedContext != null)
                {
                    return s_OptimizedContext;
                }

                BadSettingsProvider.SetRootSettings(new BadSettings());
                BadSettingsProvider.RootSettings.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching").SetValue(true);
                BadNativeClassBuilder.AddNative(BadTask.Prototype);
                BadCommonInterop.AddExtensions();
                BadInteropExtension.AddExtension<BadLinqExtensions>();
                BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();

                List<BadInteropApi> apis = new List<BadInteropApi>(BadCommonInterop.Apis);

                //apis.AddRange(BadCommonInterop.Apis);
                apis.Add(new BadIOApi());
                apis.Add(new BadJsonApi());
                apis.Add(new BadTaskRunnerApi(BadTaskRunner.Instance));

                BadFileSystem.Instance.CreateDirectory(TestDirectory);
                BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(apis);

                string[] files = BadFileSystem.Instance.GetFiles(
                        TestDirectory,
                        $".{BadRuntimeSettings.Instance.FileExtension}",
                        true
                    )
                    .ToArray();
                BadConsole.WriteLine($"Loading Files...({files.Length})");
                builder.Register(true, false, files);

                s_OptimizedContext = builder.CreateContext();

                return s_OptimizedContext;
            }
        }

        [SetUp]
        public void Setup()
        {
            Context.Setup();
            OptimizedContext.Setup();
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
            OptimizedContext.Teardown();
        }
    }
}