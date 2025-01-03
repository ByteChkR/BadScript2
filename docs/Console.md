# BadScript2 Console Documentation
[![build](https://github.com/ByteChkR/BadScript2/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ByteChkR/BadScript2/actions/workflows/dotnet.yml)

<div style="display: flex; justify-content: center;">
    <img width="256" height="256" src="https://github.com/ByteChkR/BadScript2/blob/master/res/Logo.png?raw=true"/>
</div>

Bad Script is an Interpreted Scripting Language written in pure C#. It has a similar syntax to javascript and written to be easily extensible.

## Table of Contents

- [BadScript2 Manual](#badscript2-documentation)
  - [Table of Contents](#table-of-contents)
  - [Preparations for the public branch](#preparations-for-the-public-branch)
  - [Install + Hello World](#install--hello-world)
    - [Installing](#installing)
      - [Building from Source](#building-from-source)
    - [Hello World](#hello-world)
  - [Commandline](#commandline)
    - [Subsystem `docs`](#subsystem-docs)
    - [Subsystem `run`](#subsystem-run)
    - [Subsystem `test`](#subsystem-test)
    - [Subsystem `settings`](#subsystem-settings)
    - [Subsystem `remote`](#subsystem-remote)
    - [Subsystem `html`](#subsystem-html)
  - [Start Building](#start-building)
    - [new](#new)
    - [build](#build)
  - [Extending](#extending)
    - [Adding Custom Extensions to Objects](#adding-custom-extensions-to-objects)
    - [Adding Custom APIs](#adding-custom-apis)
    - [Using Source Generators to Implement Wrappers](#using-source-generators-to-implement-wrappers)
        - [Implementing a Custom API](#implementing-a-custom-api)
        - [Implementing an Object Wrapper](#implementing-an-object-wrapper)
            - [Available Members for Type `Person`:](#available-members-for-type-person)
            - [Available Members for Type `Employee`:](#available-members-for-type-employee)
        - [Make Types Generally Available](#make-types-generally-available)
  - [Embedding](#embedding)
  - [Using BadLinq](#using-badlinq)


## Preparations for the public branch

At this point in time, there is a single dependency that is not available from the public nuget.org repository.
To be able to restore the solution, the project `BadScript2.IO.OpenKM` needs to be unloaded.

## Install + Hello World

Follow these steps to get started.

### Installing

Currently it is only possible to build the project from source. Binaries will be available on the first release.

#### Building from Source

Tested on:
- Windows 10 64-bit
- Debian 11 (Bullseye)

Requirements:
- git
- net6.0 SDK
- Powershell 7.0 or greater

1. Open a Powershell Session
  - On Windows: `powershell`
  - On Linux: `pwsh`
2. Clone this Repository
3. Change Directory to `./BadScript2`
3. Run `./build.ps1`

> use `build.ps1 -config Release` to build with optimizations.

> use `build.ps1 -writeLog` to get debug logs

`build.ps1` compiles the language project and builds all common libraries for the runtime.
The Compiled output will be generated in `./build`.

#### Building with Install Script

For convenience use the InstallBadScript2.ps1 script in ./docs to install.
It does the Cloning/Building from the Git Repo automatically.

Either download the script file and start manually. Or directly execute the Script from a URL:
```
. { iwr -useb https://bytechkr.github.io/BadScript2/InstallBadScript2.ps1 } | iex; InstallBadScript2
```



### Hello World
Create a file `helloworld.bs` with the following content:
```js
Console.WriteLine("Hello World!");
```

Run the script with the command
```
bs run -f helloworld.bs
```

Output:
```
Hello World!
```


## Commandline

The Commandline Application included in this Project has multiple subsystems.

### Subsystem `docs`

The `docs` subsystem can be used to display a auto generated documentation of all apis.

Example Command:
```
bs docs
```

### Subsystem `run`

The `run` subsystem can run BadScripts inside a commandline environment.

Example Commands:
```
bs run -f <inputfile>
bs run -f <inputfile> -a <argument>
```

Commandline Flags:
```
  -f, --files          The files to run.

  -i, --interactive    Run in interactive mode.

  -a, --args           Arguments to pass to the script.

  -b, --benchmark      Set flag to Measure Execution Time.

  -d, --debug          Set flag to Attach a Debugger.

  --help               Display this help screen.

  --version            Display version information.
```

### Subsystem `test`

The `test` subsystem can run BadScript files inside a Unit Test Environment using the NUnit Framework.

Example Commands:
```
bs test
```

Commandline Flags:
```
  --help               Display this help screen.

  --version            Display version information.
```


### Subsystem `settings`

The `settings` subsystem can load configuration files from the internal settings.

Example Commands:
```
bs settings <settingspath>
```

Commandline Flags:
```
  --help          Display this help screen.

  --version       Display version information.
```

### Subsystem `remote`

The `remote` subsystem can host or connect to a remote console on another machine.

Example Commands:
```
bs remote localhost 1337
```

Commandline Arguments:
```
  --help          Display this help screen.

  --version       Display version information.

  value pos. 0    Required. (Default: localhost) The Host to connect to

  value pos. 1    Required. (Default: 1337) The Host port to connect to
```

### Subsystem `html`

The `html` subsystem exposes the BadHtml Template Engine.

Example Commands:
```
bs html <inputfile>
bs html -f <inputfiles>
```

Commandline Arguments:
```
  -f, --files         The files to run.

  --model             The Model that the templates will use

  -d, --debug         Set flag to Attach a Debugger.

  -r, --remote        Specifies the Remote Console Host port. If not specified the remote host will not be started

  --skipEmptyNodes    If enabled, empty text nodes will be skipped.

  -m, --minify        If enabled, the output will be minified.

  --help              Display this help screen.

  --version           Display version information.
```

## Start Building

The Commandline comes with multiple BadScript apps preinstalled.
This page covers two of them.

___

### new

The `new` app is used to create new projects.

> Basic Usage: `bs new <template>`

Using this app is the recommended way to create new projects.

> Templates are stored in the directory returned from `bs settings Subsystems.New.TemplateDirectory`

___

### build

The `build` app is used to build and install a project.

> Basic Usage: `bs build [<target>]`

#### Targets

The Build System has Multiple targets.

> To View all targets open the `build` directory located at the path that gets returned from `bs settings Console.SubsystemDirectory`

Most Common targets are `Debug` and `Release`.
Both targets will build and install the project into the runtime.

> The install location is one of the directories returned from `bs settings Subsystems.Run`


## Extending


### Adding Custom Extensions to Objects

To extend arbitrary objects with extension methods and properties, inherit the `BadInteropExtension` class.
```csharp

public class MyCustomExtensions : BadInteropExtension
{
	protected override void AddExtensions(BadInteropExtensionProvider provider)
	{
		//Register the Extensions Here
		//Use the RegisterGlobal and the RegisterObject functions

		//Register an extension property named "IsEmptyObject" for the Table Object
		provider.RegisterObject<BadTable>("IsEmptyObject", t => t.InnerTable.Count == 0);

		//Register an extension property named "IsNull" for all objects inside the language
		provider.RegisterGlobal("IsNull", o => o == BadObject.Null);

		//Register an extension method named "IsNull" for the table object using the Dynamic Interop Function Helper
		provider.RegisterObject<BadTable>("IsNull", t => new BadDynamicInteropFunction("IsNull", ctx => t == BadObject.Null));

		//Register an extension method named "Split" for the String object using the Interop Function Helper
		//The Interop Object allows for more flexibility when working with optional and nullchecked parameters.
		provider.RegisterObject<string>(
            "Split",
            s => new BadInteropFunction(
                "Split",
                args => StringSplit(s, args[0], args.Length == 2 ? args[1] : BadObject.Null),
                "splitStr", //Implicit Cast to BadFunctionParameter
                new BadFunctionParameter("skipEmpty", true, false, false)
                //"skipEmpty" : The parameter name
                //true : isOptional (in bad lang this will be denoted as a '?' after the parameter name)
                //false : isNullChecked (in bad lang this will be denoted as a '!' after the parameter name)
                //false : isRestArgs (in bad lang this will be denoted as a '*' after the parameter name)
            )
        ); 
	}
}


//Adding the extension is done by registering the extensions like this:
private static void Register(BadRuntime runtime)
{
	runtime.ConfigureContextOptions(opts => opts.AddExtension<MyCustomExtensions>());
}

```

### Adding Custom APIs

To directly interface with C# code the BadScript Runtime can add APIs to its Root Scope.
```csharp

public class MyCustomApi : BadInteropApi
{
	public MyCustomApi() : base("MyApi") {}

	protected override void LoadApi(BadTable target)
	{
		//Add the api functions to this target table.

		//Either manually add them with the GetProperty Function that returns a BadObjectReference to the property
		target.GetProperty("MyProperty").Set("MYVALUE");

		//Or use one of the many extension methods
		target.SetProperty("MyProperty", "MYVALUE");
		target.SetFunction("MyFunction", () => "MYVALUE");
	}
}


//Adding the custom api is done by adding it to the Execution Context options that will be used to create the execution context
private static void Register(BadRuntime runtime)
{
	runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new MyCustomApi()));
}

```

### Using Source Generators to Implement Wrappers

Writing Wrappers and APIs require lots of boilerplate code.

A simple solution is to use the Source Generators to automatically generate Boilerplate-Code

To use the Source Generators, add the Source Generator Reference to the Project `<ProjectReference Include="..\BadScript2.Interop.Generator\BadScript2.Interop.Generator\BadScript2.Interop.Generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />`.

#### Implementing a Custom API


This Class Implements an Api with the name `Test`

```csharp

[BadInteropApi("Test")]
internal partial class TestApi
{
    private string m_Name = "World";

    [BadMethod("Hello", "Says Hello")] //Optional Name(Function within BadScript is named "Hello"), Optional Method Description
    private void SayHello()
    {
        Console.WriteLine("Hello");
    }

    [BadMethod(description: "Returns the Greeting String")] //Optional Method Description
    [return: BadReturn("Hello {name}")] //Optional Return Value Description
    private string GetGreeting()
    {
        return $"Hello {m_Name}";
    }

    [BadMethod(description: "Sets the Name")] //Optional Method Description
    private void SetName([BadParameter(description: "The Name to be set.")] /*Optional Parameter Description*/
                            string name = "World" /*Auto Detects Default Values*/)
    {
        m_Name = name;
    }

    [BadMethod(description: "Gets the Name")] //Optional Method Description
    [return: BadReturn("The Name")] //Optional Return Value Description
    private string GetName()
    {
        return m_Name;
    }

    [BadMethod(description: "Greets a list of users")] //Optional Method Description
    private void ParamsTest(params string[] names) //Auto Detects Params Parameter
    {
        foreach (string name in names)
        {
            SetName(name);
            Greet();
        }
    }

    [BadMethod(description: "Greets a list of users and resets the name")] //Optional Method Description
    private void ParamsTest2(string resetName, params string[] names) //Auto Detects Params Parameter
    {
        ParamsTest(names);
        SetName(resetName);
    }

    [BadMethod(description: "Greets the User")] //Optional Method Description
    private void Greet()
    {
        Console.WriteLine(GetGreeting());
    }
}
```

Following Properties will be available when used within BadScript (add api with `BadRuntime.UseApi(new TestApi())`)

- `void Test.Hello()`
- `string Test.GetGreeting()`
- `void Test.SetName(string name?)`
- `string Test.GetName()`
- `void Test.ParamsTest(Array names*)`
- `void Test.ParamsTest1(string resetName, Array names*)`
- `void Test.Greet()`


#### Implementing an Object Wrapper

This Code Defines 2 Objects Person and Employee

```csharp

[BadInteropObject("Person")] //Wrapper "PersonWrapper" will be generated for class Person
public class Person
{
    //Auto Detect Constructor(first public, non-static constructor will be used)
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    [BadProperty] //This Attribute makes the Property available in the wrapper.
    public string Name { get; set; }

    [BadProperty]
    public int Age { get; set; }

    [BadMethod] //Similar to the Interop Api Generator we can mark functions that need to be included in the wrapper.
    public virtual string PrintInfo()
    {
        return $"{Name} is {Age} years old";
    }
}


//To Properly handle inheritance we need to specify the Base Type Wrapper
[BadInteropObject("Employee", typeof(PersonWrapper))]
public class Employee : Person
{
    //Detects Constructor
    public Employee(string name, int age, string job, int employeeId) : base(name, age)
    {
        Job = job;
        EmployeeId = employeeId;
    }

    [BadProperty]
    public string Job { get; set; }

    [BadProperty]
    public int EmployeeId { get; set; }

    //This Method will be available in the Wrapper object because it was included in the PersonWrapper
    //Adding a BadMethodAttribute here has no effect.
    public override string PrintInfo()
    {
        return base.PrintInfo() + " and works as " + Job + " with EmployeeId " + EmployeeId;
    }
}
```

Objects of Type Person or Employee can now be wrapped. And used within BadScript
```csharp
  BadObject person = (PersonWrapper)new Person("John", 42);
  BadObject employee = (EmployeeWrapper)new Employee("Jane", 69, "Teacher", 123);
```

#### Available Members for Type `Person`

- `string Name { get; set; }`
- `num Age { get; set; }`
- `string PrintInfo()`

#### Available Members for Type `Employee`

- `string Name { get; set; }` (Inherited from Person)
- `num Age { get; set; }` (Inherited from Person)
- `string PrintInfo()` (Inherited from Person)
- `string Job { get; set; }`
- `num EmployeeId { get; set; }`

### Make Types Generally Available
To make the Types generally Available to all Runtimes we can add them to the `BadNativeClassBuilder`

```csharp
BadNativeClassBuilder.AddNative(PersonWrapper.Prototype);
BadNativeClassBuilder.AddNative(EmployeeWrapper.Prototype);
```

If the Prototype is available, we can construct a new Employee within BadScript like this:
```js
const employee = new Employee("Jane", 69, "Teacher", 123);

//Accessing a Method
Console.WriteLine(employee.PrintInfo()); //Prints "Jane is 69 years old and works as Teacher with EmployeeId 123"

//Proper Inheritance handling is also possible if the BadInteropObjectAttribute on the Employee Object specifies the typeof PersonWrapper in the constructor
Console.WriteLine($"Is Person: {employee instanceof Person}"); //Prints: "Is Person: True"
Console.WriteLine($"Is Employee: {employee instanceof Employee}"); //Prints: "Is Employee: True"
```

## Embedding


Embedding the runtime into a C# project.
Here is the minimal Example
```csharp
using BadScript2;
using BadScript2.Interop.Common;
using BadScript2.Runtime.Objects;

internal class Program
{
    private static void Main()
    {
        string script = "Console.WriteLine(\"Hello World!\");";

        //Add the default extensions
        //Without, the objects are not of much use
        //This is theoretically optional, but it is a good idea to have
        BadRuntime runtime = new BadRuntime()
            .UseCommonInterop();


        
        //Execute the script synchronously
        BadObject? result = runtime.Execute(script);
    }
}
```

## Using BadLinq

The Linq Extensions can be used to express the linq expressions in BadScript2 Syntax

```csharp
IEnumerable<int> e = Enumerable.Range(0, 123);
foreach (var v in e.Where("x => x % 2 == 0").Select("x => x * 2"))
{ 
    System.Console.WriteLine(v);
}
```