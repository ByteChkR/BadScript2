#nullable enable

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BadScript2.Interop.Generator;

[Generator]
public class BadInteropApiGenerator : IIncrementalGenerator
{
    public const string INTEROP_API_ATTRIBUTE = "BadScript2.Interop.BadInteropApiAttribute";
    public const string INTEROP_METHOD_ATTRIBUTE = "BadScript2.Interop.BadMethodAttribute";
    public const string INTEROP_METHOD_PARAMETER_ATTRIBUTE = "BadScript2.Interop.BadParameterAttribute";
    public const string INTEROP_METHOD_RETURN_ATTRIBUTE = "BadScript2.Interop.BadReturnAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            static postInitializationContext =>
                postInitializationContext.AddSource(
                    "BadInteropAttributes.cs",
                    SourceText.From(
                        """
                        using System;
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
                        }
                        """,
                        Encoding.UTF8
                    )
                )
        );

        IncrementalValuesProvider<ApiModel> pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            INTEROP_API_ATTRIBUTE,
            static (syntaxNode, cancellationToken) => syntaxNode is ClassDeclarationSyntax,
            static (context, cancellationToken) =>
            {
                INamedTypeSymbol api = (INamedTypeSymbol)context.TargetSymbol;
                IEnumerable<IMethodSymbol> methods = FindMethods(api);
                string apiName = api.GetAttributes()
                                     .First(x => x.AttributeClass?.ToDisplayString() == INTEROP_API_ATTRIBUTE)
                                     .ConstructorArguments[0]
                                     .Value?.ToString() ??
                                 api.Name;

                return new ApiModel(
                    api.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted))!,
                    api.Name,
                    GenerateMethodModels(methods).ToArray(),
                    apiName
                );
            }
        );

        context.RegisterSourceOutput(
            pipeline,
            static (context, model) =>
            {
                SourceText sourceText = SourceText.From(
                    GenerateModelSource(model),
                    Encoding.UTF8
                );

                context.AddSource($"{model.ClassName}.g.cs", sourceText);
            }
        );
    }

    private static string GenerateInvocation(MethodModel method)
    {
        StringBuilder sb = new StringBuilder();

        if (method.IsVoidReturn)
        {
            sb.Append("{");
        }
        else
        {
            sb.Append("BadObject.Wrap(");
        }

        sb.Append($"{method.MethodName}(");
        List<string> args = new List<string>();
        for (int i = 0; i < method.Parameters.Length; i++)
        {
            ParameterModel parameter = method.Parameters[i];
            if (parameter.IsContext)
            {
                args.Add("ctx");
            }
            else
            {
                args.Add($"GetParameter<{parameter.CsharpType}>(args, {i})");
            }
        }

        sb.Append(string.Join(", ", args));
        sb.Append(")");


        if (method.IsVoidReturn)
        {
            sb.Append("; return BadObject.Null; }");
        }
        else
        {
            sb.Append(")");
        }

        return sb.ToString();
    }

    private static string GenerateParameterSource(ParameterModel model)
    {
        return $"new BadFunctionParameter(\"{model.Name}\", false, {model.IsNullable.ToString().ToLower()}, false, null, BadNativeClassBuilder.GetNative(\"{model.Name}\"))";
    }

    private static void GenerateMethodSource(IndentedTextWriter sb, ApiModel apiModel, MethodModel method)
    {
        sb.WriteLine("target.SetProperty(");
        sb.Indent++;
        sb.WriteLine($"\"{method.ApiMethodName}\",");
        sb.WriteLine("new BadInteropFunction(");
        sb.Indent++;
        sb.WriteLine($"\"{method.ApiMethodName}\",");
        sb.WriteLine($"(ctx, args) => {GenerateInvocation(method)},");
        sb.WriteLine("false,");
        sb.WriteLine($"BadNativeClassBuilder.GetNative(\"{method.ReturnType}\"),");
        List<string> parameters = new List<string>();
        for (int i = 0; i < method.Parameters.Length; i++)
        {
            ParameterModel parameter = method.Parameters[i];
            if (parameter.IsContext)
            {
                continue;
            }

            sb.WriteLine(GenerateParameterSource(parameter) + (i == method.Parameters.Length - 1 ? "" : ","));
        }

        sb.Indent--;
        sb.WriteLine(").SetMetaData(");
        sb.Indent++;
        sb.WriteLine("new BadMetaData(");
        sb.Indent++;
        sb.WriteLine($"\"{method.Description}\",");
        sb.WriteLine($"\"{method.ReturnDescription}\",");
        sb.WriteLine($"\"{method.ReturnType}\",");
        sb.WriteLine("new Dictionary<string, BadParameterMetaData>");
        sb.WriteLine("{");
        sb.Indent++;
        foreach (ParameterModel parameter in method.Parameters)
        {
            if (parameter.IsContext)
            {
                continue;
            }

            sb.WriteLine($"{{\"{parameter.Name}\", new BadParameterMetaData(\"{parameter.Type}\", \"{parameter.Description}\")}},");
        }

        sb.Indent--;
        sb.WriteLine("}");
        sb.Indent--;
        sb.WriteLine(")");
        sb.Indent--;
        sb.WriteLine(")");
        sb.Indent--;
        sb.WriteLine(");");
    }

    private static string GenerateModelSource(ApiModel apiModel)
    {
        IndentedTextWriter tw = new IndentedTextWriter(new StringWriter());
        tw.WriteLine("using System.Collections.Generic;");
        tw.WriteLine("using BadScript2.Parser;");
        tw.WriteLine("using BadScript2.Runtime.Interop.Reflection;");
        tw.WriteLine("using BadScript2.Runtime.Objects;");
        tw.WriteLine("using BadScript2.Runtime.Interop.Functions;");
        tw.WriteLine("using BadScript2.Runtime.Objects.Functions;");
        tw.WriteLine("using BadScript2.Runtime.Objects.Types;");
        tw.WriteLine("using BadScript2.Runtime.Interop;");
        tw.WriteLine();
        tw.WriteLine($"namespace {apiModel.Namespace};");
        tw.WriteLine($"partial class {apiModel.ClassName} : BadInteropApi");
        tw.WriteLine("{");
        tw.Indent++;
        tw.WriteLine($"public {apiModel.ClassName}() : base(\"{apiModel.ApiName}\") {{ }}");
        tw.WriteLine();
        tw.WriteLine("protected override void LoadApi(BadTable target)");
        tw.WriteLine("{");
        tw.Indent++;
        tw.WriteLine("T? GetParameter<T>(BadObject[] args, int i) => args.Length>i?args[i].Unwrap<T>():default;");
        foreach (MethodModel method in apiModel.Methods)
        {
            GenerateMethodSource(tw, apiModel, method);
        }

        tw.Indent--;
        tw.WriteLine("}");
        tw.Indent--;
        tw.WriteLine("}");

        tw.Flush();

        string str = tw.InnerWriter.ToString();

        return str;
    }

    private static IEnumerable<IMethodSymbol> FindMethods(INamedTypeSymbol api)
    {
        IEnumerable<IMethodSymbol> methods = api.GetMembers().Where(x => x is IMethodSymbol).Cast<IMethodSymbol>();
        foreach (IMethodSymbol method in methods)
        {
            ImmutableArray<AttributeData> attributes = method.GetAttributes();
            if (attributes.Any(x => x.AttributeClass?.ToDisplayString() == INTEROP_METHOD_ATTRIBUTE))
            {
                yield return method;
            }
        }
    }


    private static string ConvertType(ITypeSymbol type)
    {
        if (type.NullableAnnotation == NullableAnnotation.Annotated && type.IsValueType)
        {
            //Unwrap nullable value types
            type = ((INamedTypeSymbol)type).TypeArguments[0];
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
            return "array";
        }

        //if type is BadObject, return "any"
        if (type.ToDisplayString() == "BadScript2.Runtime.Objects.BadObject")
        {
            return "any";
        }

        throw new NotSupportedException($"Type {type.ToDisplayString()} is not supported");
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
                AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == INTEROP_METHOD_PARAMETER_ATTRIBUTE);
                string? name = symbol.Name;
                string? description = null;
                bool isNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
                string type = ConvertType(symbol.Type);

                if (attribute != null)
                {
                    ImmutableArray<TypedConstant> cargs = attribute.ConstructorArguments;
                    name = cargs.Length > 0 ? cargs[0].Value?.ToString() : name;
                    description = cargs.Length > 1 ? cargs[1].Value?.ToString() : null;
                }

                yield return new ParameterModel(false, name, description, type, symbol.Type.ToDisplayString(), isNullable);
            }
        }
    }

    private static IEnumerable<MethodModel> GenerateMethodModels(IEnumerable<IMethodSymbol> symbols)
    {
        foreach (IMethodSymbol symbol in symbols)
        {
            AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == INTEROP_METHOD_ATTRIBUTE);

            if (attribute == null)
            {
                continue;
            }

            ImmutableArray<TypedConstant> cargs = attribute.ConstructorArguments;

            //The api name, if its not provided, use the method name
            string name = cargs.Length > 0 ? cargs[0].Value?.ToString() ?? symbol.Name : symbol.Name;

            //The description, if its not provided, use null
            string? description = cargs.Length > 1 ? cargs[1].Value?.ToString() ?? string.Empty : string.Empty;

            //Check if the symbol is a void return
            bool isVoidReturn = symbol.ReturnsVoid;

            //The return type, if its not provided, use the symbol's return type
            string returnType = isVoidReturn ? "any" : ConvertType(symbol.ReturnType);

            ImmutableArray<AttributeData> returnAttributes = symbol.GetReturnTypeAttributes();
            AttributeData? returnAttribute = returnAttributes.FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == INTEROP_METHOD_RETURN_ATTRIBUTE);
            string? returnDescription = returnAttribute?.ConstructorArguments[0].Value?.ToString() ?? string.Empty;

            MethodModel model = new MethodModel(symbol.Name, name, returnType, description, GenerateParameterModel(symbol).ToArray(), isVoidReturn, returnDescription);

            yield return model;
        }
    }

    private readonly struct ParameterModel : IEquatable<ParameterModel>
    {
        public readonly bool IsContext;
        public readonly string? Name;
        public readonly string? Description;
        public readonly string? Type;
        public readonly string? CsharpType;
        public readonly bool IsNullable;

        public ParameterModel(bool isContext, string? name = null, string? description = null, string? type = null, string? csharpType = null, bool isNullable = false)
        {
            IsContext = isContext;
            Name = name;
            Description = description;
            Type = type;
            CsharpType = csharpType;
            IsNullable = isNullable;
        }

        public bool Equals(ParameterModel other)
        {
            return IsContext == other.IsContext &&
                   Name == other.Name &&
                   Description == other.Description &&
                   Type == other.Type &&
                   CsharpType == other.CsharpType &&
                   IsNullable == other.IsNullable;
        }

        public override bool Equals(object? obj)
        {
            return obj is ParameterModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IsContext.GetHashCode();
                hashCode = hashCode * 397 ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (CsharpType != null ? CsharpType.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ IsNullable.GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(ParameterModel left, ParameterModel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ParameterModel left, ParameterModel right)
        {
            return !left.Equals(right);
        }
    }

    private readonly struct MethodModel : IEquatable<MethodModel>
    {
        public readonly string MethodName;
        public readonly string ApiMethodName;
        public readonly string ReturnType;
        public readonly string ReturnDescription;
        public readonly string Description;
        public readonly bool IsVoidReturn;
        public readonly ParameterModel[] Parameters;

        public MethodModel(string methodName, string apiMethodName, string returnType, string description, ParameterModel[] parameters, bool isVoidReturn, string returnDescription)
        {
            MethodName = methodName;
            ApiMethodName = apiMethodName;
            ReturnType = returnType;
            Description = description;
            Parameters = parameters;
            IsVoidReturn = isVoidReturn;
            ReturnDescription = returnDescription;
        }

        public bool Equals(MethodModel other)
        {
            return MethodName == other.MethodName &&
                   ApiMethodName == other.ApiMethodName &&
                   ReturnType == other.ReturnType &&
                   ReturnDescription == other.ReturnDescription &&
                   Description == other.Description &&
                   IsVoidReturn == other.IsVoidReturn &&
                   Parameters.Equals(other.Parameters);
        }

        public override bool Equals(object? obj)
        {
            return obj is MethodModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = MethodName.GetHashCode();
                hashCode = hashCode * 397 ^ ApiMethodName.GetHashCode();
                hashCode = hashCode * 397 ^ ReturnType.GetHashCode();
                hashCode = hashCode * 397 ^ ReturnDescription.GetHashCode();
                hashCode = hashCode * 397 ^ Description.GetHashCode();
                hashCode = hashCode * 397 ^ IsVoidReturn.GetHashCode();
                hashCode = hashCode * 397 ^ Parameters.GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(MethodModel left, MethodModel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MethodModel left, MethodModel right)
        {
            return !left.Equals(right);
        }
    }

    private readonly struct ApiModel : IEquatable<ApiModel>
    {
        public readonly string Namespace;
        public readonly string ClassName;
        public readonly string ApiName;
        public readonly MethodModel[] Methods;

        public ApiModel(string ns, string className, MethodModel[] methods, string apiName)
        {
            Namespace = ns;
            ClassName = className;
            Methods = methods;
            ApiName = apiName;
        }

        public bool Equals(ApiModel other)
        {
            return Namespace == other.Namespace && ClassName == other.ClassName && ApiName == other.ApiName && Methods.Equals(other.Methods);
        }

        public override bool Equals(object? obj)
        {
            return obj is ApiModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Namespace.GetHashCode();
                hashCode = hashCode * 397 ^ ClassName.GetHashCode();
                hashCode = hashCode * 397 ^ ApiName.GetHashCode();
                hashCode = hashCode * 397 ^ Methods.GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(ApiModel left, ApiModel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ApiModel left, ApiModel right)
        {
            return !left.Equals(right);
        }
    }
}