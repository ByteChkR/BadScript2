using System;

namespace BadScript2.Interop.Generator.Model;

/// <summary>
/// The Parameter Model.
/// </summary>
public readonly struct ParameterModel : IEquatable<ParameterModel>
{
    /// <summary>
    /// Indicates if the parameter is a context parameter.
    /// </summary>
    public readonly bool IsContext;
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public readonly string? Name;
    /// <summary>
    /// The description of the parameter.
    /// </summary>
    public readonly string? Description;
    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public readonly string? Type;
    /// <summary>
    /// The C# type of the parameter.
    /// </summary>
    public readonly string? CsharpType;
    /// <summary>
    /// Indicates if the parameter is nullable.
    /// </summary>
    public readonly bool IsNullable;
    /// <summary>
    /// Indicates if the parameter has a default value.
    /// </summary>
    public readonly bool HasDefaultValue;
    /// <summary>
    /// If the parameter has a default value, this is the default value.
    /// </summary>
    public readonly string? DefaultValue;
    /// <summary>
    /// Indicates if the parameter is a rest parameter.
    /// </summary>
    public readonly bool IsRestArgs;

    /// <summary>
    /// Constructs a new ParameterModel instance.
    /// </summary>
    /// <param name="isContext">Indicates if the parameter is a context parameter.</param>
    /// <param name="hasDefaultValue">Indicates if the parameter has a default value.</param>
    /// <param name="defaultValue">If the parameter has a default value, this is the default value.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="description">The description of the parameter.</param>
    /// <param name="type">The type of the parameter.</param>
    /// <param name="csharpType">The C# type of the parameter.</param>
    /// <param name="isNullable">Indicates if the parameter is nullable.</param>
    /// <param name="isRestArgs">Indicates if the parameter is a rest parameter.</param>
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ParameterModel other && Equals(other);
    }

    /// <inheritdoc />
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

    /// <summary>
    /// Equality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are equal.</returns>
    public static bool operator ==(ParameterModel left, ParameterModel right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are not equal.</returns>
    public static bool operator !=(ParameterModel left, ParameterModel right)
    {
        return !left.Equals(right);
    }
}