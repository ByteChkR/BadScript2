using System.Text;

using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser;

public class BadMetaData : BadObject
{
    public static readonly BadMetaData Empty = new BadMetaData("", "", new Dictionary<string, string>());
    public readonly string Description;
    public readonly Dictionary<string, string> ParameterDescriptions;
    public readonly string ReturnDescription;

    public BadMetaData(string description, string returnDescription, Dictionary<string, string> parameterDescriptions)
    {
        Description = description;
        ReturnDescription = returnDescription;
        ParameterDescriptions = parameterDescriptions;
    }

    public override BadClassPrototype GetPrototype()
    {
        throw new NotSupportedException();
    }

    public override bool HasProperty(BadObject propName)
    {
        return propName is IBadString { Value: "Description" or "Return" or "Parameters" } || base.HasProperty(propName);
    }

    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        if (propName is IBadString s)
        {
            if (s.Value == "Description")
            {
                return BadObjectReference.Make("BadMetaData.Description", () => Description);
            }

            if (s.Value == "Return")
            {
                return BadObjectReference.Make("BadMetaData.Return", () => ReturnDescription);
            }

            if (s.Value == "Parameters")
            {
                return BadObjectReference.Make("BadMetaData.Parameters", () => new BadTable(ParameterDescriptions.ToDictionary(x => (BadObject)x.Key, x => (BadObject)x.Value)));
            }
        }

        return base.GetProperty(propName, caller);
    }

    public override string ToSafeString(List<BadObject> done)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(Description);

        foreach (KeyValuePair<string, string> kv in ParameterDescriptions)
        {
            sb.AppendLine($"Parameter {kv.Key}: {kv.Value}");
        }

        sb.AppendLine("Returns: " + ReturnDescription);

        return sb.ToString();
    }
}