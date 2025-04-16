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
            SourceText.From("""
                            using BadScript2.Runtime.Interop;
                            using BadScript2.Runtime.Objects;
                            using System;

                            namespace BadScript2.Interop;

                            /// <summary>
                            ///     Defines meta data for an Interop API.
                            /// </summary>
                            [AttributeUsage(AttributeTargets.Class)]
                            internal sealed class BadInteropApiAttribute : Attribute
                            {
                                /// <summary>
                                ///     Creates a new instance of the <see cref="BadInteropApiAttribute" /> class.
                                /// </summary>
                                public BadInteropApiAttribute(string? name = null, bool constructorPrivate = false)
                                {
                                    Name = name;
                                    ConstructorPrivate = constructorPrivate;
                                }

                                /// <summary>
                                ///     The name of the API. (if not specified in the attribute, the C# class name is used)
                                /// </summary>
                                public string? Name { get; }

                                /// <summary>
                                ///     Indicates that the source generator should generate a private constructor for the API.
                                /// </summary>
                                public bool ConstructorPrivate { get; }
                            }

                            /// <summary>
                            ///     Defines meta data for an object.
                            /// </summary>
                            [AttributeUsage(AttributeTargets.Class)]
                            internal sealed class BadInteropObjectAttribute : Attribute
                            {
                                /// <summary>
                                ///     Creates a new instance of the <see cref="BadInteropObjectAttribute" /> class.
                                /// </summary>
                                public BadInteropObjectAttribute(string? typeName = null, Type? baseType = null)
                                {
                                    TypeName = typeName;
                                    BaseType = baseType;
                                }

                                /// <summary>
                                ///     The Type Name of the Object. (if not specified in the attribute, the C# class name is used)
                                /// </summary>
                                public string? TypeName { get; }

                                /// <summary>
                                ///     The Base Type of the Object.
                                /// </summary>
                                public Type? BaseType { get; }
                            }

                            /// <summary>
                            ///     Defines meta data for a method of an object.
                            /// </summary>
                            [AttributeUsage(AttributeTargets.Method)]
                            internal sealed class BadMethodAttribute : Attribute
                            {
                                /// <summary>
                                ///     Creates a new instance of the <see cref="BadMethodAttribute" /> class.
                                /// </summary>
                                public BadMethodAttribute(string? name = null, string? description = null)
                                {
                                    Name = name;
                                    Description = description;
                                }

                                /// <summary>
                                ///     The name of the method. (if not specified in the attribute, the C# method name is used)
                                /// </summary>
                                public string? Name { get; }

                                /// <summary>
                                ///     Describes the method.
                                /// </summary>
                                public string? Description { get; }
                            }

                            /// <summary>
                            ///     Defines meta data for a property of a method.
                            /// </summary>
                            [AttributeUsage(AttributeTargets.Property)]
                            internal sealed class BadPropertyAttribute : Attribute
                            {
                                /// <summary>
                                ///     Creates a new instance of the <see cref="BadPropertyAttribute" /> class.
                                /// </summary>
                                /// <param name="name">The name of the property. (if not specified in the attribute, the C# property name is used)</param>
                                /// <param name="description">Describes the property.</param>
                                /// <param name="readOnly">Indicates if the property is read only.</param>
                                /// <param name="allowNativeTypes">Allows the property to be a native type.</param>
                                public BadPropertyAttribute(string? name = null, string? description = null, bool readOnly = false,
                                    bool allowNativeTypes = false)
                                {
                                    Name = name;
                                    Description = description;
                                    ReadOnly = readOnly;
                                    AllowNativeTypes = allowNativeTypes;
                                }

                                /// <summary>
                                ///     The name of the property. (if not specified in the attribute, the C# property name is used)
                                /// </summary>
                                public string? Name { get; }

                                /// <summary>
                                ///     Describes the property.
                                /// </summary>
                                public string? Description { get; }

                                /// <summary>
                                ///     Indicates if the property is read only.
                                /// </summary>
                                public bool ReadOnly { get; }

                                /// <summary>
                                ///     Allows the property to be a native type.
                                /// </summary>
                                public bool AllowNativeTypes { get; }
                            }

                            /// <summary>
                            ///     Defines meta data for a parameter of a method.
                            /// </summary>
                            [AttributeUsage(AttributeTargets.Parameter)]
                            internal sealed class BadParameterAttribute : Attribute
                            {
                                /// <summary>
                                ///     Creates a new instance of the <see cref="BadParameterAttribute" /> class.
                                /// </summary>
                                /// <param name="name">The name of the parameter. (if not specified in the attribute, the C# parameter name is used)</param>
                                /// <param name="description">Describes the parameter.</param>
                                /// <param name="nativeType">The native type of the parameter.</param>
                                public BadParameterAttribute(string? name = null, string? description = null, string? nativeType = null)
                                {
                                    Name = name;
                                    Description = description;
                                    NativeType = nativeType;
                                }

                                /// <summary>
                                ///     The name of the parameter. (if not specified in the attribute, the C# parameter name is used)
                                /// </summary>
                                public string? Name { get; }

                                /// <summary>
                                ///     Describes the parameter.
                                /// </summary>
                                public string? Description { get; }

                                /// <summary>
                                ///     The native type of the parameter.
                                /// </summary>
                                public string? NativeType { get; }
                            }

                            /// <summary>
                            ///     Defines meta data for the return value of a method.
                            /// </summary>
                            [AttributeUsage(AttributeTargets.ReturnValue)]
                            internal sealed class BadReturnAttribute : Attribute
                            {
                                /// <summary>
                                ///     Creates a new instance of the <see cref="BadReturnAttribute" /> class.
                                /// </summary>
                                /// <param name="description">The description of the return value.</param>
                                /// <param name="allowNativeTypes">Allows the return type to be a native type.</param>
                                public BadReturnAttribute(string? description = null, bool allowNativeTypes = false)
                                {
                                    Description = description;
                                    AllowNativeTypes = allowNativeTypes;
                                }

                                /// <summary>
                                ///     The description of the return value.
                                /// </summary>
                                public string? Description { get; }

                                /// <summary>
                                ///     Allows the return type to be a native type.
                                /// </summary>
                                public bool AllowNativeTypes { get; }
                            }

                            /// <summary>
                            ///     Implemented by the Source Generator to generate the Interop API.
                            /// </summary>
                            internal abstract class BadAutoGeneratedInteropApi : BadInteropApi
                            {
                                /// <summary>
                                ///     Creates a new instance of the <see cref="BadAutoGeneratedInteropApi" /> class.
                                /// </summary>
                                /// <param name="apiName">The name of the api.</param>
                                protected BadAutoGeneratedInteropApi(string apiName) : base(apiName)
                                {
                                }

                                /// <summary>
                                ///     Gets invoked as the last step after the api was loaded.
                                /// </summary>
                                /// <param name="target">The target table to add the api to</param>
                                protected virtual void AdditionalData(BadTable target)
                                {
                                }
                            }
                            """,
                Encoding.UTF8
            )
        );
    }
}