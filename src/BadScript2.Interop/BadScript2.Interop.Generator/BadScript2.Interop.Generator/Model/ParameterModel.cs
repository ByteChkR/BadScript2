using System;

namespace BadScript2.Interop.Generator.Model;

public readonly struct ParameterModel : IEquatable<ParameterModel>
{
    public readonly bool IsContext;
    public readonly string? Name;
    public readonly string? Description;
    public readonly string? Type;
    public readonly string? CsharpType;
    public readonly bool IsNullable;
    public readonly bool HasDefaultValue;
    public readonly string? DefaultValue;
    public readonly bool IsRestArgs;

    public ParameterModel(bool isContext,
                          bool hasDefaultValue = false,
                          string? defaultValue = null,
                          string? name = null,
                          string? description = null,
                          string? type = null,
                          string? csharpType = null,
                          bool isNullable = false,
                          bool isRestArgs = false)
    {
        IsContext = isContext;
        HasDefaultValue = hasDefaultValue;
        DefaultValue = defaultValue;
        Name = name;
        Description = description;
        Type = type;
        CsharpType = csharpType;
        IsNullable = isNullable;
        IsRestArgs = isRestArgs;
    }

    public bool Equals(ParameterModel other)
    {
        return IsContext == other.IsContext &&
               Name == other.Name &&
               Description == other.Description &&
               Type == other.Type &&
               CsharpType == other.CsharpType &&
               IsNullable == other.IsNullable &&
               HasDefaultValue == other.HasDefaultValue &&
               DefaultValue == other.DefaultValue &&
               IsRestArgs == other.IsRestArgs;
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
            hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (CsharpType != null ? CsharpType.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ IsNullable.GetHashCode();
            hashCode = (hashCode * 397) ^ HasDefaultValue.GetHashCode();
            hashCode = (hashCode * 397) ^ (DefaultValue != null ? DefaultValue.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ IsRestArgs.GetHashCode();

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