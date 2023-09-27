using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Error;

/// <summary>
///     Implements the Error Object Type
/// </summary>
public class BadRuntimeError : BadObject
{
    private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadRuntimeError>(
        "Error",
        (_, _) => throw new BadRuntimeException("Error")
    );

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

    public static BadRuntimeError FromException(Exception e, string? scriptStackTrace = null)
    {
        BadRuntimeError? inner = e.InnerException == null ? null : FromException(e.InnerException);

        string st;

        if (scriptStackTrace == null)
        {
            st = e.StackTrace;
        }
        else
        {
            st = "Script Stack Trace: " +
                 Environment.NewLine +
                 scriptStackTrace +
                 Environment.NewLine +
                 "Runtime Stacktrace:" +
                 Environment.NewLine +
                 e.StackTrace;
        }

        return new BadRuntimeError(inner, e.Message, st);
    }

    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return $"{ErrorObject.ToSafeString(done)} at\n{StackTrace}\n{InnerError?.ToSafeString(done) ?? ""}";
    }

    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        if (propName is IBadString str)
        {
            switch (str.Value)
            {
                case "StackTrace":
                    return BadObjectReference.Make("Error.StackTrace", () => StackTrace);
                case "InnerError":
                    return BadObjectReference.Make("Error.InnerError", () => InnerError ?? Null);
                case "ErrorObject":
                    return BadObjectReference.Make("Error.ErrorObject", () => ErrorObject);
            }
        }

        return base.GetProperty(propName, caller);
    }

    public override bool HasProperty(BadObject propName)
    {
        return propName is IBadString
               {
                   Value: "StackTrace" or "InnerError" or "ErrorObject",
               } ||
               base.HasProperty(propName);
    }
}