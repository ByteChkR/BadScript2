using System.Text;

using BadScript2.Interop.Generator.Interop;
using BadScript2.Interop.Generator.Model;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
namespace BadScript2.Interop.Generator;

[Generator]
public class BadInteropGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(BadInteropStaticCode.RegisterAttributeSource);

        IncrementalValuesProvider<ApiModel> pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            BadInteropStaticCode.INTEROP_API_ATTRIBUTE,
            static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
            static (context, _) =>
            {
                INamedTypeSymbol api = (INamedTypeSymbol)context.TargetSymbol;
                BadInteropApiModelBuilder builder = new BadInteropApiModelBuilder();

                return builder.GenerateModel(api);
            }
        );

        context.RegisterSourceOutput(
            pipeline,
            static (context, model) =>
            {
                bool isError = false;
                if (model.Diagnostics.Length != 0)
                {
                    foreach (Diagnostic diagnostic in model.Diagnostics)
                    {
                        context.ReportDiagnostic(diagnostic);
                        isError |= diagnostic.Severity == DiagnosticSeverity.Error;
                    }
                }

                BadInteropApiSourceGenerator gen = new BadInteropApiSourceGenerator(context);
                SourceText sourceText = SourceText.From(
                    gen.GenerateModelSource(context, model, isError),
                    Encoding.UTF8
                );


                context.AddSource($"{model.ClassName}.g.cs", sourceText);
            }
        );
    }
}