using System.Text.RegularExpressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Interop.Common.Regex;

public class BadGroup : BadNative<Group>
{
    private readonly Dictionary<string, BadObjectReference> _refs = new();
    public static readonly BadNativeClassPrototype<BadGroup> Prototype = new BadNativeClassPrototype<BadGroup>(
        "Group",
        (c, _) => throw BadRuntimeException.Create(c.Scope, "Group cannot be instantiated directly."),
        () => Array.Empty<BadInterfacePrototype>(), null);
    public BadGroup(Group value) : base(value)
    {
        _refs["Success"] = BadObjectReference.Make("Group.Success", p => Value.Success);
        _refs["Value"] = BadObjectReference.Make("Group.Value", p => Value.Value);
        _refs["Index"] = BadObjectReference.Make("Group.Index", p => Value.Index);
        _refs["Length"] = BadObjectReference.Make("Group.Length", p => Value.Length);
        _refs["Captures"] = BadObjectReference.Make("Group.Captures", p => new BadArray(Value.Captures.Select(c => (BadObject)new BadCapture(c)).ToList()));
        var toString = new BadDynamicInteropFunction("ToString", _ => ToString(),
            BadNativeClassBuilder.GetNative("string"));
        _refs["ToString"] = BadObjectReference.Make("Group.ToString", p => toString);
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
        return $"Group(\"{Value}\")";
    }
}