using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using BadScript2.Interop.Generator.Model;

using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator.Interop;

/// <summary>
/// The Model Builder for the Interop API
/// </summary>
public class BadInteropApiModelBuilder
{
    /// <summary>
    /// List of Diagnostics
    /// </summary>
    private readonly List<Diagnostic> m_Diagnostics = new List<Diagnostic>();

    /// <summary>
    /// Adds a Diagnostic to the list of Diagnostics
    /// </summary>
    /// <param name="diagnostic">The Diagnostic to add</param>
    private void AddDiagnostic(Diagnostic diagnostic)
    {
        m_Diagnostics.Add(diagnostic);
    }

    /// <summary>
    /// Finds all methods in the given INamedTypeSymbol
    /// </summary>
    /// <param name="api">The INamedTypeSymbol to search in</param>
    /// <returns>List of methods</returns>
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
    /// Generates an ApiModel from the given INamedTypeSymbol
    /// </summary>
    /// <param name="api">The INamedTypeSymbol to generate the model from</param>
    /// <returns>The generated ApiModel</returns>
    /// <exception cref="Exception">Gets thrown if the ApiModel could not be generated</exception>
    public ApiModel GenerateModel(INamedTypeSymbol api)
    {
        IEnumerable<IMethodSymbol> methods = FindMethods(api);
        AttributeData? apiAttribute = api.GetInteropApiAttribute();

        if (apiAttribute == null)
        {
            throw new Exception("BadInteropApiAttribute not found");
        }

        string apiName = api.Name;
        bool constructorPrivate = false;

        if (apiAttribute.ConstructorArguments.Length > 0)
        {
            apiName = apiAttribute.ConstructorArguments[0]
                                  .Value?.ToString() ??
                      apiName;
            bool? priv = apiAttribute.ConstructorArguments[1].Value as bool?;
            constructorPrivate = apiAttribute.ConstructorArguments.Length > 1 && priv != null && priv.Value;
        }

        MethodModel[] methodModels = GenerateMethodModels(methods)
            .ToArray();
        Diagnostic[] diagnostics = Array.Empty<Diagnostic>();

        if (m_Diagnostics.Count != 0)
        {
            diagnostics = m_Diagnostics.ToArray();
            m_Diagnostics.Clear();
        }

        return new ApiModel(api.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                                                                         .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle
                                                                                 .Omitted
                                                                             )
                                                                    )!,
                            api.Name,
                            methodModels,
                            apiName,
                            constructorPrivate,
                            diagnostics
                           );
    }

    /// <summary>
    /// Escapes the description for the generated code
    /// </summary>
    /// <param name="str">The description to escape</param>
    /// <returns>The escaped description</returns>
    private string EscapeDescription(string str)
    {
        return str.Replace("\\", "\\\\")
                  .Replace("\"", "\\\"")
                  .Replace("\n", "\\n");
    }

    /// <summary>
    /// Generates the ParameterModel for the given IMethodSymbol
    /// </summary>
    /// <param name="method">The IMethodSymbol to generate the ParameterModel for</param>
    /// <returns>The generated ParameterModel</returns>
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
    /// Stringifies the default value of the given object
    /// </summary>
    /// <param name="obj">The object to stringify</param>
    /// <param name="symbol">The symbol to use for the diagnostic</param>
    /// <returns>The stringified default value</returns>
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
    /// Generates the MethodModels for the given IEnumerable of IMethodSymbols
    /// </summary>
    /// <param name="symbols">The IEnumerable of IMethodSymbols to generate the MethodModels for</param>
    /// <returns>The generated MethodModels</returns>
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
    /// Converts the given ITypeSymbol to a BadScript2 Type Name
    /// </summary>
    /// <param name="type">The ITypeSymbol to convert</param>
    /// <param name="allowAny">If any is allowed</param>
    /// <param name="sourceSymbol">The symbol to use for the diagnostic</param>
    /// <returns>The converted type name</returns>
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