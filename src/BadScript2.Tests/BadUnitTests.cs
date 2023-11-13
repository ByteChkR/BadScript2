using BadHtml;

using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Interop.NUnit;
using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Settings;

using HtmlAgilityPack;

namespace BadScript2.Tests;

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

			BadRuntime runtime = new BadRuntime()
				.UseCommonInterop()
				.UseLinqApi()
				.UseFileSystemApi()
				.UseJsonApi();

			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);


			BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
			BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadRuntime runtime = new BadRuntime()
				.UseCommonInterop()
				.UseLinqApi()
				.UseFileSystemApi()
				.UseJsonApi();

			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);


			BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
			BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadRuntime runtime = new BadRuntime()
				.UseCommonInterop()
				.UseLinqApi()
				.UseFileSystemApi()
				.UseJsonApi();

			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);


			BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
			BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadRuntime runtime = new BadRuntime()
				.UseCommonInterop()
				.UseLinqApi()
				.UseFileSystemApi()
				.UseJsonApi();

			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);


			BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
			BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadRuntime runtime = new BadRuntime()
				.UseCommonInterop()
				.UseLinqApi()
				.UseFileSystemApi()
				.UseJsonApi();

			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);


			BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
			BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadRuntime runtime = new BadRuntime()
				.UseCommonInterop()
				.UseLinqApi()
				.UseFileSystemApi()
				.UseJsonApi();

			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);


			BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
			BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadRuntime runtime = new BadRuntime()
				.UseCommonInterop()
				.UseLinqApi()
				.UseFileSystemApi()
				.UseJsonApi();

			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);


			BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
			BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

			BadRuntime runtime = new BadRuntime()
				.UseCommonInterop()
				.UseLinqApi()
				.UseFileSystemApi()
				.UseJsonApi();

			BadSettingsProvider.RootSettings
				.FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
				.SetValue(true);


			BadFileSystem.Instance.CreateDirectory(ScriptTestDirectory);
			BadUnitTestContextBuilder builder = new BadUnitTestContextBuilder(runtime);

			string[] files = BadFileSystem.Instance.GetFiles(ScriptTestDirectory,
					$".{BadRuntimeSettings.Instance.FileExtension}",
					true)
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

	public static BadNUnitTestCase[] GetCompiledOptimizedFoldingTestCases()
	{
		if (CompiledOptimizedFoldingContext == null)
		{
			throw new BadRuntimeException("CompiledOptimizedFoldingContext is null");
		}


		return CompiledOptimizedFoldingContext.GetTestCases()
			.Where(x => x.AllowCompile)
			.Select(x =>
			{
				BadExpressionFunction func = (BadExpressionFunction)x.Function!;
				BadCompiledFunction compiled = BadCompilerApi.CompileFunction(BadCompiler.Instance, func, true);

				return new BadNUnitTestCase(compiled, x.TestName, true);
			})
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
			.Select(x =>
			{
				BadExpressionFunction func = (BadExpressionFunction)x.Function!;
				BadCompiledFunction compiled = BadCompilerApi.CompileFunction(BadCompiler.Instance, func, true);

				return new BadNUnitTestCase(compiled, x.TestName, true);
			})
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
		OptimizedFoldingContext.Teardown();
		OptimizedSubstitutionContext.Teardown();
		OptimizedContext.Teardown();
		CompiledContext.Teardown();
		CompiledOptimizedFoldingContext.Teardown();
		CompiledOptimizedSubstitutionContext.Teardown();
		CompiledOptimizedContext.Teardown();
	}
}
