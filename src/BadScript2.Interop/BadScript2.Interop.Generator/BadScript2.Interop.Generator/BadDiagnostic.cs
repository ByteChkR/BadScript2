using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator;

public static class BadDiagnostic
{
    public static readonly DiagnosticDescriptor CanNotConvertType = new DiagnosticDescriptor("BAS0001",
        "Can not convert type",
        "Can not convert type {0}",
        "BadScript2.Interop.Generator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor CanNotStringifyDefaultValue = new DiagnosticDescriptor("BAS0002",
        "Can not stringify default value",
        "Can not stringify default value {0}",
        "BadScript2.Interop.Generator",
        DiagnosticSeverity.Error,
        true
    );
}