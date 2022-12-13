using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.NUnit;

public class BadNUnitConsoleApi : BadInteropApi
{
    private readonly BadUnitTestContextBuilder m_Console;

    public BadNUnitConsoleApi(BadUnitTestContextBuilder console) : base("NUnit")
    {
        m_Console = console;
    }

    protected override void LoadApi(BadTable target)
    {
        target.SetFunction<string>("Load", s => m_Console.Register(false, s));
        target.SetFunction("Reset", m_Console.Reset);
        target.SetProperty(
            "AddTest",
            new BadInteropFunction(
                "AddTest",
                (context, args) =>
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
                "func",
                "name",
                new BadFunctionParameter("allowCompile", true, false, false)
            )
        );
        target.SetFunction<BadFunction>("AddSetup", m_Console.AddSetup);
        target.SetFunction<BadFunction>("AddTeardown", m_Console.AddTeardown);
    }
}