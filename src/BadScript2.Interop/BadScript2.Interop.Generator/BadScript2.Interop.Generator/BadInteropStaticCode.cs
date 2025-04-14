using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BadScript2.Interop.Generator;

/// <summary>
/// Static Code for the BadScript2.Interop Generator
/// </summary>
public static class BadInteropStaticCode
{
    /// <summary>
    /// Full Name of the Interop API Attribute
    /// </summary>
    public const string INTEROP_API_ATTRIBUTE = "BadScript2.Interop.BadInteropApiAttribute";
    /// <summary>
    /// Full Name of the Interop Object Attribute
    /// </summary>
    public const string INTEROP_OBJECT_ATTRIBUTE = "BadScript2.Interop.BadInteropObjectAttribute";
    /// <summary>
    /// Full Name of the Interop Property Attribute
    /// </summary>
    public const string INTEROP_PROPERTY_ATTRIBUTE = "BadScript2.Interop.BadPropertyAttribute";
    /// <summary>
    /// Full Name of the Interop Method Attribute
    /// </summary>
    public const string INTEROP_METHOD_ATTRIBUTE = "BadScript2.Interop.BadMethodAttribute";
    /// <summary>
    /// Full Name of the Interop Method Parameter Attribute
    /// </summary>
    public const string INTEROP_METHOD_PARAMETER_ATTRIBUTE = "BadScript2.Interop.BadParameterAttribute";
    /// <summary>
    /// Full Name of the Interop Method Return Attribute
    /// </summary>
    public const string INTEROP_METHOD_RETURN_ATTRIBUTE = "BadScript2.Interop.BadReturnAttribute";

    /// <summary>
    /// Creates a Diagnostic for the given Symbol
    /// </summary>
    /// <param name="descriptor">The Diagnostic Descriptor</param>
    /// <param name="symbol">The Symbol to create the Diagnostic for</param>
    /// <param name="args">The Arguments to format the Diagnostic with</param>
    /// <returns></returns>
    public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor,
                                              ISymbol symbol,
                                              params object?[]? args)
    {
        return Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args);
    }

    /// <summary>
    /// Creates a Diagnostic for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to create the Diagnostic for</param>
    /// <param name="descriptor">The Diagnostic Descriptor</param>
    /// <param name="args">The Arguments to format the Diagnostic with</param>
    /// <returns></returns>
    public static Diagnostic CreateDiagnostic(this ISymbol symbol,
                                              DiagnosticDescriptor descriptor,
                                              params object?[]? args)
    {
        return Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args);
    }

    /// <summary>
    /// Returns the API AttributeData for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to get the Attribute for</param>
    /// <returns>The AttributeData or null if it does not exist</returns>
    public static AttributeData? GetInteropApiAttribute(this ITypeSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_API_ATTRIBUTE);
    }
    /// <summary>
    /// Returns the API AttributeData for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to get the Attribute for</param>
    /// <returns>The AttributeData or null if it does not exist</returns>
    public static AttributeData? GetInteropObjectAttribute(this ITypeSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_OBJECT_ATTRIBUTE);
    }

    /// <summary>
    /// Returns the API AttributeData for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to get the Attribute for</param>
    /// <returns>The AttributeData or null if it does not exist</returns>
    public static AttributeData? GetInteropMethodAttribute(this IMethodSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_METHOD_ATTRIBUTE);
    }
    /// <summary>
    /// Returns the API AttributeData for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to get the Attribute for</param>
    /// <returns>The AttributeData or null if it does not exist</returns>
    public static AttributeData? GetInteropPropertyAttribute(this IPropertySymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_PROPERTY_ATTRIBUTE);
    }

    /// <summary>
    /// Returns the API AttributeData for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to get the Attribute for</param>
    /// <returns>The AttributeData or null if it does not exist</returns>
    public static AttributeData? GetReturnTypeAttribute(this IMethodSymbol symbol)
    {
        return symbol.GetReturnTypeAttribute(INTEROP_METHOD_RETURN_ATTRIBUTE);
    }

    /// <summary>
    /// Returns the API AttributeData for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to get the Attribute for</param>
    /// <returns>The AttributeData or null if it does not exist</returns>
    public static AttributeData? GetParameterAttribute(this IParameterSymbol symbol)
    {
        return symbol.GetAttribute(INTEROP_METHOD_PARAMETER_ATTRIBUTE);
    }

    /// <summary>
    /// Returns the API AttributeData for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to get the Attribute for</param>
    /// <param name="name">The Name of the Attribute</param>
    /// <returns>The AttributeData or null if it does not exist</returns>
    public static AttributeData? GetAttribute(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes()
                     .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == name);
    }

    /// <summary>
    /// Returns the API AttributeData for the given Symbol
    /// </summary>
    /// <param name="symbol">The Symbol to get the Attribute for</param>
    /// <param name="name">The Name of the Attribute</param>
    /// <returns>The AttributeData or null if it does not exist</returns>
    public static AttributeData? GetReturnTypeAttribute(this IMethodSymbol symbol, string name)
    {
        return symbol.GetReturnTypeAttributes()
                     .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == name);
    }

    /// <summary>
    /// Registers the Attribute Source for the Generator
    /// </summary>
    /// <param name="context">The Context to register the Source in</param>
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
        public Type? BaseType { get; }
        public BadInteropObjectAttribute(string? typeName = null, Type? baseType = null)
        {
            TypeName = typeName;
            BaseType = baseType;
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