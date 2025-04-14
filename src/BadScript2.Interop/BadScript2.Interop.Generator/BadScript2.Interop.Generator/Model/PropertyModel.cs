using System;

namespace BadScript2.Interop.Generator.Model;

/// <summary>
/// The Property Model.
/// </summary>
public readonly struct PropertyModel : IEquatable<PropertyModel>
{
    /// <summary>
    /// The Property Name(if not specified in the attribute, the c# property name is used)
    /// </summary>
    public readonly string ApiParameterName;
    /// <summary>
    /// The C# Property Name
    /// </summary>
    public readonly string ParameterName;
    /// <summary>
    /// The BadScript2 Type of the Property.
    /// </summary>
    public readonly string Type;
    /// <summary>
    /// The C# Type of the Property.
    /// </summary>
    public readonly string ParameterType;
    /// <summary>
    /// The Description of the Property.
    /// </summary>
    public readonly string Description;
    /// <summary>
    /// Indicates if the Property is ReadOnly.
    /// </summary>
    public readonly bool IsReadOnly;
    /// <summary>
    /// Constructs a new PropertyModel instance.
    /// </summary>
    /// <param name="parameterName">The C# Property Name</param>
    /// <param name="apiParameterName">The Property Name(if not specified in the attribute, the c# property name is used)</param>
    /// <param name="type">The BadScript2 Type of the Property.</param>
    /// <param name="description">The Description of the Property.</param>
    /// <param name="isReadOnly">Indicates if the Property is ReadOnly.</param>
    /// <param name="parameterType">Indicates if the Property is ReadOnly.</param>
    public PropertyModel(string parameterName, string apiParameterName, string type, string description, bool isReadOnly, string parameterType)
    {
        ParameterName = parameterName;
        ApiParameterName = apiParameterName;
        Type = type;
        Description = description;
        IsReadOnly = isReadOnly;
        ParameterType = parameterType;
    }

    /// <inheritdoc />
    public bool Equals(PropertyModel other)
    {
        return ApiParameterName == other.ApiParameterName && ParameterName == other.ParameterName && Type == other.Type && ParameterType == other.ParameterType && Description == other.Description && IsReadOnly == other.IsReadOnly;
    }
    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is PropertyModel other && Equals(other);
    }
    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = ApiParameterName.GetHashCode();
            hashCode = (hashCode * 397) ^ ParameterName.GetHashCode();
            hashCode = (hashCode * 397) ^ Type.GetHashCode();
            hashCode = (hashCode * 397) ^ ParameterType.GetHashCode();
            hashCode = (hashCode * 397) ^ Description.GetHashCode();
            hashCode = (hashCode * 397) ^ IsReadOnly.GetHashCode();
            return hashCode;
        }
    }
    /// <summary>
    /// Equality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are equal.</returns>
    public static bool operator ==(PropertyModel left, PropertyModel right)
    {
        return left.Equals(right);
    }
    /// <summary>
    /// Inequality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are not equal.</returns>
    public static bool operator !=(PropertyModel left, PropertyModel right)
    {
        return !left.Equals(right);
    }
}