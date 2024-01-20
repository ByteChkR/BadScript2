/// <summary>
/// Contains the Interop Reflection Classes for the BadScript2 Language
/// </summary>
namespace BadScript2.Runtime.Interop.Reflection;

/// <summary>
///     Extensions for Reflection
/// </summary>
public static class BadReflectionExtensions
{
    /// <summary>
    /// Returns true if the Type is a Numeric Type
    /// </summary>
    /// <param name="t">The Type to check</param>
    /// <returns>True if the Type is a Numeric Type</returns>
    public static bool IsNumericType(this Type t)
    {
        switch (Type.GetTypeCode(t))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.DBNull:
            case TypeCode.Empty:
            case TypeCode.Object:
            case TypeCode.String:
            default:
                return false;
        }
    }
}