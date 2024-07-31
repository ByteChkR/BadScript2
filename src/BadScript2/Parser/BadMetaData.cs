using System.Text;

using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

/// <summary>
/// Contains the Parser for the BadScript2 Language
/// </summary>
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
    public BadMetaData(string description,
                       string returnDescription,
                       string returnType,
                       Dictionary<string, BadParameterMetaData> parameterDescriptions)
    {
        Description = description;
        ReturnDescription = returnDescription;
        ParameterDescriptions = parameterDescriptions;
        ReturnType = returnType;
    }

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return propName is "Description" or "Return" or "Parameters" ||
               base.HasProperty(propName, caller);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        switch (propName)
        {
            case "Description":
                return BadObjectReference.Make("BadMetaData.Description", () => Description);
            case "Return":
                return BadObjectReference.Make("BadMetaData.Return",
                                               () => new BadTable(new Dictionary<string, BadObject>
                                                                  {
                                                                      { "Type", ReturnType },
                                                                      { "Description", ReturnDescription },
                                                                  }
                                                                 )
                                              );
            case "Parameters":
                return BadObjectReference.Make("BadMetaData.Parameters",
                                               () => new BadTable(ParameterDescriptions.ToDictionary(x => x.Key,
                                                                       x => (BadObject)
                                                                           new BadTable(new Dictionary<string,
                                                                                       BadObject>
                                                                                   {
                                                                                       { "Type", x.Value.Type },
                                                                                       {
                                                                                           "Description",
                                                                                           x.Value.Description
                                                                                       },
                                                                                   }
                                                                               )
                                                                      )
                                                                 )
                                              );
        }

        return base.GetProperty(propName, caller);
    }

    /// <inheritdoc />
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