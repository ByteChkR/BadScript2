using System;

namespace BadScript2.Interop.Generator;

public readonly struct MethodModel : IEquatable<MethodModel>
{
    public readonly string MethodName;
    public readonly string ApiMethodName;
    public readonly string ReturnType;
    public readonly string ReturnDescription;
    public readonly string Description;
    public readonly bool IsVoidReturn;
    public readonly ParameterModel[] Parameters;

    public MethodModel(string methodName, string apiMethodName, string returnType, string description, ParameterModel[] parameters, bool isVoidReturn, string returnDescription)
    {
        MethodName = methodName;
        ApiMethodName = apiMethodName;
        ReturnType = returnType;
        Description = description;
        Parameters = parameters;
        IsVoidReturn = isVoidReturn;
        ReturnDescription = returnDescription;
    }

    public bool Equals(MethodModel other)
    {
        return MethodName == other.MethodName &&
               ApiMethodName == other.ApiMethodName &&
               ReturnType == other.ReturnType &&
               ReturnDescription == other.ReturnDescription &&
               Description == other.Description &&
               IsVoidReturn == other.IsVoidReturn &&
               Parameters.Equals(other.Parameters);
    }

    public override bool Equals(object? obj)
    {
        return obj is MethodModel other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = MethodName.GetHashCode();
            hashCode = hashCode * 397 ^ ApiMethodName.GetHashCode();
            hashCode = hashCode * 397 ^ ReturnType.GetHashCode();
            hashCode = hashCode * 397 ^ ReturnDescription.GetHashCode();
            hashCode = hashCode * 397 ^ Description.GetHashCode();
            hashCode = hashCode * 397 ^ IsVoidReturn.GetHashCode();
            hashCode = hashCode * 397 ^ Parameters.GetHashCode();

            return hashCode;
        }
    }

    public static bool operator ==(MethodModel left, MethodModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MethodModel left, MethodModel right)
    {
        return !left.Equals(right);
    }
}