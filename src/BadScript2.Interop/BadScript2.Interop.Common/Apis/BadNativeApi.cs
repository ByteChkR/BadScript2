using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
namespace BadScript2.Interop.Common.Apis;

[BadInteropApi("Native")]
internal partial class BadNativeApi
{
    /// <summary>
    ///     Returns true if the given object is an instance of a class prototype
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is an instance of a class prototype")]
    [return: BadReturn("True if the given object is an instance of a class prototype")]
    private bool IsPrototypeInstance([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is BadClass;
    }

    /// <summary>
    ///     Returns true if the given object is a class prototype
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is a class prototype")]
    [return: BadReturn("True if the given object is a class prototype")]
    private bool IsPrototype([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is BadClassPrototype;
    }

    /// <summary>
    ///     Returns true if the given object is a native object
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is a native object")]
    [return: BadReturn("True if the given object is a native object")]
    private bool IsNative([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is IBadNative;
    }

    /// <summary>
    ///     Returns true if the given object is a function
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is a function")]
    [return: BadReturn("True if the given object is a function")]
    private bool IsFunction([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is BadFunction;
    }

    /// <summary>
    ///     Returns true if the given object is a table
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is a table")]
    [return: BadReturn("True if the given object is a table")]
    private bool IsTable([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is BadTable;
    }

    /// <summary>
    ///     Returns true if the given object is a string
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is a string")]
    [return: BadReturn("True if the given object is a string")]
    private bool IsString([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is IBadString;
    }

    /// <summary>
    ///     Returns true if the given object is a number
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is a number")]
    [return: BadReturn("True if the given object is a number")]
    private bool IsNumber([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is IBadNumber;
    }

    /// <summary>
    ///     Returns true if the given object is a boolean
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is a boolean")]
    [return: BadReturn("True if the given object is a boolean")]
    private bool IsBoolean([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is IBadBoolean;
    }

    /// <summary>
    ///     Returns true if the given object is an array
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is an array")]
    [return: BadReturn("True if the given object is an array")]
    private bool IsArray([BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg is BadArray;
    }

    /// <summary>
    ///     Returns true if the given object is enumerable
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is enumerable")]
    [return: BadReturn("True if the given object is enumerable")]
    private bool IsEnumerable(BadExecutionContext ctx, [BadParameter(description: "Object to test")] BadObject? arg)
    {
        return arg?.HasProperty("GetEnumerator", ctx.Scope) ?? false;
    }

    /// <summary>
    ///     Returns true if the given object is an enumerator
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    [BadMethod(description: "Returns true if the given object is an enumerator")]
    [return: BadReturn("True if the given object is an enumerator")]
    private bool IsEnumerator(BadExecutionContext ctx, [BadParameter(description: "Object to test")] BadObject? arg)
    {
        return (arg?.HasProperty("GetCurrent", ctx.Scope) ?? false) &&
               (arg?.HasProperty("MoveNext", ctx.Scope) ?? false);
    }
}