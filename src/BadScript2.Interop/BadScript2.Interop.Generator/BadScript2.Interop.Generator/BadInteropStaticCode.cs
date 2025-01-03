using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BadScript2.Interop.Generator;

public static class BadInteropStaticCode
{
    public const string INTEROP_API_ATTRIBUTE = "BadScript2.Interop.BadInteropApiAttribute";
    public const string INTEROP_OBJECT_ATTRIBUTE = "BadScript2.Interop.BadInteropObjectAttribute";
    public const string INTEROP_PROPERTY_ATTRIBUTE = "BadScript2.Interop.BadPropertyAttribute";
    public const string INTEROP_METHOD_ATTRIBUTE = "BadScript2.Interop.BadMethodAttribute";
    public const string INTEROP_METHOD_PARAMETER_ATTRIBUTE = "BadScript2.Interop.BadParameterAttribute";
    public const string INTEROP_METHOD_RETURN_ATTRIBUTE = "BadScript2.Interop.BadReturnAttribute";

    public static DiagnosticDescriptor CreateDescriptor(string id,
                                                        string title,
                                                        string messageFormat,
                                                        string category,
                                                        DiagnosticSeverity severity)
    {
        return new DiagnosticDescriptor(id, title, messageFormat, category, severity, true);
    }

    public static Diagnostic CreateDiagnostic(this ISymbol symbol,
                                              string id,
                                              string title,
                                              string messageFormat,
                                              string category,
                                              DiagnosticSeverity severity,
                                              params object?[]? args)
    {
        return CreateDescriptor(id, title, messageFormat, category, severity)
            .CreateDiagnostic(symbol, args);
    }

    public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor,
                                              ISymbol symbol,
                                              params object?[]? args)
    {
        return Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args);
    }

    public static Diagnostic CreateDiagnostic(this ISymbol symbol,
                                              DiagnosticDescriptor descriptor,
                                              params object?[]? args)
    {
        return Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args);
    }

    public static AttributeData? GetInteropApiAttribute(this ITypeSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_API_ATTRIBUTE);
    }
    public static AttributeData? GetInteropObjectAttribute(this ITypeSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_OBJECT_ATTRIBUTE);
    }

    public static AttributeData? GetInteropMethodAttribute(this IMethodSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_METHOD_ATTRIBUTE);
    }
    public static AttributeData? GetInteropPropertyAttribute(this IPropertySymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_PROPERTY_ATTRIBUTE);
    }

    public static AttributeData? GetReturnTypeAttribute(this IMethodSymbol symbol)
    {
        return symbol.GetReturnTypeAttribute(INTEROP_METHOD_RETURN_ATTRIBUTE);
    }

    public static AttributeData? GetParameterAttribute(this IParameterSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_METHOD_PARAMETER_ATTRIBUTE);
    }

    public static AttributeData? GetAttribute(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes()
                     .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == name);
    }

    public static AttributeData? GetReturnTypeAttribute(this IMethodSymbol symbol, string name)
    {
        return symbol.GetReturnTypeAttributes()
                     .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == name);
    }

    public static void RegisterAttributeSource(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("BadInteropAttributes.cs",
                          SourceText.From(@"using System;
namespace BadScript2.Interop
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class BadInteropApiAttribute : Attribute
    {
        public string? Name { get; }
        public bool ConstructorPrivate { get; }
        public BadInteropApiAttribute(string? name = null, bool constructorPrivate = false)
        {
            Name = name;
            ConstructorPrivate = constructorPrivate;
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class BadInteropObjectAttribute : Attribute
    {
        public string? TypeName { get; }
        public BadInteropObjectAttribute(string? typeName = null)
        {
            TypeName = typeName;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class BadMethodAttribute : Attribute
    {
        public string? Name { get; }
        public string? Description { get; }
        public BadMethodAttribute(string? name = null, string? description = null)
        {
            Name = name;
            Description = description;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class BadPropertyAttribute : Attribute
    {
        public string? Name { get; }
        public string? Description { get; }
        public bool ReadOnly { get; }
        public bool AllowNativeTypes { get; }
        public BadPropertyAttribute(string? name = null, string? description = null, bool readOnly = false, bool allowNativeTypes = false)
        {
            Name = name;
            Description = description;
            ReadOnly = readOnly;
            AllowNativeTypes = allowNativeTypes;
        }
    }
    
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class BadParameterAttribute : Attribute
    {
        public string? Name { get; }
        public string? Description { get; }
        public string? NativeType { get; }
        public BadParameterAttribute(string? name = null, string? description = null, string? nativeType = null)
        {
            Name = name;
            Description = description;
            NativeType = nativeType;
        }
    }
    
    [AttributeUsage(AttributeTargets.ReturnValue)]
    internal sealed class BadReturnAttribute : Attribute
    {
        public string? Description { get; }
        public bool AllowNativeTypes { get; }
        public BadReturnAttribute(string? description = null, bool allowNativeTypes = false)
        {
            Description = description;
            AllowNativeTypes = allowNativeTypes;
        }
    }

    internal abstract class BadAutoGeneratedInteropApi : BadScript2.Runtime.Interop.BadInteropApi
    {
        protected BadAutoGeneratedInteropApi(string apiName) : base(apiName) { }

        protected virtual void AdditionalData(BadScript2.Runtime.Objects.BadTable target) { }
    }

}",
                                          Encoding.UTF8
                                         )
                         );
    }
}