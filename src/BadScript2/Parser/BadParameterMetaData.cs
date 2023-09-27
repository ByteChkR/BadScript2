namespace BadScript2.Parser;

/// <summary>
///     Implements a Meta Data container for a parameter
/// </summary>
public class BadParameterMetaData
{
	/// <summary>
	///     The Description of the Parameter
	/// </summary>
	public readonly string Description;

	/// <summary>
	///     The Type of the Parameter
	/// </summary>
	public readonly string Type;

	/// <summary>
	///     Creates a new Parameter Meta Data Object
	/// </summary>
	/// <param name="type">The Type of the Parameter</param>
	/// <param name="description">The Description of the Parameter</param>
	public BadParameterMetaData(string type, string description)
    {
        Type = type;
        Description = description;
    }
}