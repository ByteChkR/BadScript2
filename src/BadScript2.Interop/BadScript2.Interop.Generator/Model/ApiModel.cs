using System;

namespace BadScript2.Interop.Generator;

public readonly struct ApiModel : IEquatable<ApiModel>
{
    public readonly string Namespace;
    public readonly string ClassName;
    public readonly string ApiName;
    public readonly MethodModel[] Methods;

    public ApiModel(string ns, string className, MethodModel[] methods, string apiName)
    {
        Namespace = ns;
        ClassName = className;
        Methods = methods;
        ApiName = apiName;
    }

    public bool Equals(ApiModel other)
    {
        return Namespace == other.Namespace && ClassName == other.ClassName && ApiName == other.ApiName && Methods.Equals(other.Methods);
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