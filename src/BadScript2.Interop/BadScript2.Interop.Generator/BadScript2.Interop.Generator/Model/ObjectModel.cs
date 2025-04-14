using System;
using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator.Model;

/// <summary>
/// Object Model.
/// </summary>
public readonly struct ObjectModel : IEquatable<ObjectModel>
{
    /// <summary>
    /// Namespace of the Object.
    /// </summary>
    public readonly string Namespace;
    /// <summary>
    /// The C# Class Name of the Object.
    /// </summary>
    public readonly string ClassName;
    /// <summary>
    /// The Object Name(if not specified in the attribute, the class name is used).
    /// </summary>
    public readonly string ObjectName;
    /// <summary>
    /// The Base Class Name of the Object.
    /// </summary>
    public readonly string? BaseClassName;
    /// <summary>
    /// The Constructor Model
    /// </summary>
    public readonly MethodModel Constructor;
    /// <summary>
    /// The Methods of the Object.
    /// </summary>
    public readonly MethodModel[] Methods;
    /// <summary>
    /// The Properties of the Object.
    /// </summary>
    public readonly PropertyModel[] Properties;
    /// <summary>
    /// The Diagnostics of the Object.
    /// </summary>
    public readonly Diagnostic[] Diagnostics;
    
    /// <summary>
    /// Constructs a new ObjectModel instance.
    /// </summary>
    /// <param name="ns">The Namespace of the Object.</param>
    /// <param name="className">The C# Class Name of the Object.</param>
    /// <param name="methods">The Methods of the Object.</param>
    /// <param name="objectName">The Object Name(if not specified in the attribute, the class name is used).</param>
    /// <param name="diagnostics">The Diagnostics of the Object.</param>
    /// <param name="properties">The Properties of the Object.</param>
    /// <param name="constructor">The Constructor Model</param>
    /// <param name="baseClassName">The Base Class Name of the Object.</param>
    public ObjectModel(string ns,
        string className,
        MethodModel[] methods,
        string objectName,
        Diagnostic[] diagnostics, 
        PropertyModel[] properties, MethodModel constructor, string? baseClassName)
    {
        Namespace = ns;
        ClassName = className;
        Methods = methods;
        ObjectName = objectName;
        Diagnostics = diagnostics;
        Properties = properties;
        Constructor = constructor;
        BaseClassName = baseClassName;
    }
    /// <inheritdoc />
    public bool Equals(ObjectModel other)
    {
        return Namespace == other.Namespace && ClassName == other.ClassName && ObjectName == other.ObjectName && Constructor.Equals(other.Constructor) && Methods.Equals(other.Methods) && Properties.Equals(other.Properties) && Diagnostics.Equals(other.Diagnostics);
    }
    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ObjectModel other && Equals(other);
    }
    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ ClassName.GetHashCode();
            hashCode = (hashCode * 397) ^ ObjectName.GetHashCode();
            hashCode = (hashCode * 397) ^ Constructor.GetHashCode();
            hashCode = (hashCode * 397) ^ Methods.GetHashCode();
            hashCode = (hashCode * 397) ^ Properties.GetHashCode();
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
    public static bool operator ==(ObjectModel left, ObjectModel right)
    {
        return left.Equals(right);
    }
    
    /// <summary>
    /// Inequality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are not equal.</returns>
    public static bool operator !=(ObjectModel left, ObjectModel right)
    {
        return !left.Equals(right);
    }
}