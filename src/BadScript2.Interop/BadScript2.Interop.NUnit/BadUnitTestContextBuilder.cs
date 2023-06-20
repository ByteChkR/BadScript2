using BadScript2.IO;
using BadScript2.Optimizations;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.NUnit;

/// <summary>
/// Builds a BadScript NUnit Test Context
/// </summary>
public class BadUnitTestContextBuilder
{
	/// <summary>
	/// The Interop Apis that are available to the Test Context
	/// </summary>
	private readonly List<BadInteropApi> m_Apis;
	/// <summary>
	/// The Test Cases
	/// </summary>
	private readonly List<BadNUnitTestCase> m_Cases = new List<BadNUnitTestCase>();
	
	/// <summary>
	/// The Setup Functions
	/// </summary>
	private readonly List<BadFunction> m_Setup = new List<BadFunction>();
	
	/// <summary>
	/// The Teardown Functions
	/// </summary>
	private readonly List<BadFunction> m_Teardown = new List<BadFunction>();

	/// <summary>
	/// Constructs a new BadUnitTestContextBuilder
	/// </summary>
	/// <param name="apis">The Interop Apis that are available to the Test Context</param>
	public BadUnitTestContextBuilder(IEnumerable<BadInteropApi> apis)
	{
		m_Apis = apis.ToList();
		m_Apis.Add(new BadNUnitApi());
		m_Apis.Add(new BadNUnitConsoleApi(this));
	}
	/// <summary>
	/// Constructs a new BadUnitTestContextBuilder
	/// </summary>
	/// <param name="apis">The Interop Apis that are available to the Test Context</param>
	public BadUnitTestContextBuilder(params BadInteropApi[] apis) : this((IEnumerable<BadInteropApi>)apis) { }

	/// <summary>
	/// Registers one or multiple files to the Test Context
	/// </summary>
	/// <param name="optimize">Optimize the expressions?</param>
	/// <param name="files">The Source Files containing the test cases</param>
	public void Register(bool optimize, params string[] files)
	{
		foreach (string file in files)
		{
			SetupStage(file, optimize);
		}
	}

	/// <summary>
	/// Creates a new BadUnitTestContext
	/// </summary>
	/// <returns>BadUnitTestContext</returns>
	public BadUnitTestContext CreateContext()
	{
		return new BadUnitTestContext(m_Cases.ToList(), m_Setup.ToList(), m_Teardown.ToList());
	}

	/// <summary>
	/// Adds a Test Case to the Test Context
	/// </summary>
	/// <param name="function">The Testcase Function</param>
	/// <param name="testName">The Test Name</param>
	/// <param name="allowCompile">Allow compilation of the function?</param>
	/// <exception cref="InvalidOperationException">Gets raised if the testName is not a string</exception>
	public void AddTest(BadFunction function, BadObject testName, bool allowCompile = true)
	{
		string? name = null;

		if (testName != BadObject.Null)
		{
			name = (testName as IBadString)?.Value ?? throw new InvalidOperationException("Test name must be a string");
		}

		m_Cases.Add(new BadNUnitTestCase(function, name, allowCompile));
	}

	/// <summary>
	/// Adds a Setup Function to the Test Context
	/// </summary>
	/// <param name="function">The Setup Function</param>
	public void AddSetup(BadFunction function)
	{
		m_Setup.Add(function);
	}


	/// <summary>
	/// Adds a Teardown Function to the Test Context
	/// </summary>
	/// <param name="function">The Teardown Function</param>
	public void AddTeardown(BadFunction function)
	{
		m_Teardown.Add(function);
	}
	
	/// <summary>
	/// Resets the Builder state
	/// </summary>
	public void Reset()
	{
		m_Teardown.Clear();
		m_Setup.Clear();
		m_Cases.Clear();
	}


	/// <summary>
	/// Loads the Interop Apis into the Context
	/// </summary>
	/// <param name="context">The Context</param>
	private void LoadApis(BadExecutionContext context)
	{
		foreach (BadInteropApi api in m_Apis)
		{
			BadTable target;

			if (context.Scope.HasLocal(api.Name) && context.Scope.GetVariable(api.Name).Dereference() is BadTable table)
			{
				target = table;
			}
			else
			{
				target = new BadTable();
				context.Scope.DefineVariable(api.Name, target);
			}

			api.Load(target);
		}

		foreach (BadClassPrototype type in BadNativeClassBuilder.NativeTypes)
		{
			context.Scope.DefineVariable(type.Name, type);
		}
	}

	/// <summary>
	/// Runs the Setup Stage
	/// Registering test cases and setup/teardown functions happens here
	/// </summary>
	/// <param name="file">The Source File</param>
	/// <param name="optimize">Optimize the Expressions?</param>
	private void SetupStage(string file, bool optimize = false)
	{
		//Load expressions
		IEnumerable<BadExpression> expressions = BadSourceParser.Create(file, BadFileSystem.ReadAllText(file)).Parse();

		if (optimize)
		{
			expressions = BadExpressionOptimizer.Optimize(expressions);
		}

		//Create Context
		BadExecutionContext context = BadExecutionContext.Create();

		//Add Apis
		LoadApis(context);

		context.Run(expressions);
	}
}
