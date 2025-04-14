using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator;

/// <summary>
/// Contains the Diagnostic Descriptors for the BadScript2.Interop Generator
/// </summary>
public static class BadDiagnostic
{
    /// <summary>
    /// Diagnostic Descriptor for when a type can not be converted
    /// </summary>
    public static readonly DiagnosticDescriptor CanNotConvertType = new DiagnosticDescriptor("BAS0001",
        "Can not convert type",
        "Can not convert type {0}",
        "BadScript2.Interop.Generator",
        DiagnosticSeverity.Error,
        true
    );

    /// <summary>
    /// Diagnostic Descriptor for when a default value can not be stringified
    /// </summary>
    public static readonly DiagnosticDescriptor CanNotStringifyDefaultValue = new DiagnosticDescriptor("BAS0002",
        "Can not stringify default value",
        "Can not stringify default value {0}",
        "BadScript2.Interop.Generator",
        DiagnosticSeverity.Error,
        true
    );
}