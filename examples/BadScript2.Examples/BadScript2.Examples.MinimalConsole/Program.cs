// See https://aka.ms/new-console-template for more information

using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("Minimal Example");


        // The Script to Execute
        string source = "123 + 321";

        // Create the Execution Context
        BadExecutionContext ctx = BadExecutionContextOptions.Default.Build();

        // Parse the Script
        // Call "ToArray" on the result to parse the script completely.
        // Otherwise the Parsing happens during the execution(which is fine normally, but since we are executing the same script twice, the parsing can be cached)
        BadExpression[] script = BadSourceParser.Parse("<none>", source).ToArray();


        // Run the Script
        // the result will be null because the Run method will only return a value if the script contains a return statement.
        BadObject? result = ctx.Run(script);

        //To get the result of the last statement in the script, use the ExecuteScript method
        //If an empty enumerable of expressions is passed into the method, the return will be BadObject.Null
        BadObject result2 = ctx.ExecuteScript(script);

        //Write the Result to the console
        Console.WriteLine($"{source} = {result2}");
    }
}