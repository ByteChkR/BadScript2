using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Error
{
    public class BadRuntimeError : BadObject
    {
        public BadRuntimeError(BadRuntimeError? innerError, BadObject obj, string stackTrace)
        {
            InnerError = innerError;
            ErrorObject = obj;
            StackTrace = stackTrace;
        }

        public string StackTrace { get; }

        public BadRuntimeError? InnerError { get; set; }
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
}