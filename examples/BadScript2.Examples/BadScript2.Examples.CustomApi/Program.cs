// See https://aka.ms/new-console-template for more information

using BadScript2;
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

        // Create the Runtime
        using BadRuntime runtime = new BadRuntime();
        runtime.Options.AddApi(new MyCustomApi());
        

        // Run the Script
        // the result will be null because the Run method will only return a value if the script contains a return statement.
        BadObject? result = runtime.Execute(source);
    }
}