using BadScript2.Interactive;
using BadScript2.Interop;
using BadScript2.Interop.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Generator.Test;

[BadInteropObject("Person")]
public class Person
{
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
    [BadProperty]
    public string Name { get; set; }
    [BadProperty]
    public int Age { get; set; }

    [BadMethod]
    public virtual string PrintInfo()
    {
        return $"{Name} is {Age} years old";
    }
}

[BadInteropObject("Employee", typeof(PersonWrapper))]
public class Employee : Person
{
    public Employee(string name, int age, string job, int employeeId) : base(name, age)
    {
        Job = job;
        EmployeeId = employeeId;
    }
    [BadProperty]
    public string Job { get; set; }
    [BadProperty]
    public int EmployeeId { get; set; }

    public override string PrintInfo()
    {
        return base.PrintInfo() + " and works as " + Job + " with EmployeeId " + EmployeeId;
    }
}


[BadInteropApi("TestApi")]
internal partial class Program
{
    private string m_Name = "World";

    [BadMethod("Hello", "Says Hello")]
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
                             .UseApi(new Program());
        BadObject obj = (PersonWrapper)new Person("John", 42);
        
        BadNativeClassBuilder.AddNative(PersonWrapper.Prototype);
        BadNativeClassBuilder.AddNative(EmployeeWrapper.Prototype);
        
        runtime.RunInteractive(Enumerable.Empty<string>());
    }
}

