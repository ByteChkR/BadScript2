using System.Text;

using BadScript2.Interop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Examples.CustomApi;

/*
 * The Source Generator tries to remove as much of the boilerplate code as possible.
 * Creating a custom API is done in multiple steps:
 * 1. Create a internal partial class
 * 2. Add the BadInteropApiAttribute to the class (specify a name, if none is specified the class name will be used)
 * 3. Add a Method to the class
 * 4. Use the BadMethodAttribute to generate a wrapper for it
 * 4.1. Specify a Name for the method, if none is specified the method name will be used
 * 4.2. Specify a Description for the Method (optional)
 * 4.3. If the class returns a value, you can use the BadReturnAttribute to specify the return type description
 * 5. If the method has parameters it is also possible (but not required) to decorate them with the BadParameterAttribute
 * 5.1. Specify a Name for the parameter, if none is specified the parameter name will be used
 * 5.2. Specify a Description for the parameter (optional)
 *
 *
 * Supported Parameter Options:
 * - Null Checked: If not explicitly declared as nullable, the parameter will be null checked by the runtime
 * - Optional: The parameter can be omitted (specify default value for a parameter)
 * - Params: The parameter is declared as RestArgs by the runtime(and will contain all remaining arguments)
 *
 * Supported Parameter Types:
 * - bool
 * - byte
 * - sbyte
 * - short
 * - ushort
 * - int
 * - uint
 * - long
 * - ulong
 * - float
 * - double
 * - decimal
 * - char
 * - string
 * - BadObject (and all derived types)
 * - One-Dimensional Arrays of the listed types
 * - BadNullable of the listed types(legacy support for the old interop system)
 * - Nullable of the listed types(will be treated as nullable by the runtime)
 * - IList and List of any of the listed types
 *
 *
 * !! If allow Native Types is enabled, the runtime will wrap the return value in a BadNative<T> Instance if the return type is not supported by the runtime. !!
 * !! This is disabled by default. Enable it by specifying it in the BadReturnAttribute. !!
 * Supported Return Types:
 * - bool
 * - byte
 * - sbyte
 * - short
 * - ushort
 * - int
 * - uint
 * - long
 * - ulong
 * - float
 * - double
 * - decimal
 * - char
 * - string
 * - BadObject (and all derived types)
 * - Null
 */

/// <summary>
///     Implements a Custom API that is making use of the Source Generator
/// </summary>
[BadInteropApi("MySourceGenApi")]
internal partial class MySourceGeneratedCustomApi
{
    //Very simple method, that has no input and no output
    [BadMethod]
    private void SayHello()
    {
        Say(MyVeryLongMethodName());
    }

    //Renames the Method for the API and adds a description
    [BadMethod("GetHello", "Returns a Hello World String")]

    //Returns a value, so we use the BadReturnAttribute to specify the return description
    [return: BadReturn("Returns the Hello World String")]
    private string MyVeryLongMethodName()
    {
        return "Hello World!";
    }

    [BadMethod]
    private void Say(
        [BadParameter("text", "The Text to be printed to the console.")]
        string s //Renames the Parameter for the API and adds a description
    )
    {
        Console.WriteLine(s);
    }

    //This method has a nullable parameter and a nullable return type
    [BadMethod]
    [return: BadReturn("Returns the input text")]
    private string? MyNullableMethod(
        [BadParameter("text", "The Text to be returned.")]
        string? s //Renames the Parameter for the API and adds a description
    )
    {
        return s;
    }

    //This method has a nullable parameter and a nullable return type that is optional
    [BadMethod]
    [return: BadReturn("Returns the input text")]
    private string? MyNullableMethodWithDefault(
        [BadParameter("text", "The Text to be returned.")]
        string? s = "Hello World!" //Renames the Parameter for the API and adds a description
    )
    {
        return s;
    }

    //This method uses the params keyword to accept any number of arguments
    [BadMethod]
    private void SayMultiple(
        [BadParameter("texts", "The Text to be printed to the console.")]
        params string[] s

        //Renames the Parameter for the API and adds a description
        //The parameter automatically gets converted to a string array by the runtime.
    )
    {
        foreach (string s1 in s)
        {
            Console.WriteLine(s1);
        }
    }

    //This method explicitly allows returning a native type by specifying it in the BadReturnAttribute
    //This is disabled by default and only really useful to pass native types *THROUGH* the runtime to other c# code.
    [BadMethod]
    [return: BadReturn("Returns the input text", true)]
    private Encoding MyNativeMethod()
    {
        return Encoding.UTF8;
    }

    //This method uses a custom type as a parameter
    [BadMethod]
    private BadArray Encode(Encoding encoding, string text)
    {
        return new BadArray(encoding.GetBytes(text).Select(x => (BadObject)x).ToList());
    }

    [BadMethod]
    private void SayEncoded(Encoding encoding, byte[] data)
    {
        string str = encoding.GetString(data);
        Say(str);
    }
}