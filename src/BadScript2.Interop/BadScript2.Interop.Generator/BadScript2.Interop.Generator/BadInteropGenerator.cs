using System.Text;

using BadScript2.Interop.Generator.Interop;
using BadScript2.Interop.Generator.Model;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BadScript2.Interop.Generator;

/// <summary>
/// Generator that generates BadScript2.Interop code
/// </summary>
[Generator]
public class BadInteropGenerator : IIncrementalGenerator
{
#region IIncrementalGenerator Members

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(BadInteropStaticCode.RegisterAttributeSource);

        IncrementalValuesProvider<ApiModel> apiPipeline =
            context.SyntaxProvider.ForAttributeWithMetadataName(BadInteropStaticCode.INTEROP_API_ATTRIBUTE,
                static (syntaxNode, _) =>
                    syntaxNode is ClassDeclarationSyntax,
                static (context, _) =>
                {
                    INamedTypeSymbol api =
                        (INamedTypeSymbol)context.TargetSymbol;

                    BadInteropApiModelBuilder builder =
                        new BadInteropApiModelBuilder();

                    return builder.GenerateModel(api);
                }
            );
        
        IncrementalValuesProvider<ObjectModel> objectPipeline =
            context.SyntaxProvider.ForAttributeWithMetadataName(BadInteropStaticCode.INTEROP_OBJECT_ATTRIBUTE,
                static (syntaxNode, _) =>
                    syntaxNode is ClassDeclarationSyntax,
                static (context, _) =>
                {
                    INamedTypeSymbol api =
                        (INamedTypeSymbol)context.TargetSymbol;

                    BadInteropObjectModelBuilder builder =
                        new BadInteropObjectModelBuilder();

                    return builder.GenerateModel(api);
                }
            );

        context.RegisterSourceOutput(apiPipeline,
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

                SourceText sourceText =
                    SourceText.From(gen.GenerateModelSource(context, model, isError),
                        Encoding.UTF8
                    );

                context.AddSource($"{model.ClassName}.Api.g.cs", sourceText);
            }
        );
        
        context.RegisterSourceOutput(objectPipeline,
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

                BadInteropObjectSourceGenerator gen = new BadInteropObjectSourceGenerator(context);

                SourceText sourceText =
                    SourceText.From(gen.GenerateModelSource(context, model, isError),
                        Encoding.UTF8
                    );

                context.AddSource($"{model.ClassName}.Object.g.cs", sourceText);
            }
        );
    }

#endregion
}