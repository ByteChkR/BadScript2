[![build](https://github.com/ByteChkR/BadScript2/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ByteChkR/BadScript2/actions/workflows/dotnet.yml)

# BadScript2 Console Documentation

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