using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.NUnit;

/// <summary>
///     Implements the "NUnit" Api(Console Version)
/// </summary>
public class BadNUnitConsoleApi : BadInteropApi
{
	/// <summary>
	///     The Console Context
	/// </summary>
	private readonly BadUnitTestContextBuilder m_Console;

	/// <summary>
	///     Public Constructor
	/// </summary>
	/// <param name="console">The Console Context</param>
	public BadNUnitConsoleApi(BadUnitTestContextBuilder console) : base("NUnit")
    {
        m_Console = console;
    }

    protected override void LoadApi(BadTable target)
    {
        target.SetFunction<string>("Load", s => m_Console.Register(false, false, s));
        target.SetFunction("Reset", m_Console.Reset);
        target.SetProperty(
            "AddTest",
            new BadInteropFunction(
                "AddTest",
                (_, args) =>
                {
                    if (args[0] is not BadFunction func)
                    {
                        throw new ArgumentException("Expected Function");
                    }

                    bool allowCompile = true;

                    if (args.Length > 2)
                    {
                        if (args[2] is not IBadBoolean b)
                        {
                            throw new ArgumentException("Expected Boolean");
                        }

                        allowCompile = b.Value;
                    }

                    m_Console.AddTest(func, args[1], allowCompile);

                    return BadObject.Null;
                },
                false,
                BadAnyPrototype.Instance,
                "func",
                "name",
                new BadFunctionParameter("allowCompile", true, false, false)
            )
        );
        target.SetFunction<BadFunction>("AddSetup", m_Console.AddSetup);
        target.SetFunction<BadFunction>("AddTeardown", m_Console.AddTeardown);
    }
}