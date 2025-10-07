using System.Text.RegularExpressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Interop.Common.Regex;

public class BadCapture : BadNative<Capture>
{
    private readonly Dictionary<string, BadObjectReference> _refs = new();
    public static readonly BadNativeClassPrototype<BadCapture> Prototype = new BadNativeClassPrototype<BadCapture>(
        "Capture",
        (c, _) => throw BadRuntimeException.Create(c.Scope, "Capture cannot be instantiated directly."),
        () => Array.Empty<BadInterfacePrototype>(), null);
    public BadCapture(Capture value) : base(value)
    {
        _refs["Value"] = BadObjectReference.Make("Capture.Value", p => Value.Value);
        _refs["Index"] = BadObjectReference.Make("Capture.Index", p => Value.Index);
        _refs["Length"] = BadObjectReference.Make("Capture.Length", p => Value.Length);
        var toString = new BadDynamicInteropFunction("ToString", _ => ToString(),
            BadNativeClassBuilder.GetNative("string"));
        _refs["ToString"] = BadObjectReference.Make("Capture.ToString", p => toString);
    }
    
    
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return _refs.ContainsKey(propName) || base.HasProperty(propName, caller);
    }
    
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        if (_refs.TryGetValue(propName, out BadObjectReference? reference))
        {
            return reference;
        }

        return base.GetProperty(propName, caller);
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return $"Capture(\"{Value}\")";
    }
}