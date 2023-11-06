using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects;

/// <summary>
///     Stores Meta Information about a Property
/// </summary>
public class BadPropertyInfo
{
	/// <summary>
	///     Creates a new Property Info
	/// </summary>
	/// <param name="type">(optional) Type</param>
	/// <param name="isReadOnly">Is the property readonly?</param>
	public BadPropertyInfo(BadClassPrototype? type = null, bool isReadOnly = false)
    {
        Type = type;
        IsReadOnly = isReadOnly;
    }

	/// <summary>
	///     The (optional) Type used for typechecking if a value gets assigned to this property
	/// </summary>
	public BadClassPrototype? Type { get; }

	/// <summary>
	///     Indicates if this property is read only
	/// </summary>
	public bool IsReadOnly { get; set; }
}