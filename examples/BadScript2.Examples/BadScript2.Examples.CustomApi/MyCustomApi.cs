using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Examples.CustomApi;

public class MyCustomApi : BadInteropApi
{
    //Set the name of the API in the constructor
    public MyCustomApi() : base("MyCustomApi") { }

    public override void Load(BadTable target)
    {
        //Add your api to the target table here

        //There are several helper and extension methods to make this easier

        //Add a function to the table and that takes no arguments
        target.SetFunction("SayHello", () => Console.WriteLine("Hello World!"));

        //Add a function that autmaticly converts the arguments to the specified types
        target.SetFunction<string>("Say", Console.WriteLine);

        //Add a function that returns a value
        target.SetFunction("WhoAmI", () => "I am a function");

        //Instead of functions you can also add properties
        target.SetProperty("MyName", "MyValue");

        //You can also add a table to the target table
        BadTable table = new BadTable();
        table.SetProperty("Nested", "Table");
        target.SetProperty("MyTable", table);
        
        //It is possible to add a function the "oldschool" way.
        //This allows for maximum flexibility but is also the most verbose
        target.SetProperty(
            "OldSchool",
            new BadInteropFunction(
                "OldSchool",
                (ctx, args) =>
                {
                    //First argument is has no type specified so we need to check manually
                    if (args[0] is not IBadString name)
                    {
                        ctx.Scope.SetError("Invalid Type for 'name' Expected String", null);
                        return BadObject.Null;
                    }

                    string desc = "";
                    //Second argument can be null. So we need to check for that
                    if (args.Length == 2 && args[1] != BadObject.Null)
                    {
                        //We dont need to check the type. as it is checked by the runtime.
                        desc = ((IBadString)args[1]).Value;
                    }
                    
                    Console.WriteLine("Name: {0} Description: {1}", name.Value, desc);
                    return BadObject.Null;
                },
                "name", //Implicit Cast
                new BadFunctionParameter( //Explicit Creation of the parameters
                    "description", //Parameter Name
                    true, //This parameter can be omitted
                    false, //This parameter is not nullchecked by the runtime
                    false, //This parameter is not the rest parameter
                    null, //Expression that results to a class prototype(is used by the runtime if a type is specified)
                    BadNativeClassBuilder.GetNative("string") //Can be used to specify a type directly
                )
            )
        );
    }
}