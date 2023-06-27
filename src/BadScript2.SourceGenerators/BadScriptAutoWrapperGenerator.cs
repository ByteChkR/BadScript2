using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BadScript2.SourceGenerators;

[Generator]
public class BadScriptAutoWrapperGenerator : ISourceGenerator
{
    private const string s_PropertyAttributeName = "BadScript2.SourceGenerators.BadPropertyAttribute";
    private const string s_FunctionAttributeName = "BadScript2.SourceGenerators.BadFunctionAttribute";
    private const string s_ObjectAttributeName = "BadScript2.SourceGenerators.BadAutoWrapObjectAttribute";
    private const string s_AutoWrapInterfaceName = "BadScript2.SourceGenerators.IBadAutoWrappedObjectValue";


    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
        {
            return;
        }

        // group the fields by class, and generate the source
        foreach (IGrouping<INamedTypeSymbol, ISymbol> group in receiver.Members.GroupBy<ISymbol, INamedTypeSymbol>(f => f.ContainingType, SymbolEqualityComparer.Default))
        {
            string classSource = ProcessClass(group.Key, group.ToList(), GetAutoWrappedObjectFor(group.Key), receiver);
            context.AddSource($"{group.Key.Name}_BadScriptWrapper.g.cs", SourceText.From(classSource, Encoding.UTF8));
        }
    }

    public static string GetAutoWrappedObjectFor(INamedTypeSymbol type)
    {
        return $"BadScript2.SourceGenerators.BadAutoWrappedObject<{type.ToDisplayString()}>";
    }

    private string ProcessClass(
        INamedTypeSymbol classSymbol,
        List<ISymbol> members,
        string autoWrapperObj,
        SyntaxReceiver receiver)
    {
        if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            return null; //TODO: issue a diagnostic that it must be top level
        }

        string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

        // begin building the generated source
        StringBuilder source = new StringBuilder(
            $@"
namespace {namespaceName}
{{
    public partial class {classSymbol.Name} : {s_AutoWrapInterfaceName}
    {{
        BadScript2.Runtime.Objects.BadObject {s_AutoWrapInterfaceName}.CreateWrapper()
        {{
            return new {classSymbol.Name}_Wrapper(this);
        }}
        public static implicit operator BadScript2.Runtime.Objects.BadObject({classSymbol.Name} obj)
        {{
            return new {classSymbol.Name}_Wrapper(this);
        }}
    }}
    
    public class {classSymbol.Name}_Wrapper : {autoWrapperObj}
    {{
            public {classSymbol.Name}_Wrapper({classSymbol.Name} value) : base(value){{}}
            
            public override string ToSafeString(List<BadScript2.Runtime.Objects.BadObject> done)
	        {{
                return Value.ToString();
            }}

            public override BadScript2.Runtime.Objects.BadObjectReference GetProperty(BadScript2.Runtime.Objects.BadObject propName, BadScript2.Runtime.BadScope caller = null)
            {{
                if(propName is BadScript2.Runtime.Objects.Native.IBadString name)
                {{
"
        );


        // create properties for each field 
        foreach (ISymbol symbol in members)
        {
            string propertyRef;
            if (symbol is IPropertySymbol property)
            {
                string getter = $@"{{
        {property.Type.ToDisplayString()} ret = Value.{property.Name};
        if(ret is {s_AutoWrapInterfaceName} autoWrapValue)
        {{
            return autoWrapValue.CreateWrapper();
        }}
        else
        {{
            return BadScript2.Runtime.Objects.BadObject.Wrap(ret);
        }}
}}";
                if (property.IsReadOnly)
                {
                    propertyRef = $"BadScript2.Runtime.Objects.BadObjectReference.Make(\"{classSymbol.Name}.{property.Name}\", () => {getter}, null)";
                }
                else
                {
                    propertyRef =
                        $@"BadScript2.Runtime.Objects.BadObjectReference.Make(""{classSymbol.Name}.{property.Name}"", () => {getter}, (o, _) => Value.{property.Name} = BadScript2.Runtime.Interop.BadInteropHelper.Unwrap<{property.Type.ToDisplayString()}>(o))";
                }
            }
            else if (symbol is IMethodSymbol method)
            {
                IParameterSymbol[] parameters = method.Parameters.ToArray();
                ITypeSymbol[] parameterTypes = parameters.Select(x => x.Type).ToArray();
                string action;
                if (method.ReturnsVoid)
                {
                    action = GetInvocationAction(method.Name, parameterTypes);
                }
                else
                {
                    action = GetInvocationFunc(method.Name, parameterTypes, method.ReturnType, receiver);
                }

                propertyRef =
                    $@"BadScript2.Runtime.Objects.BadObjectReference.Make(""{classSymbol.Name}.{method.Name}"", () => new {GetInteropFunctionType(parameterTypes)}(""{method.Name}"", {action}{GetInteropFunctionParameterInfos(parameters)}))";
            }
            else
            {
                throw new InvalidOperationException("Only properties and methods are supported");
            }

            string propertyIf = $@"
                    if (name.Value == ""{symbol.Name}"")
                    {{
                        return {propertyRef};
                    }}
";
            source.AppendLine(propertyIf);
        }

        source.Append(
            @"
            }
            return base.GetProperty(propName, caller);
        }
    } 
}"
        );

        return source.ToString();
    }


    private static string GetInteropFunctionType(ITypeSymbol[] parameterTypes)
    {
        if (parameterTypes.Length == 0)
        {
            return "BadScript2.Runtime.Interop.Functions.BadDynamicInteropFunction";
        }

        return $"BadScript2.Runtime.Interop.Functions.BadDynamicInteropFunction<{string.Join(", ", parameterTypes.Select(x => x.ToDisplayString()))}>";
    }

    private static string GetInteropFunctionParameterInfos(IParameterSymbol[] parameters)
    {
        if (parameters.Length == 0)
        {
            return string.Empty;
        }

        return ", " + string.Join(", ", parameters.Select(GetInteropFunctionParameterInfo));
    }

    private static string GetInteropFunctionParameterInfo(IParameterSymbol parameter)
    {
        return $"new BadScript2.Runtime.Objects.Functions.BadFunctionParameter(\"{parameter.Name}\", false, false, false)";
    }


    private static string GetInvocationFunc(string funcName, ITypeSymbol[] parameterTypes, ITypeSymbol returnType, SyntaxReceiver receiver)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("(BadScript2.Runtime.BadExecutionContext _");
        for (int i = 0; i < parameterTypes.Length; i++)
        {
            sb.Append($", {parameterTypes[i].ToDisplayString()} p{i}");
        }

        sb.AppendLine(") => {");

        sb.Append($"{returnType.ToDisplayString()} ret = Value.{funcName}(");

        for (int i = 0; i < parameterTypes.Length; i++)
        {
            sb.Append($"p{i}");
            if (i < parameterTypes.Length - 1)
            {
                sb.Append(", ");
            }
        }

        sb.AppendLine(");");

        sb.Append(
            $@"if(ret is {s_AutoWrapInterfaceName} autoWrapValue)
        {{
            return autoWrapValue.CreateWrapper();
        }}
        else
        {{
            return BadScript2.Runtime.Objects.BadObject.Wrap(ret);
        }}
"
        );

        sb.Append("}");

        return sb.ToString();
    }

    private static string GetInvocationAction(string funcName, ITypeSymbol[] parameterTypes)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("(BadScript2.Runtime.BadExecutionContext _");
        for (int i = 0; i < parameterTypes.Length; i++)
        {
            sb.Append($", {parameterTypes[i].ToDisplayString()} p{i}");
        }

        sb.Append(") => ");
        sb.Append("Value." + funcName);
        sb.Append("(");
        for (int i = 0; i < parameterTypes.Length; i++)
        {
            sb.Append($"p{i}");
            if (i < parameterTypes.Length - 1)
            {
                sb.Append(", ");
            }
        }

        sb.Append(")");

        return sb.ToString();
    }

    private class SyntaxReceiver : ISyntaxContextReceiver
    {
        public readonly List<ISymbol> Members = new List<ISymbol>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node.Parent is not ClassDeclarationSyntax type)
            {
                return;
            }
            ISymbol typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);
            if (!typeSymbol.GetAttributes().Any(x => x.AttributeClass.ToDisplayString() == s_ObjectAttributeName))
            {
                return;
            }

            // any field with at least one attribute is a candidate for property generation

            if (context.Node is PropertyDeclarationSyntax propertyDeclarationSyntax && propertyDeclarationSyntax.AttributeLists.Count > 0)
            {
                ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax);
                if (symbol.GetAttributes().Any(x => x.AttributeClass.ToDisplayString() == s_PropertyAttributeName))
                {
                    Members.Add(symbol);
                }
            }
            else if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax && methodDeclarationSyntax.AttributeLists.Count > 0)
            {
                ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
                if (symbol.GetAttributes().Any(x => x.AttributeClass.ToDisplayString() == s_FunctionAttributeName))
                {
                    Members.Add(symbol);
                }
            }
        }
    }
}