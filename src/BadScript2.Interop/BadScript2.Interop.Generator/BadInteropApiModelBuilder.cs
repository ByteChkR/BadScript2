using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

using BadScript2.Interop.Generator.Model;

using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator;

public static class BadInteropApiModelBuilder
{
    private static IEnumerable<IMethodSymbol> FindMethods(INamedTypeSymbol api)
    {
        IEnumerable<IMethodSymbol> methods = api.GetMembers().Where(x => x is IMethodSymbol).Cast<IMethodSymbol>();
        foreach (IMethodSymbol method in methods)
        {
            AttributeData? attribute = method.GetInteropMethodAttribute();
            if (attribute != null)
            {
                yield return method;
            }
        }
    }

    public static ApiModel GenerateModel(INamedTypeSymbol api)
    {
        IEnumerable<IMethodSymbol> methods = FindMethods(api);
        AttributeData? apiAttribute = api.GetInteropApiAttribute();
        if (apiAttribute == null)
        {
            throw new Exception("BadInteropApiAttribute not found");
        }

        string apiName = api.Name;
        if (apiAttribute.ConstructorArguments.Length > 0)
        {
            apiName = apiAttribute.ConstructorArguments[0].Value?.ToString() ?? apiName;
        }

        return new ApiModel(
            api.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted))!,
            api.Name,
            GenerateMethodModels(methods).ToArray(),
            apiName
        );
    }

    private static string EscapeDescription(string str)
    {
        return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
    }
    
    private static IEnumerable<ParameterModel> GenerateParameterModel(IMethodSymbol method)
    {
        foreach (IParameterSymbol symbol in method.Parameters.Where(x => x.Ordinal >= 0).OrderBy(x => x.Ordinal))
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
                string type = ConvertType(symbol.Type);

                if (attribute != null)
                {
                    ImmutableArray<TypedConstant> cargs = attribute.ConstructorArguments;
                    name = cargs.Length > 0 ? cargs[0].Value?.ToString() ?? name : name;
                    description = cargs.Length > 1 ? cargs[1].Value?.ToString() : null;
                    if (description != null)
                    {
                        description = EscapeDescription(description);
                    }
                }

                bool hasDefaultValue = symbol.HasExplicitDefaultValue;
                string? defaultValue = null;
                if (hasDefaultValue)
                {
                    defaultValue = StringifyDefaultValue(symbol.ExplicitDefaultValue);
                }

                bool isRestArgs = symbol.IsParams;

                yield return new ParameterModel(false, hasDefaultValue, defaultValue, name, description, type, symbol.Type.ToDisplayString(), isNullable, isRestArgs);
            }
        }
    }

    private static string StringifyDefaultValue(object? obj)
    {
        return obj switch
        {
            string str => $"\"{str}\"",
            char c => $"'{c}'",
            bool b => b.ToString().ToLower(),
            float f => $"{f}f",
            double d => $"{d}d",
            decimal m => $"{m}m",
            int i => i.ToString(),
            long l => $"{l}L",
            uint u => $"{u}u",
            ulong ul => $"{ul}ul",
            short s => $"{s}s",
            ushort us => $"{us}us",
            byte by => $"{by}b",
            sbyte sb => $"{sb}sb",
            Enum e => $"{e.GetType().Name}.{e}",
            Type t => $"typeof({t.Name})",
            null => "null",
            _ => throw new NotSupportedException($"Type {obj.GetType()} is not supported"),
        };
    }

    private static IEnumerable<MethodModel> GenerateMethodModels(IEnumerable<IMethodSymbol> symbols)
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
            string name = cargs.Length > 0 ? cargs[0].Value?.ToString() ?? symbol.Name : symbol.Name;

            //The description, if its not provided, use null
            string description = EscapeDescription(cargs.Length > 1 ? cargs[1].Value?.ToString() ?? string.Empty : string.Empty);

            //Check if the symbol is a void return
            bool isVoidReturn = symbol.ReturnsVoid;

            //The return type, if its not provided, use the symbol's return type
            string returnType = isVoidReturn ? "any" : ConvertType(symbol.ReturnType);

            AttributeData? returnAttribute = symbol.GetReturnTypeAttribute();
            string returnDescription = EscapeDescription(returnAttribute?.ConstructorArguments[0].Value?.ToString() ?? string.Empty);

            MethodModel model = new MethodModel(symbol.Name, name, returnType, description, GenerateParameterModel(symbol).ToArray(), isVoidReturn, returnDescription);

            yield return model;
        }
    }

    private static string ConvertType(ITypeSymbol type)
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
        
        if(type.ToDisplayString() == "BadScript2.Runtime.Objects.Types.BadClass")
        {
            return "Class";
        }
        if(type.ToDisplayString() == "BadScript2.Runtime.Objects.Error.BadRuntimeError")
        {
            return "Error";
        }

        //If Type is IBadBoolean or BadBoolean return "bool"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.IBadBoolean" || type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.BadBoolean")
        {
            return "bool";
        }

        //If Type is IBadNumber or BadNumber return "num"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.IBadNumber" || type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.BadNumber")
        {
            return "num";
        }

        //If Type is IBadString or BadString return "string"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.IBadString" || type.ToDisplayString() == "BadScript2.Runtime.Objects.Native.BadString")
        {
            return "string";
        }

        //If Type is BadScope return "Scope"
        if (type.ToDisplayString() == "BadScript2.Runtime.BadScope")
        {
            return "Scope";
        }

        throw new NotSupportedException($"Type {type.ToDisplayString()} is not supported");
    }
}