﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BadScript2.Interop.Generator.Model;
using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator.Interop;

/// <summary>
/// Generates the ObjectModel for the BadScript2.Interop API.
/// </summary>
public class BadInteropObjectModelBuilder
{

    /// <summary>
    /// List of Diagnostics that were created during the generation process.
    /// </summary>
    private readonly List<Diagnostic> m_Diagnostics = new List<Diagnostic>();

    /// <summary>
    /// Adds a Diagnostic to the list of Diagnostics.
    /// </summary>
    /// <param name="diagnostic">The Diagnostic to add.</param>
    private void AddDiagnostic(Diagnostic diagnostic)
    {
        m_Diagnostics.Add(diagnostic);
    }

    /// <summary>
    /// Finds all methods in the given INamedTypeSymbol that have the InteropMethodAttribute.
    /// </summary>
    /// <param name="api">The INamedTypeSymbol to search.</param>
    /// <returns>An IEnumerable of IMethodSymbol that have the InteropMethodAttribute.</returns>
    private IEnumerable<IMethodSymbol> FindMethods(INamedTypeSymbol api)
    {
        IEnumerable<IMethodSymbol> methods = api.GetMembers()
            .Where(x => x is IMethodSymbol)
            .Cast<IMethodSymbol>();

        foreach (IMethodSymbol method in methods)
        {
            AttributeData? attribute = method.GetInteropMethodAttribute();

            if (attribute != null)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Finds all properties in the given INamedTypeSymbol that have the InteropPropertyAttribute.
    /// </summary>
    /// <param name="api">The INamedTypeSymbol to search.</param>
    /// <returns>An IEnumerable of IPropertySymbol that have the InteropPropertyAttribute.</returns>
    private IEnumerable<IPropertySymbol> FindProperties(INamedTypeSymbol api)
    {
        IEnumerable<IPropertySymbol> methods = api.GetMembers()
            .Where(x => x is IPropertySymbol)
            .Cast<IPropertySymbol>();

        foreach (IPropertySymbol method in methods)
        {
            AttributeData? attribute = method.GetInteropPropertyAttribute();

            if (attribute != null)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Generates the ObjectModel for the given INamedTypeSymbol.
    /// </summary>
    /// <param name="api">The INamedTypeSymbol to generate the ObjectModel for.</param>
    /// <returns>The generated ObjectModel.</returns>
    /// <exception cref="Exception">Gets thrown if the InteropObjectAttribute is not found.</exception>
    public ObjectModel GenerateModel(INamedTypeSymbol api)
    {
        IEnumerable<IMethodSymbol> methods = FindMethods(api);
        IEnumerable<IPropertySymbol> properties = FindProperties(api);
        AttributeData? apiAttribute = api.GetInteropObjectAttribute();

        if (apiAttribute == null)
        {
            throw new Exception("BadInteropObjectAttribute not found");
        }

        string apiName = api.Name;
        string? baseClassName = null;
        if (apiAttribute.ConstructorArguments.Length > 0)
        {
            apiName = apiAttribute.ConstructorArguments[0]
                          .Value?.ToString() ??
                      apiName;
        }
        if(apiAttribute.ConstructorArguments.Length > 1)
        {
            var t = apiAttribute.ConstructorArguments[1].Value;
            baseClassName = $"{t}";
        }

        MethodModel[] methodModels = GenerateMethodModels(methods)
            .ToArray();
        PropertyModel[] propertyModels = GeneratePropertyModels(properties).ToArray();
        Diagnostic[] diagnostics = [];

        var ctorModel = GenerateConstructorModel(api.Constructors.FirstOrDefault(x => x.DeclaredAccessibility == Accessibility.Public && !x.IsStatic));
        

        if (m_Diagnostics.Count != 0)
        {
            diagnostics = m_Diagnostics.ToArray();
            m_Diagnostics.Clear();
        }

        return new ObjectModel(api.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle
                    .Omitted
                )
            )!,
            api.Name,
            methodModels,
            apiName,
            diagnostics,
            propertyModels,
            ctorModel,
            baseClassName
        );
    }

    /// <summary>
    /// Escapes the description string for use in the generated code.
    /// </summary>
    /// <param name="str">The description string to escape.</param>
    /// <returns>The escaped description string.</returns>
    private string EscapeDescription(string str)
    {
        return str.Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n");
    }

    /// <summary>
    /// Generates the parameter models for the given method.
    /// </summary>
    /// <param name="method">The method to generate the parameter model for.</param>
    /// <returns>An IEnumerable of ParameterModel.</returns>
    private IEnumerable<ParameterModel> GenerateParameterModel(IMethodSymbol method)
    {
        foreach (IParameterSymbol symbol in method.Parameters.Where(x => x.Ordinal >= 0)
                     .OrderBy(x => x.Ordinal))
        {
            //if parameter is first parameter and its of type BadScript2.Runtime.BadExecutionContext
            if (symbol.Ordinal == 0 && symbol.Type.ToDisplayString() == "BadScript2.Runtime.BadExecutionContext")
            {
                yield return new ParameterModel(true);
            }
            else
            {
                AttributeData? attribute = symbol.GetParameterAttribute();
                string? name = symbol.Name;
                string? description = null;
                bool isNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
                string type = ConvertType(symbol.Type, true, symbol);

                if (attribute != null)
                {
                    ImmutableArray<TypedConstant> cargs = attribute.ConstructorArguments;

                    name = cargs.Length > 0
                        ? cargs[0]
                              .Value?.ToString() ??
                          name
                        : name;

                    description = cargs.Length > 1
                        ? cargs[1]
                            .Value?.ToString()
                        : null;

                    if (description != null)
                    {
                        description = EscapeDescription(description);
                    }

                    type = cargs.Length > 2
                        ? cargs[2]
                              .Value?.ToString() ??
                          type
                        : type;
                }

                bool hasDefaultValue = symbol.HasExplicitDefaultValue;
                string? defaultValue = null;

                if (hasDefaultValue)
                {
                    defaultValue = StringifyDefaultValue(symbol.ExplicitDefaultValue, symbol);
                }

                bool isRestArgs = symbol.IsParams;

                yield return new ParameterModel(false,
                    hasDefaultValue,
                    defaultValue,
                    name,
                    description,
                    type,
                    symbol.Type.ToDisplayString(),
                    isNullable,
                    isRestArgs
                );
            }
        }
    }

    /// <summary>
    /// Stringifies the default value of the given object.
    /// </summary>
    /// <param name="obj">The object to stringify.</param>
    /// <param name="symbol">The symbol to use for diagnostics.</param>
    /// <returns>>The stringified default value.</returns>
    private string StringifyDefaultValue(object? obj, ISymbol symbol)
    {
        switch (obj)
        {
            case string str:
                return $"\"{str}\"";
            case char c:
                return $"'{c}'";
            case bool b:
                return b.ToString()
                    .ToLower();
            case float f:
                return $"{f}f";
            case double d:
                return $"{d}d";
            case decimal m:
                return $"{m}m";
            case int i:
                return i.ToString();
            case long l:
                return $"{l}L";
            case uint u:
                return $"{u}u";
            case ulong ul:
                return $"{ul}ul";
            case short s:
                return $"{s}s";
            case ushort us:
                return $"{us}us";
            case byte by:
                return $"{by}b";
            case sbyte sb:
                return $"{sb}sb";
            case Enum e:
                return $"{e.GetType().Name}.{e}";
            case Type t:
                return $"typeof({t.Name})";
            case null:
                return "null";
            default:
            {
                AddDiagnostic(symbol.CreateDiagnostic(BadDiagnostic.CanNotStringifyDefaultValue, obj));

                return "null";
            }
        }
    }

    /// <summary>
    /// Generates the property models for the given properties.
    /// </summary>
    /// <param name="symbols">The properties to generate the property model for.</param>
    /// <returns>An IEnumerable of PropertyModel.</returns>
    private IEnumerable<PropertyModel> GeneratePropertyModels(IEnumerable<IPropertySymbol> symbols)
    {
        foreach (IPropertySymbol symbol in symbols)
        {
            AttributeData? attribute = symbol.GetInteropPropertyAttribute();

            if (attribute == null)
            {
                continue;
            }

            ImmutableArray<TypedConstant> cargs = attribute.ConstructorArguments;

            //The api name, if its not provided, use the method name
            string name = cargs.Length > 0
                ? cargs[0]
                      .Value?.ToString() ??
                  symbol.Name
                : symbol.Name;

            //The description, if its not provided, use null
            string description = EscapeDescription(cargs.Length > 1
                ? cargs[1]
                      .Value?.ToString() ??
                  string.Empty
                : string.Empty
            );
            bool readOnly = cargs.Length > 2 && (cargs[2].Value as bool? ?? false);

            bool allowNativeReturn = cargs.Length > 3 && (cargs[3].Value as bool? ?? false);

            //The return type, if its not provided, use the symbol's return type
            string returnType = ConvertType(symbol.Type, allowNativeReturn, symbol);
            
            var model = new PropertyModel(symbol.Name, name, returnType, description, readOnly || symbol.SetMethod == null, symbol.Type.ToDisplayString());

            yield return model;
        }
    }

    /// <summary>
    /// Generates the constructor model for the given method symbol.
    /// </summary>
    /// <param name="symbol">The method symbol to generate the constructor model for.</param>
    /// <returns>>The generated constructor model.</returns>
    private MethodModel GenerateConstructorModel(IMethodSymbol? symbol)
    {
        if (symbol == null)
        {
            return new MethodModel(".ctor", ".ctor", "", "", [], false, "", false);
        }

        return new MethodModel(".ctor", ".ctor", "", "", GenerateParameterModel(symbol).ToArray(), false, "", false);
    }
    
    /// <summary>
    /// Generates the method models for the given method symbols.
    /// </summary>
    /// <param name="symbols">The method symbols to generate the method model for.</param>
    /// <returns>>An IEnumerable of MethodModel.</returns>
    private IEnumerable<MethodModel> GenerateMethodModels(IEnumerable<IMethodSymbol> symbols)
    {
        foreach (IMethodSymbol symbol in symbols)
        {
            AttributeData? attribute = symbol.GetInteropMethodAttribute();

            if (attribute == null)
            {
                continue;
            }

            ImmutableArray<TypedConstant> cargs = attribute.ConstructorArguments;

            //The api name, if its not provided, use the method name
            string name = cargs.Length > 0
                ? cargs[0]
                      .Value?.ToString() ??
                  symbol.Name
                : symbol.Name;

            //The description, if its not provided, use null
            string description = EscapeDescription(cargs.Length > 1
                ? cargs[1]
                      .Value?.ToString() ??
                  string.Empty
                : string.Empty
            );

            //Check if the symbol is a void return
            bool isVoidReturn = symbol.ReturnsVoid;
            AttributeData? returnAttribute = symbol.GetReturnTypeAttribute();
            string returnDescription = string.Empty;
            bool allowNativeReturn = false;

            if (returnAttribute != null)
            {
                if (returnAttribute.ConstructorArguments.Length > 0)
                {
                    returnDescription = EscapeDescription(returnAttribute.ConstructorArguments[0]
                                                              .Value?.ToString() ??
                                                          string.Empty
                    );
                }

                if (returnAttribute.ConstructorArguments.Length > 1)
                {
                    allowNativeReturn = (bool)returnAttribute.ConstructorArguments[1].Value!;
                }
            }

            //The return type, if its not provided, use the symbol's return type
            string returnType = isVoidReturn ? "any" : ConvertType(symbol.ReturnType, allowNativeReturn, symbol);

            MethodModel model = new MethodModel(symbol.Name,
                name,
                returnType,
                description,
                GenerateParameterModel(symbol)
                    .ToArray(),
                isVoidReturn,
                returnDescription,
                allowNativeReturn
            );

            yield return model;
        }
    }

    /// <summary>
    /// Converts the given type symbol to a BadScript2 Type Name
    /// </summary>
    /// <param name="type">The type symbol to convert.</param>
    /// <param name="allowAny">If any type is allowed.</param>
    /// <param name="sourceSymbol">The symbol to use for diagnostics.</param>
    /// <returns>>The converted type name.</returns>
    private string ConvertType(ITypeSymbol type, bool allowAny, ISymbol sourceSymbol)
    {
        if (type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            //Unwrap nullable value types
            type = type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        }

        //If type is string, return "string"
        if (type.SpecialType == SpecialType.System_String)
        {
            return "string";
        }

        //if type is bool return "bool"
        if (type.SpecialType == SpecialType.System_Boolean)
        {
            return "bool";
        }

        //if type is numeric type return "num"
        if (type.SpecialType == SpecialType.System_Byte ||
            type.SpecialType == SpecialType.System_SByte ||
            type.SpecialType == SpecialType.System_Int16 ||
            type.SpecialType == SpecialType.System_UInt16 ||
            type.SpecialType == SpecialType.System_Int32 ||
            type.SpecialType == SpecialType.System_UInt32 ||
            type.SpecialType == SpecialType.System_Int64 ||
            type.SpecialType == SpecialType.System_UInt64 ||
            type.SpecialType == SpecialType.System_Single ||
            type.SpecialType == SpecialType.System_Double ||
            type.SpecialType == SpecialType.System_Decimal)
        {
            return "num";
        }

        //if type is array or list or ilist, return "array"
        if (type is IArrayTypeSymbol ||
            type.AllInterfaces.Any(x => x.ToDisplayString() == "System.Collections.Generic.IList<T>") ||
            type.AllInterfaces.Any(x => x.ToDisplayString() == "System.Collections.IList<T>"))
        {
            return "Array";
        }

        //if type is BadObject, return "any"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.BadObject")
        {
            return "any";
        }

        if (type.ToDisplayString() == "BadScript2.Interop.Common.Task.BadTask")
        {
            return "Task";
        }

        //If type is BadTable, return "Table"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.BadTable")
        {
            return "Table";
        }

        //If type is BadArray return "Array"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.BadArray")
        {
            return "Array";
        }

        //If type is BadFunction return "Function"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Functions.BadFunction")
        {
            return "Function";
        }

        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Types.BadClassPrototype")
        {
            return "Type";
        }

        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Error.BadRuntimeError")
        {
            return "Error";
        }

        //If Type is IBadBoolean or BadBoolean return "bool"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.IBadBoolean" ||
            type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.BadBoolean")
        {
            return "bool";
        }

        //If Type is IBadNumber or BadNumber return "num"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.IBadNumber" ||
            type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.BadNumber")
        {
            return "num";
        }

        //If Type is IBadString or BadString return "string"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.IBadString" ||
            type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.BadString")
        {
            return "string";
        }

        //If Type is BadScope return "Scope"
        if (type.ToDisplayString() == "BadScript2.Runtime.BadScope")
        {
            return "Scope";
        }

        if (allowAny)
        {
            return "any";
        }

        AddDiagnostic(sourceSymbol.CreateDiagnostic(BadDiagnostic.CanNotConvertType, type.ToDisplayString()));

        return "any";
    }
}