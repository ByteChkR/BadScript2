using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

/// <summary>
/// Contains the Error Objects for the BadScript2 Language
/// </summary>
namespace BadScript2.Runtime.Error;

/// <summary>
///     Implements the Error Object Type
/// </summary>
public class BadRuntimeError : BadObject
{
    public static readonly BadClassPrototype Prototype = new BadNativeClassPrototype<BadRuntimeError>("Error",
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

    /// <summary>
    ///     Creates a BadRuntimeError from an Exception
    /// </summary>
    /// <param name="e">The Exception</param>
    /// <param name="scriptStackTrace">The Script Stack Trace</param>
    /// <returns>Returns a BadRuntimeError</returns>
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

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);

        return $"{ErrorObject.ToSafeString(done)} at\n{StackTrace}\n{InnerError?.ToSafeString(done) ?? ""}";
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        switch (propName)
        {
            case "StackTrace":
                return BadObjectReference.Make("Error.StackTrace", (p) => StackTrace);
            case "InnerError":
                return BadObjectReference.Make("Error.InnerError", (p) => InnerError ?? Null);
            case "ErrorObject":
                return BadObjectReference.Make("Error.ErrorObject", (p) => ErrorObject);
        }

        return base.GetProperty(propName, caller);
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return propName is "StackTrace" or "InnerError" or "ErrorObject" ||
               base.HasProperty(propName, caller);
    }
}