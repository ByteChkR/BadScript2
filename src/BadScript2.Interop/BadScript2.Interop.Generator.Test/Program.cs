using BadScript2.Interactive;
using BadScript2.Interop;
using BadScript2.Interop.Common;

namespace BadScript2.Generator.Test;

[BadInteropApi("TestApi")]
public partial class Program
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
    private void SetName([BadParameter(description: "The Name to be set.")] string name)
    {
        m_Name = name;
    }

    [BadMethod(description: "Gets the Name")]
    [return: BadReturn("The Name")]
    private string GetName()
    {
        return m_Name;
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