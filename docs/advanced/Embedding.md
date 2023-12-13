# Embedding


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

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)