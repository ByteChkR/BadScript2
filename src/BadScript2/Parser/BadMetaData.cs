using System.Text;

using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser;


public class BadParameterMetaData
{
    public readonly string Description;
    public readonly string Type;
    public BadParameterMetaData(string type, string description)
    {
        Type = type;
        Description = description;
    }
}
public class BadMetaData : BadObject
{
    public static readonly BadMetaData Empty = new BadMetaData("", "", "any", new Dictionary<string, BadParameterMetaData>());
    public readonly string Description;
    public readonly Dictionary<string, BadParameterMetaData> ParameterDescriptions;
    public readonly string ReturnDescription;
    public readonly string ReturnType;

    public BadMetaData(string description, string returnDescription, string returnType, Dictionary<string, BadParameterMetaData> parameterDescriptions)
    {
        Description = description;
        ReturnDescription = returnDescription;
        ParameterDescriptions = parameterDescriptions;
        ReturnType = returnType;
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
                return BadObjectReference.Make(
                    "BadMetaData.Return",
                    () => new BadTable(
                        new Dictionary<BadObject, BadObject>
                        {
                            { "Type", ReturnType },
                            { "Description", ReturnDescription }
                        }
                    )
                );
            }

            if (s.Value == "Parameters")
            {
                return BadObjectReference.Make(
                    "BadMetaData.Parameters",
                    () => new BadTable(
                        ParameterDescriptions.ToDictionary(
                            x => (BadObject)x.Key,
                            x => (BadObject)new BadTable(new Dictionary<BadObject, BadObject>
                            {
                                { "Type", x.Value.Type }, 
                                { "Description", x.Value.Description }
                            })
                        )
                    )
                );
            }
        }

        return base.GetProperty(propName, caller);
    }

    public override string ToSafeString(List<BadObject> done)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(Description);

        foreach (KeyValuePair<string, BadParameterMetaData> kv in ParameterDescriptions)
        {
            sb.AppendLine($"Parameter {kv.Key} {kv.Value.Type}: {kv.Value.Description}");
        }

        sb.AppendLine("Returns: " + ReturnDescription);

        return sb.ToString();
    }
}