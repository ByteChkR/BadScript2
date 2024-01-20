using System.Text;

using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser;

/// <summary>
///     Implements a Meta Data container for an expression
/// </summary>
public class BadMetaData : BadObject
{
    /// <summary>
    ///     An empty Meta Data object
    /// </summary>
    public static readonly BadMetaData Empty =
        new BadMetaData("", "", "any", new Dictionary<string, BadParameterMetaData>());

    /// <summary>
    ///     The Description of the Expression
    /// </summary>
    public readonly string Description;

    /// <summary>
    ///     The Description of the Function Parameters
    /// </summary>
    public readonly Dictionary<string, BadParameterMetaData> ParameterDescriptions;

    /// <summary>
    ///     The Description of the Return Value
    /// </summary>
    public readonly string ReturnDescription;

    /// <summary>
    ///     The Return Type of the Expression
    /// </summary>
    public readonly string ReturnType;

    /// <summary>
    ///     Creates a new Meta Data Object
    /// </summary>
    /// <param name="description">The Description of the Expression</param>
    /// <param name="returnDescription">The Description of the Return Value</param>
    /// <param name="returnType">The Return Type of the Expression</param>
    /// <param name="parameterDescriptions">The Description of the Function Parameters</param>
    public BadMetaData(
        string description,
        string returnDescription,
        string returnType,
        Dictionary<string, BadParameterMetaData> parameterDescriptions)
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

    public override bool HasProperty(BadObject propName, BadScope? caller = null)
    {
        return propName is IBadString
               {
                   Value: "Description" or "Return" or "Parameters",
               } ||
               base.HasProperty(propName, caller);
    }

    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        if (propName is not IBadString s)
        {
            return base.GetProperty(propName, caller);
        }

        switch (s.Value)
        {
            case "Description":
                return BadObjectReference.Make("BadMetaData.Description", () => Description);
            case "Return":
                return BadObjectReference.Make(
                    "BadMetaData.Return",
                    () => new BadTable(
                        new Dictionary<BadObject, BadObject>
                        {
                            {
                                "Type", ReturnType
                            },
                            {
                                "Description", ReturnDescription
                            },
                        }
                    )
                );
            case "Parameters":
                return BadObjectReference.Make(
                    "BadMetaData.Parameters",
                    () => new BadTable(
                        ParameterDescriptions.ToDictionary(
                            x => (BadObject)x.Key,
                            x => (BadObject)new BadTable(
                                new Dictionary<BadObject, BadObject>
                                {
                                    {
                                        "Type", x.Value.Type
                                    },
                                    {
                                        "Description", x.Value.Description
                                    },
                                }
                            )
                        )
                    )
                );
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