using System;

using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator.Model;

/// <summary>
/// API Model.
/// </summary>
public readonly struct ApiModel : IEquatable<ApiModel>
{
    /// <summary>
    /// The Containing Namespace of the API.
    /// </summary>
    public readonly string Namespace;
    /// <summary>
    /// The C# Class Name of the API.
    /// </summary>
    public readonly string ClassName;
    /// <summary>
    /// The Api Name(if not specified in the attribute, the class name is used).
    /// </summary>
    public readonly string ApiName;
    /// <summary>
    /// The Methods of the API.
    /// </summary>
    public readonly MethodModel[] Methods;
    
    /// <summary>
    /// Indicates if the constructor of the API is private.
    /// </summary>
    public readonly bool ConstructorPrivate;
    
    /// <summary>
    /// The Diagnostics of the API.
    /// </summary>
    public readonly Diagnostic[] Diagnostics;

    /// <summary>
    /// Constructs a new ApiModel instance.
    /// </summary>
    /// <param name="ns">The Namespace of the API.</param>
    /// <param name="className">The C# Class Name of the API.</param>
    /// <param name="methods">The Methods of the API.</param>
    /// <param name="apiName">The Api Name(if not specified in the attribute, the class name is used).</param>
    /// <param name="constructorPrivate">Indicates if the constructor of the API is private.</param>
    /// <param name="diagnostics">The Diagnostics of the API.</param>
    public ApiModel(string ns,
                    string className,
                    MethodModel[] methods,
                    string apiName,
                    bool constructorPrivate,
                    Diagnostic[] diagnostics)
    {
        Namespace = ns;
        ClassName = className;
        Methods = methods;
        ApiName = apiName;
        ConstructorPrivate = constructorPrivate;
        Diagnostics = diagnostics;
    }

    /// <inheritdoc />
    public bool Equals(ApiModel other)
    {
        return Namespace == other.Namespace &&
               ClassName == other.ClassName &&
               ApiName == other.ApiName &&
               Methods.Equals(other.Methods) &&
               ConstructorPrivate == other.ConstructorPrivate &&
               Diagnostics.Equals(other.Diagnostics);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ApiModel other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ ClassName.GetHashCode();
            hashCode = (hashCode * 397) ^ ApiName.GetHashCode();
            hashCode = (hashCode * 397) ^ Methods.GetHashCode();
            hashCode = (hashCode * 397) ^ ConstructorPrivate.GetHashCode();
            hashCode = (hashCode * 397) ^ Diagnostics.GetHashCode();

            return hashCode;
        }
    }

    /// <summary>
    /// Equality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are equal.</returns>
    public static bool operator ==(ApiModel left, ApiModel right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are not equal.</returns>
    public static bool operator !=(ApiModel left, ApiModel right)
    {
        return !left.Equals(right);
    }
}