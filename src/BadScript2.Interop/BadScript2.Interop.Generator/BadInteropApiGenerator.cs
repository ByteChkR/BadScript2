#nullable enable

using System.Text;

using BadScript2.Interop.Generator.Model;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BadScript2.Interop.Generator;

[Generator]
public class BadInteropApiGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(BadInteropApiAttributes.RegisterAttributeSource);

        IncrementalValuesProvider<ApiModel> pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            BadInteropApiAttributes.INTEROP_API_ATTRIBUTE,
            static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
            static (context, _) =>
            {
                INamedTypeSymbol api = (INamedTypeSymbol)context.TargetSymbol;

                return BadInteropApiModelBuilder.GenerateModel(api);
            }
        );

        context.RegisterSourceOutput(
            pipeline,
            static (context, model) =>
            {
                SourceText sourceText = SourceText.From(
                    BadInteropApiSourceGenerator.GenerateModelSource(model),
                    Encoding.UTF8
                );

                context.AddSource($"{model.ClassName}.g.cs", sourceText);
            }
        );
    }

    
}