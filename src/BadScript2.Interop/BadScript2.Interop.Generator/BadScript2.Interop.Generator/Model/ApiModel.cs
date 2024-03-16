using System;

using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator.Model;

public readonly struct ApiModel : IEquatable<ApiModel>
{
    public readonly string Namespace;
    public readonly string ClassName;
    public readonly string ApiName;
    public readonly MethodModel[] Methods;
    public readonly bool ConstructorPrivate;
    public readonly Diagnostic[] Diagnostics;

    public ApiModel(string ns, string className, MethodModel[] methods, string apiName, bool constructorPrivate, Diagnostic[] diagnostics)
    {
        Namespace = ns;
        ClassName = className;
        Methods = methods;
        ApiName = apiName;
        ConstructorPrivate = constructorPrivate;
        Diagnostics = diagnostics;
    }

    public bool Equals(ApiModel other)
    {
        return Namespace == other.Namespace &&
               ClassName == other.ClassName &&
               ApiName == other.ApiName &&
               Methods.Equals(other.Methods) &&
               ConstructorPrivate == other.ConstructorPrivate &&
               Diagnostics.Equals(other.Diagnostics);
    }

    public override bool Equals(object? obj)
    {
        return obj is ApiModel other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Namespace.GetHashCode();
            hashCode = hashCode * 397 ^ ClassName.GetHashCode();
            hashCode = hashCode * 397 ^ ApiName.GetHashCode();
            hashCode = hashCode * 397 ^ Methods.GetHashCode();
            hashCode = hashCode * 397 ^ ConstructorPrivate.GetHashCode();
            hashCode = hashCode * 397 ^ Diagnostics.GetHashCode();

            return hashCode;
        }
    }

    public static bool operator ==(ApiModel left, ApiModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ApiModel left, ApiModel right)
    {
        return !left.Equals(right);
    }
}