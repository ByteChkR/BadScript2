# Embedding


Embedding the runtime into a C# project.
Here is the minimal Example
```csharp
using BadScript2.Interop.Common;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

internal class Program
{
    private static void Main()
    {
        string script = "Console.WriteLine(\"Hello World!\");";

        //Add the default extensions
        //Without, the objects are not of much use
        //This is theoretically optional, but it is a good idea to have
        BadCommonInterop.AddExtensions();
        BadExecutionContextOptions.Default.Apis.AddRange(BadCommonInterop.Apis);


        //Create the Parser. Specify a correct filename if possible to get line level error messages
        BadSourceParser parser = BadSourceParser.Create("<file-name>", script);

        //Parse the script
        IEnumerable<BadExpression> expressions = parser.Parse();

        //Build the Context
        BadExecutionContext context = BadExecutionContextOptions.Default.Build();

        //Execute the script synchronously
        BadObject? result = context.Run(expressions);
        
        //Or Execute the script iteratively as a coroutine

        foreach (BadObject o in context.Execute(expressions))
        {
            //Do some work, or yield the object
        }
    }
}
```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)