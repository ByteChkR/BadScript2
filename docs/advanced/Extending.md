# Extending


## Adding Custom Extensions to Objects

To extend arbitrary objects with extension methods and properties, inherit the `BadInteropExtension` class.
```csharp

public class MyCustomExtensions : BadInteropExtension
{
	protected override void AddExtensions()
	{
		//Register the Extensions Here
		//Use the RegisterGlobal and the RegisterObject functions

		//Register an extension property named "IsEmptyObject" for the Table Object
		RegisterObject<BadTable>("IsEmptyObject", t => t.InnerTable.Count == 0);

		//Register an extension property named "IsNull" for all objects inside the language
		RegisterGlobal("IsNull", o => o == BadObject.Null);

		//Register an extension method named "IsNull" for the table object using the Dynamic Interop Function Helper
		RegisterObject<BadTable>("IsNull", t => new BadDynamicInteropFunction("IsNull", ctx => t == BadObject.Null));

		//Register an extension method named "Split" for the String object using the Interop Function Helper
		//The Interop Object allows for more flexibility when working with optional and nullchecked parameters.
		RegisterObject<string>(
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
private static void Main()
{
	BadInteropExtension.AddExtension<MyCustomExtensions>();
}

```

## Adding Custom APIs

To directly interface with C# code the BadScript Runtime can add APIs to its Root Scope.
```csharp

public class MyCustomApi : BadInteropApi
{
	public MyCustomApi() : base("MyApi") {}

	public override void Load(BadTable target)
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
private static void Main()
{
	BadExecutionContextOptions.Default.Apis.Add(new MyCustomApi());
}

```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)