using System;

namespace BadScript2.Interop.Generator.Model;

/// <summary>
/// Model for a Method.
/// </summary>
public readonly struct MethodModel : IEquatable<MethodModel>
{
    /// <summary>
    /// The C# Method Name.
    /// </summary>
    public readonly string MethodName;
    /// <summary>
    /// The Api Method Name.(if not specified in the attribute, the method name is used)
    /// </summary>
    public readonly string ApiMethodName;
    /// <summary>
    /// The Return Type of the Method.
    /// </summary>
    public readonly string ReturnType;
    /// <summary>
    /// The Description of the Return Value.
    /// </summary>
    public readonly string ReturnDescription;
    /// <summary>
    /// The Description of the Method.
    /// </summary>
    public readonly string Description;
    /// <summary>
    /// Indicates if the Method has a void return type.
    /// </summary>
    public readonly bool IsVoidReturn;
    /// <summary>
    /// The Parameters of the Method.
    /// </summary>
    public readonly ParameterModel[] Parameters;
    /// <summary>
    /// Indicates if the Method allows a native return type.
    /// </summary>
    public readonly bool AllowNativeReturn;

    /// <summary>
    /// Constructs a new MethodModel instance.
    /// </summary>
    /// <param name="methodName">The C# Method Name.</param>
    /// <param name="apiMethodName">The Api Method Name.(if not specified in the attribute, the method name is used)</param>
    /// <param name="returnType">The Return Type of the Method.</param>
    /// <param name="description">The Description of the Method.</param>
    /// <param name="parameters">The Parameters of the Method.</param>
    /// <param name="isVoidReturn">Indicates if the Method has a void return type.</param>
    /// <param name="returnDescription">The Description of the Return Value.</param>
    /// <param name="allowNativeReturn">Indicates if the Method allows a native return type.</param>
    public MethodModel(string methodName,
                       string apiMethodName,
                       string returnType,
                       string description,
                       ParameterModel[] parameters,
                       bool isVoidReturn,
                       string returnDescription,
                       bool allowNativeReturn)
    {
        MethodName = methodName;
        ApiMethodName = apiMethodName;
        ReturnType = returnType;
        Description = description;
        Parameters = parameters;
        IsVoidReturn = isVoidReturn;
        ReturnDescription = returnDescription;
        AllowNativeReturn = allowNativeReturn;
    }
    
    /// <inheritdoc />
    public bool Equals(MethodModel other)
    {
        return MethodName == other.MethodName &&
               ApiMethodName == other.ApiMethodName &&
               ReturnType == other.ReturnType &&
               ReturnDescription == other.ReturnDescription &&
               Description == other.Description &&
               IsVoidReturn == other.IsVoidReturn &&
               Parameters.Equals(other.Parameters) &&
               AllowNativeReturn == other.AllowNativeReturn;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is MethodModel other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = MethodName.GetHashCode();
            hashCode = (hashCode * 397) ^ ApiMethodName.GetHashCode();
            hashCode = (hashCode * 397) ^ ReturnType.GetHashCode();
            hashCode = (hashCode * 397) ^ ReturnDescription.GetHashCode();
            hashCode = (hashCode * 397) ^ Description.GetHashCode();
            hashCode = (hashCode * 397) ^ IsVoidReturn.GetHashCode();
            hashCode = (hashCode * 397) ^ Parameters.GetHashCode();
            hashCode = (hashCode * 397) ^ AllowNativeReturn.GetHashCode();

            return hashCode;
        }
    }

    /// <summary>
    /// Equality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are equal.</returns>
    public static bool operator ==(MethodModel left, MethodModel right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator for the Model.
    /// </summary>
    /// <param name="left">Left side of the operator.</param>
    /// <param name="right">Right side of the operator.</param>
    /// <returns>True if the two models are not equal.</returns>
    public static bool operator !=(MethodModel left, MethodModel right)
    {
        return !left.Equals(right);
    }
}