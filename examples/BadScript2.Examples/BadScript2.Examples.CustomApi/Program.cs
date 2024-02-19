// See https://aka.ms/new-console-template for more information

using BadScript2;
using BadScript2.Examples.CustomApi;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("Custom Api Example");


        // The Script to Execute
        string source =
            @"
MyCustomApi.SayHello();
MyCustomApi.Say(""LOL"");
MyCustomApi.Say(MyCustomApi.WhoAmI());
MyCustomApi.Say(MyCustomApi.MyName);
MyCustomApi.Say(MyCustomApi.MyTable.Nested);
MyCustomApi.OldSchool(""Name"");
MyCustomApi.OldSchool(""Name"", ""The Description"");";

        // Create the Runtime
        using BadRuntime runtime = new BadRuntime()
            .ConfigureContextOptions(opts => opts.AddApi(new MyCustomApi()));


        // Run the Script
        // the result will be null because the Run method will only return a value if the script contains a return statement.
        BadRuntimeExecutionResult result = runtime.Execute(source);
    }
}