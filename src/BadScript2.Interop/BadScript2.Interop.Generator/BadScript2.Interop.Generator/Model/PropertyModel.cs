using System;

namespace BadScript2.Interop.Generator.Model;

public readonly struct PropertyModel : IEquatable<PropertyModel>
{
    public readonly string ApiParameterName;
    public readonly string ParameterName;
    public readonly string Type;
    public readonly string ParameterType;
    public readonly string Description;
    public readonly bool IsReadOnly;
    public PropertyModel(string parameterName, string apiParameterName, string type, string description, bool isReadOnly, string parameterType)
    {
        ParameterName = parameterName;
        ApiParameterName = apiParameterName;
        Type = type;
        Description = description;
        IsReadOnly = isReadOnly;
        ParameterType = parameterType;
    }

    public bool Equals(PropertyModel other)
    {
        return ApiParameterName == other.ApiParameterName && ParameterName == other.ParameterName && Type == other.Type && ParameterType == other.ParameterType && Description == other.Description && IsReadOnly == other.IsReadOnly;
    }
    public override bool Equals(object? obj)
    {
        return obj is PropertyModel other && Equals(other);
    }
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
    public static bool operator ==(PropertyModel left, PropertyModel right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(PropertyModel left, PropertyModel right)
    {
        return !left.Equals(right);
    }
}