using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Error;

/// <summary>
///     Implements the Error Object Type
/// </summary>
public class BadRuntimeError : BadObject
{
    /// <summary>
    ///     Creates a new Error Object
    /// </summary>
    /// <param name="innerError">The Inner Error</param>
    /// <param name="obj">The Object that was thrown</param>
    /// <param name="stackTrace">The Stacktrace of the Error</param>
    public BadRuntimeError(BadRuntimeError? innerError, BadObject obj, string stackTrace)
    {
        InnerError = innerError;
        ErrorObject = obj;
        StackTrace = stackTrace;
    }

    /// <summary>
    ///     The Stacktrace of the Error
    /// </summary>
    public string StackTrace { get; }

    /// <summary>
    ///     The Inner Error
    /// </summary>
    public BadRuntimeError? InnerError { get; set; }

    /// <summary>
    ///     The Object that was thrown
    /// </summary>
    public BadObject ErrorObject { get; }

    public override BadClassPrototype GetPrototype()
    {
        return new BadNativeClassPrototype<BadRuntimeError>(
            "Error",
            (_, _) => throw new BadRuntimeException("Error")
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return $"{ErrorObject.ToSafeString(done)} at\n{StackTrace}\n{InnerError?.ToSafeString(done) ?? ""}";
    }

    public override BadObjectReference GetProperty(BadObject propName)
    {
        if (propName is IBadString str)
        {
            switch (str.Value)
            {
                case "StackTrace": return BadObjectReference.Make("Error.StackTrace", () => StackTrace);
                case "InnerError": return BadObjectReference.Make("Error.InnerError", () => InnerError ?? Null);
                case "ErrorObject": return BadObjectReference.Make("Error.ErrorObject", () => ErrorObject);
            }
        }

        return base.GetProperty(propName);
    }

    public override bool HasProperty(BadObject propName)
    {
        return propName is IBadString { Value: "StackTrace" or "InnerError" or "ErrorObject" } ||
               base.HasProperty(propName);
    }
}