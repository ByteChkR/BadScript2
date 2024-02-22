using BadScript2.Interactive;
using BadScript2.Interop;
using BadScript2.Interop.Common;

namespace BadScript2.Generator.Test;

[BadInteropApi("TestApi")]
internal partial class Program
{
    private string m_Name = "World";

    [BadMethod(description: "Says Hello")]
    private void SayHello()
    {
        Console.WriteLine("Hello");
    }

    [BadMethod(description: "Returns the Greeting String")]
    [return: BadReturn("Hello {name}")]
    private string GetGreeting()
    {
        return $"Hello {m_Name}";
    }

    [BadMethod(description: "Sets the Name")]
    private void SetName([BadParameter(description: "The Name to be set.")] string name = "World")
    {
        m_Name = name;
    }

    [BadMethod(description: "Gets the Name")]
    [return: BadReturn("The Name")]
    private string GetName()
    {
        return m_Name;
    }

    [BadMethod(description: "Greets a list of users")]
    private void ParamsTest(params string[] names)
    {
        foreach (string name in names)
        {
            SetName(name);
            Greet();
        }
    }

    [BadMethod(description: "Greets a list of users and resets the name")]
    private void ParamsTest2(string resetName, params string[] names)
    {
        ParamsTest(names);
        SetName(resetName);
    }

    [BadMethod(description: "Greets the User")]
    private void Greet()
    {
        Console.WriteLine(GetGreeting());
    }

    private static void Main()
    {
        BadRuntime runtime = new BadRuntime()
            .UseCommonInterop()
            .ConfigureContextOptions(x => x.AddApi(new Program()));


        runtime.RunInteractive(Enumerable.Empty<string>());
    }
}