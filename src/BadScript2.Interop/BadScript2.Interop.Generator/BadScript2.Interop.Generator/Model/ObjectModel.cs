using System;
using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator.Model;

public readonly struct ObjectModel : IEquatable<ObjectModel>
{
    public readonly string Namespace;
    public readonly string ClassName;
    public readonly string ObjectName;
    public readonly MethodModel Constructor;
    public readonly MethodModel[] Methods;
    public readonly PropertyModel[] Properties;
    public readonly Diagnostic[] Diagnostics;
    
    public ObjectModel(string ns,
        string className,
        MethodModel[] methods,
        string objectName,
        Diagnostic[] diagnostics, 
        PropertyModel[] properties, MethodModel constructor)
    {
        Namespace = ns;
        ClassName = className;
        Methods = methods;
        ObjectName = objectName;
        Diagnostics = diagnostics;
        Properties = properties;
        Constructor = constructor;
    }
    public bool Equals(ObjectModel other)
    {
        return Namespace == other.Namespace && ClassName == other.ClassName && ObjectName == other.ObjectName && Constructor.Equals(other.Constructor) && Methods.Equals(other.Methods) && Properties.Equals(other.Properties) && Diagnostics.Equals(other.Diagnostics);
    }
    public override bool Equals(object? obj)
    {
        return obj is ObjectModel other && Equals(other);
    }
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
    public static bool operator ==(ObjectModel left, ObjectModel right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(ObjectModel left, ObjectModel right)
    {
        return !left.Equals(right);
    }
}