using BadScript2.IO;
using BadScript2.Optimizations.Folding;
using BadScript2.Optimizations.Substitution;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Builds a BadScript NUnit Test Context
/// </summary>
public class BadUnitTestContextBuilder
{
	/// <summary>
	///     The Test Cases
	/// </summary>
	private readonly List<BadNUnitTestCase> m_Cases = new List<BadNUnitTestCase>();

    private readonly BadRuntime m_Runtime;

    /// <summary>
    ///     The Setup Functions
    /// </summary>
    private readonly List<BadFunction> m_Setup = new List<BadFunction>();

    /// <summary>
    ///     The Teardown Functions
    /// </summary>
    private readonly List<BadFunction> m_Teardown = new List<BadFunction>();

    /// <summary>
    ///     Constructs a new BadUnitTestContextBuilder
    /// </summary>
    public BadUnitTestContextBuilder(BadRuntime runtime)
    {
        m_Runtime = runtime.Clone()
            .ConfigureContextOptions(
                opts =>
                {
                    opts.AddApi(new BadNUnitApi());
                    opts.AddApi(new BadNUnitConsoleApi(this));
                }
            );
    }


    /// <summary>
    ///     Registers one or multiple files to the Test Context
    /// </summary>
    /// <param name="optimize">Optimize the expressions?</param>
    /// <param name="files">The Source Files containing the test cases</param>
    public void Register(bool optimizeFolding, bool optimizeSubstitution, params string[] files)
    {
        foreach (string file in files)
        {
            SetupStage(file, optimizeFolding, optimizeSubstitution);
        }
    }

    /// <summary>
    ///     Creates a new BadUnitTestContext
    /// </summary>
    /// <returns>BadUnitTestContext</returns>
    public BadUnitTestContext CreateContext()
    {
        return new BadUnitTestContext(m_Cases.ToList(), m_Setup.ToList(), m_Teardown.ToList(), m_Runtime);
    }

    /// <summary>
    ///     Adds a Test Case to the Test Context
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
    ///     Adds a Setup Function to the Test Context
    /// </summary>
    /// <param name="function">The Setup Function</param>
    public void AddSetup(BadFunction function)
    {
        m_Setup.Add(function);
    }


    /// <summary>
    ///     Adds a Teardown Function to the Test Context
    /// </summary>
    /// <param name="function">The Teardown Function</param>
    public void AddTeardown(BadFunction function)
    {
        m_Teardown.Add(function);
    }

    /// <summary>
    ///     Resets the Builder state
    /// </summary>
    public void Reset()
    {
        m_Teardown.Clear();
        m_Setup.Clear();
        m_Cases.Clear();
    }


    /// <summary>
    ///     Runs the Setup Stage
    ///     Registering test cases and setup/teardown functions happens here
    /// </summary>
    /// <param name="file">The Source File</param>
    /// <param name="optimizeFolding">Optimize the Expressions?</param>
    private void SetupStage(string file, bool optimizeFolding = false, bool optimizeSubstitution = false)
    {
        //Load expressions
        IEnumerable<BadExpression> expressions = BadSourceParser.Create(file, BadFileSystem.ReadAllText(file)).Parse();

        if (optimizeFolding)
        {
            expressions = BadConstantFoldingOptimizer.Optimize(expressions);
        }

        if (optimizeSubstitution)
        {
            expressions = BadConstantSubstitutionOptimizer.Optimize(expressions);
        }

        m_Runtime.Execute(expressions);
    }
}