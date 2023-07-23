using BadHtml;

using BadScript2.ConsoleAbstraction;
using BadScript2.Debugger.Scriptable;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.Common.Versioning;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Interop.NUnit;
using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

using HtmlAgilityPack;

namespace BadScript2.Tests;

public class BadUnitTests
{
	private static BadUnitTestContext? s_Context;
	private static BadUnitTestContext? s_OptimizedContext;
	private static BadUnitTestContext? s_CompiledContext;
	private static BadUnitTestContext? s_CompiledOptimizedContext;


	private static string TestDirectory => Path.Combine(TestContext.CurrentContext.TestDirectory, "tests");

	private static string ScriptTestDirectory => Path.Combine(TestDirectory, "basic");

	private static string HtmlTestDirectory => Path.Combine(TestDirectory, "html");

	private static BadUnitTestContext Context
	{
		get
		{
			if (s_Context != null)
			{
				return s_Context;
			}

			BadSettingsProvider.SetRootSettings(new BadSettings());
			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);
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

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadSettingsProvider.SetRootSettings(new BadSettings());
			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);
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

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadSettingsProvider.SetRootSettings(new BadSettings());
			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);
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

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadSettingsProvider.SetRootSettings(new BadSettings());
			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);
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

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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
		Context.Setup();
		OptimizedContext.Setup();
		CompiledContext.Setup();
		CompiledOptimizedContext.Setup();

		//Required to get the raw Assert.Pass exception instead of the runtime exception that is caught
		BadRuntimeSettings.Instance.CatchRuntimeExceptions = false;
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
			.Select(x =>
			{
				BadExpressionFunction func = (BadExpressionFunction)x.Function!;
				BadCompiledFunction compiled = BadCompilerApi.CompileFunction(BadCompiler.Instance, func, true);

				return new BadNUnitTestCase(compiled, x.TestName, true);
			})
			.ToArray();
	}

	public static string[] GetHtmlTemplateFiles()
	{
		return BadFileSystem.Instance.GetFiles(HtmlTestDirectory, ".bhtml", true).ToArray();
	}

	public static BadNUnitTestCase[] GetCompiledOptimizedTestCases()
	{
		if (CompiledOptimizedContext == null)
		{
			throw new BadRuntimeException("CompiledOptimizedContext is null");
		}


		return CompiledOptimizedContext.GetTestCases()
			.Where(x => x.AllowCompile)
			.Select(x =>
			{
				BadExpressionFunction func = (BadExpressionFunction)x.Function!;
				BadCompiledFunction compiled = BadCompilerApi.CompileFunction(BadCompiler.Instance, func, true);

				return new BadNUnitTestCase(compiled, x.TestName, true);
			})
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

	private HtmlDocument LoadReference(string file)
	{
		HtmlDocument reference = new HtmlDocument();
		reference.Load(file);

		return reference;
	}

	[Test]
	[TestCaseSource(nameof(GetHtmlTemplateFiles))]
	public void TestHtmlTemplateDefault(string file)
	{
		BadHtmlTemplateOptions options = new BadHtmlTemplateOptions();
		string referenceFile = Path.ChangeExtension(file, $".{(options.SkipEmptyTextNodes ? "skip" : "default")}.html");
		HtmlDocument result = BadHtmlTemplate.Create(file).RunTemplate(null, options);

		if (BadFileSystem.Instance.Exists(referenceFile))
		{
			HtmlDocument reference = LoadReference(referenceFile);
			Assert.That(result.DocumentNode.OuterHtml, Is.EqualTo(reference.DocumentNode.OuterHtml));
		}
		else
		{
			BadFileSystem.Instance.CreateDirectory(Path.GetDirectoryName(referenceFile)!, true);
			result.Save(referenceFile);
			Assert.Inconclusive("Reference file does not exist. Saving...");
		}
	}

	[Test]
	[TestCaseSource(nameof(GetHtmlTemplateFiles))]
	public void TestHtmlTemplateSkipEmpty(string file)
	{
		BadHtmlTemplateOptions options = new BadHtmlTemplateOptions
		{
			SkipEmptyTextNodes = true
		};
		string referenceFile = Path.ChangeExtension(file, $".{(options.SkipEmptyTextNodes ? "skip" : "default")}.html");
		HtmlDocument result = BadHtmlTemplate.Create(file).RunTemplate(null, options);

		if (BadFileSystem.Instance.Exists(referenceFile))
		{
			HtmlDocument reference = LoadReference(referenceFile);
			Assert.That(result.DocumentNode.OuterHtml, Is.EqualTo(reference.DocumentNode.OuterHtml));
		}
		else
		{
			BadFileSystem.Instance.CreateDirectory(Path.GetDirectoryName(referenceFile)!, true);
			result.Save(referenceFile);
			Assert.Inconclusive("Reference file does not exist. Saving...");
		}
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
