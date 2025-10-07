using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Regex;

public class BadRegex : BadNative<System.Text.RegularExpressions.Regex>
{
    public static readonly BadNativeClassPrototype<BadRegex> Prototype = new BadNativeClassPrototype<BadRegex>(
        "Regex",
        (c, args) =>
        {
            if (args.Length != 1)
            {
                throw BadRuntimeException.Create(c.Scope, "Regex constructor expects exactly one argument.");
            }

            if (args[0] is not IBadString str)
            {
                throw BadRuntimeException.Create(c.Scope, "Regex constructor expects a string as the first argument.");
            }

            return new BadRegex(new System.Text.RegularExpressions.Regex(str.Value));
        }, StaticMembers(), null);

    private static Dictionary<string, BadObjectReference> StaticMembers()
    {
        Dictionary<string, BadObjectReference> members = new Dictionary<string, BadObjectReference>();
        var escape = new BadDynamicInteropFunction<string>("Escape", (_, s) => System.Text.RegularExpressions.Regex.Escape(s),
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("str", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var unescape = new BadDynamicInteropFunction<string>("Unescape", (_, s) => System.Text.RegularExpressions.Regex.Unescape(s),
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("str", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        // Add IsMatch, Match, Matches, Replace, Split as static methods
        var isMatch = new BadDynamicInteropFunction<string, string>("IsMatch", (_, pattern, input) => System.Text.RegularExpressions.Regex.IsMatch(input, pattern),
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("pattern", false, false, false, null, BadNativeClassBuilder.GetNative("string")),
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var match = new BadDynamicInteropFunction<string, string>("Match", (_, pattern, input) => new BadMatch(System.Text.RegularExpressions.Regex.Match(input, pattern)),
            BadMatch.Prototype,
            new BadFunctionParameter("pattern", false, false, false, null, BadNativeClassBuilder.GetNative("string")),
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var matches = new BadDynamicInteropFunction<string, string>("Matches", (_, pattern, input) => new BadArray(System.Text.RegularExpressions.Regex.Matches(input, pattern).Select(x => (BadObject)new BadMatch(x)).ToList()),
            BadArray.Prototype,
            new BadFunctionParameter("pattern", false, false, false, null, BadNativeClassBuilder.GetNative("string")),
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var replace = new BadDynamicInteropFunction<string, string, string>("Replace", (_, pattern, input, replacement) => System.Text.RegularExpressions.Regex.Replace(input, pattern, replacement),
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("pattern", false, false, false, null, BadNativeClassBuilder.GetNative("string")),
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")),
            new BadFunctionParameter("replacement", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var split = new BadDynamicInteropFunction<string, string>("Split", (_, pattern, input) => new BadArray(System.Text.RegularExpressions.Regex.Split(input, pattern).Select(x => (BadObject)x).ToList()),
            BadArray.Prototype,
            new BadFunctionParameter("pattern", false, false, false, null, BadNativeClassBuilder.GetNative("string")),
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        
        //Add to members
        members["Escape"] = BadObjectReference.Make("Regex.Escape", p => escape);
        members["Unescape"] = BadObjectReference.Make("Regex.Unescape", p => unescape);
        members["IsMatch"] = BadObjectReference.Make("Regex.IsMatch", p => isMatch);
        members["Match"] = BadObjectReference.Make("Regex.Match", p => match);
        members["Matches"] = BadObjectReference.Make("Regex.Matches", p => matches);
        members["Replace"] = BadObjectReference.Make("Regex.Replace", p => replace);
        members["Split"] = BadObjectReference.Make("Regex.Split", p => split);
        return members;
    }

    private readonly Dictionary<string, BadObjectReference> _refs = new();
    public BadRegex(System.Text.RegularExpressions.Regex value) : base(value)
    {
        var isMatch = new BadDynamicInteropFunction<string>("IsMatch", (_, s) => IsMatch(s),
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        
        var match = new BadDynamicInteropFunction<string>("Match", (_, s) => Match(s),
            BadMatch.Prototype,
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var matches = new BadDynamicInteropFunction<string>("Matches", (_, s) => Matches(s),
            BadArray.Prototype,
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var replace = new BadDynamicInteropFunction<string, string>("Replace", (_, s, r) => Replace(s, r),
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")),
            new BadFunctionParameter("replacement", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var split = new BadDynamicInteropFunction<string>("Split", (_, s) => Split(s),
            BadArray.Prototype,
            new BadFunctionParameter("input", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var toString = new BadDynamicInteropFunction("ToString", _ => ToString(),
            BadNativeClassBuilder.GetNative("string"));
        var getGroupNames = new BadDynamicInteropFunction("GetGroupNames", _ => GetGroupNames(),
            BadArray.Prototype);
        var getGroupNumbers = new BadDynamicInteropFunction("GetGroupNumbers", _ => GetGroupNumbers(),
            BadArray.Prototype);
        var groupNumberFromName = new BadDynamicInteropFunction<string>("GroupNumberFromName", (_, s) => GroupNumberFromName(s),
            BadNativeClassBuilder.GetNative("num"),
            new BadFunctionParameter("name", false, false, false, null, BadNativeClassBuilder.GetNative("string")));
        var groupNameFromNumber = new BadDynamicInteropFunction<int>("GroupNameFromNumber", (_, i) => GroupNameFromNumber(i),
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("number", false, false, false, null, BadNativeClassBuilder.GetNative("num")));
        _refs["MatchTimeout"] = BadObjectReference.Make("Regex.MatchTimeout", p => Value.MatchTimeout);
        _refs["RightToLeft"] = BadObjectReference.Make("Regex.RightToLeft", p => Value.RightToLeft);
        _refs["IsMatch"] = BadObjectReference.Make("Regex.IsMatch", p => isMatch);
        _refs["Match"] = BadObjectReference.Make("Regex.Match", p => match);
        _refs["Matches"] = BadObjectReference.Make("Regex.Matches", p => matches);
        _refs["Replace"] = BadObjectReference.Make("Regex.Replace", p => replace);
        _refs["Split"] = BadObjectReference.Make("Regex.Split", p => split);
        _refs["ToString"] = BadObjectReference.Make("Regex.ToString", p => toString);
        _refs["GetGroupNames"] = BadObjectReference.Make("Regex.GetGroupNames", p => getGroupNames);
        _refs["GetGroupNumbers"] = BadObjectReference.Make("Regex.GetGroupNumbers", p => getGroupNumbers);
        _refs["GroupNumberFromName"] = BadObjectReference.Make("Regex.GroupNumberFromName", p => groupNumberFromName);
        _refs["GroupNameFromNumber"] = BadObjectReference.Make("Regex.GroupNameFromNumber", p => groupNameFromNumber);
    }

    public bool IsMatch(string input)
    {
        return Value.IsMatch(input);
    }

    public BadMatch Match(string input)
    {
        return new BadMatch(Value.Match(input));
    }

    public BadArray Matches(string input)
    {
        return new BadArray(Value.Matches(input).Select(x => (BadObject)new BadMatch(x)).ToList());
    }
    
    //Implement wrappers for other Regex Methods: Replace, Split, ToString, GetGroupNames, GetGroupNumbers, GetGroupNumberFromName, GetGroupNameFromNumber
    public string Replace(string input, string replacement)
    {
        return Value.Replace(input, replacement);
    }
    
    public BadArray Split(string input)
    {
        return new BadArray(Value.Split(input).Select(x => (BadObject)x).ToList());
    }
    
    public override string ToString()
    {
        return Value.ToString();
    }
    
    public BadArray GetGroupNames()
    {
        return new BadArray(Value.GetGroupNames().Select(x => (BadObject)x).ToList());
    }
    
    public BadArray GetGroupNumbers()
    {
        return new BadArray(Value.GetGroupNumbers().Select(x => (BadObject)x).ToList());
    }
    
    public int GroupNumberFromName(string name)
    {
        return Value.GroupNumberFromName(name);
    }
    
    public string GroupNameFromNumber(int number)
    {
        return Value.GroupNameFromNumber(number);
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
        return $"Regex(\"{Value}\")";
    }
}