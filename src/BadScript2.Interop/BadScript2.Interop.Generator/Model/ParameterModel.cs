using System;

namespace BadScript2.Interop.Generator;

public readonly struct ParameterModel : IEquatable<ParameterModel>
{
    public readonly bool IsContext;
    public readonly string? Name;
    public readonly string? Description;
    public readonly string? Type;
    public readonly string? CsharpType;
    public readonly bool IsNullable;

    public ParameterModel(bool isContext, string? name = null, string? description = null, string? type = null, string? csharpType = null, bool isNullable = false)
    {
        IsContext = isContext;
        Name = name;
        Description = description;
        Type = type;
        CsharpType = csharpType;
        IsNullable = isNullable;
    }

    public bool Equals(ParameterModel other)
    {
        return IsContext == other.IsContext &&
               Name == other.Name &&
               Description == other.Description &&
               Type == other.Type &&
               CsharpType == other.CsharpType &&
               IsNullable == other.IsNullable;
    }

    public override bool Equals(object? obj)
    {
        return obj is ParameterModel other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = IsContext.GetHashCode();
            hashCode = hashCode * 397 ^ (Name != null ? Name.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (Description != null ? Description.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (Type != null ? Type.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (CsharpType != null ? CsharpType.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ IsNullable.GetHashCode();

            return hashCode;
        }
    }

    public static bool operator ==(ParameterModel left, ParameterModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ParameterModel left, ParameterModel right)
    {
        return !left.Equals(right);
    }
}