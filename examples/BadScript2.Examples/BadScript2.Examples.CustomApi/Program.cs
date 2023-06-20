// See https://aka.ms/new-console-template for more information

using BadScript2.Examples.CustomApi;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("Custom Api Example");


        // The Script to Execute
        string source =
            "MyCustomApi.SayHello();\nMyCustomApi.Say(\"LOL\");\nMyCustomApi.Say(MyCustomApi.WhoAmI());\nMyCustomApi.Say(MyCustomApi.MyName);\nMyCustomApi.Say(MyCustomApi.MyTable.Nested);\nMyCustomApi.OldSchool(\"Name\");\nMyCustomApi.OldSchool(\"Name\", \"The Description\");";

        // Create the Execution Context
        BadExecutionContextOptions options = new BadExecutionContextOptions();

        //Add the Custom API
        options.AddApi(new MyCustomApi());
        BadExecutionContext ctx = options.Build();

        // Parse the Script
        // Call "ToArray" on the result to parse the script completely.
        // Otherwise the Parsing happens during the execution(which is fine normally, but since we are executing the same script twice, the parsing can be cached)
        BadExpression[] script = BadSourceParser.Parse("<none>", source).ToArray();


        // Run the Script
        // the result will be null because the Run method will only return a value if the script contains a return statement.
        BadObject? result = ctx.Run(script);
    }
}