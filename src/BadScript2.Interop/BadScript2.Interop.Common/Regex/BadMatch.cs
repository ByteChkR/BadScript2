using System.Text.RegularExpressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Interop.Common.Regex;

public class BadMatch : BadNative<Match>
{
    private readonly Dictionary<string, BadObjectReference> _refs = new();
    public static readonly BadNativeClassPrototype<BadMatch> Prototype = new BadNativeClassPrototype<BadMatch>(
        "Match",
        (c, _) => throw BadRuntimeException.Create(c.Scope, "Match cannot be instantiated directly."),
        () => Array.Empty<BadInterfacePrototype>(), null);
    public BadMatch(Match value) : base(value)
    {
        _refs["Success"] = BadObjectReference.Make("Match.Success", p => Value.Success);
        _refs["Value"] = BadObjectReference.Make("Match.Value", p => Value.Value);
        _refs["Index"] = BadObjectReference.Make("Match.Index", p => Value.Index);
        _refs["Length"] = BadObjectReference.Make("Match.Length", p => Value.Length);
        _refs["Groups"] = BadObjectReference.Make("Match.Groups", p => new BadArray(Value.Groups.Select(g => (BadObject)new BadGroup(g)).ToList()));
        _refs["Captures"] = BadObjectReference.Make("Match.Groups", p => new BadArray(Value.Captures.Select(g => (BadObject)new BadCapture(g)).ToList()));
        var toString = new BadDynamicInteropFunction("ToString", _ => ToString(),
            BadNativeClassBuilder.GetNative("string"));
        _refs["ToString"] = BadObjectReference.Make("Match.ToString", p => toString);
        _refs["NextMatch"] = BadObjectReference.Make("Match.NextMatch", p => new BadMatch(Value.NextMatch()));
        var result = new BadDynamicInteropFunction<string>("Result", (_, s) => Value.Result(s),
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("replacement", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        _refs["Result"] = BadObjectReference.Make("Match.Result", p => result);
        
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
        return $"Match(\"{Value}\")";
    }
}