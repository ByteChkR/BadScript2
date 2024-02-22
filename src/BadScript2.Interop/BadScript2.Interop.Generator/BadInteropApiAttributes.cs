using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BadScript2.Interop.Generator;

public static class BadInteropApiAttributes
{
    public const string INTEROP_API_ATTRIBUTE = "BadScript2.Interop.BadInteropApiAttribute";
    public const string INTEROP_METHOD_ATTRIBUTE = "BadScript2.Interop.BadMethodAttribute";
    public const string INTEROP_METHOD_PARAMETER_ATTRIBUTE = "BadScript2.Interop.BadParameterAttribute";
    public const string INTEROP_METHOD_RETURN_ATTRIBUTE = "BadScript2.Interop.BadReturnAttribute";

    public static AttributeData? GetInteropApiAttribute(this ITypeSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_API_ATTRIBUTE);
    }

    public static AttributeData? GetInteropMethodAttribute(this IMethodSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_METHOD_ATTRIBUTE);
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
        return symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == name);
    }

    public static AttributeData? GetReturnTypeAttribute(this IMethodSymbol symbol, string name)
    {
        return symbol.GetReturnTypeAttributes().FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == name);
    }

    public static void RegisterAttributeSource(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            "BadInteropAttributes.cs",
            SourceText.From(
                @"using System;
namespace BadScript2.Interop
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class BadInteropApiAttribute : Attribute
    {
        public string Name { get; }
        public string? Description { get; }
        public BadInteropApiAttribute(string name, string? description = null)
        {
            Name = name;
            Description = description;
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
    
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class BadParameterAttribute : Attribute
    {
        public string Name { get; }
        public string? Description { get; }
        public BadParameterAttribute(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }
    }
    
    [AttributeUsage(AttributeTargets.ReturnValue)]
    internal sealed class BadReturnAttribute : Attribute
    {
        public string? Description { get; }
        public BadReturnAttribute(string? description = null)
        {
            Description = description;
        }
    }
}",
                Encoding.UTF8
            )
        );
    }
}