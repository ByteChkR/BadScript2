using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using BadScript2.Interop.Generator.Model;

using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator.Interop;

/// <summary>
/// The BadInteropApiSourceGenerator generates the source code for the BadScript2.Interop API.
/// </summary>
public class BadInteropApiSourceGenerator
{
    /// <summary>
    /// The context of the generator.
    /// </summary>
    private readonly SourceProductionContext m_Context;

    /// <summary>
    /// Constructs a new BadInteropApiSourceGenerator instance.
    /// </summary>
    /// <param name="context"></param>
    public BadInteropApiSourceGenerator(SourceProductionContext context)
    {
        m_Context = context;
    }

    /// <summary>
    /// Generates an Invocation for the given MethodModel.
    /// </summary>
    /// <param name="method">The MethodModel to generate the invocation for.</param>
    /// <returns>The generated invocation as a string.</returns>
    private string GenerateInvocation(MethodModel method)
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
        bool hasCtx = method.Parameters.Any(x => x.IsContext);
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
                int index = i - (hasCtx ? 1 : 0);
                string suppressNullable = parameter.IsNullable ? "" : "!";

                if (parameter.IsRestArgs)
                {
                    if (index == 0)
                    {
                        args.Add($"new BadArray(args.ToList()).Unwrap<{parameter.CsharpType}>()");
                    }
                    else
                    {
                        args.Add($"new BadArray(args.Skip({index}).ToList()).Unwrap<{parameter.CsharpType}>()");
                    }
                }
                else if (parameter.HasDefaultValue)
                {
                    args.Add($"GetParameter<{parameter.CsharpType}>(args, {index}, {parameter.DefaultValue ?? $"default({parameter.CsharpType})"}){suppressNullable}"
                            );
                }
                else
                {
                    args.Add($"GetParameter<{parameter.CsharpType}>(args, {index}){suppressNullable}");
                }
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
            sb.Append($", {method.AllowNativeReturn.ToString().ToLower()})");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates a parameter source for the given ParameterModel.
    /// </summary>
    /// <param name="model">The ParameterModel to generate the source for.</param>
    /// <returns>The generated parameter source as a string.</returns>
    private string GenerateParameterSource(ParameterModel model)
    {
        return
            $"new BadFunctionParameter(\"{model.Name}\", {model.HasDefaultValue.ToString().ToLower()}, {(!model.IsNullable).ToString().ToLower()}, {model.IsRestArgs.ToString().ToLower()}, null, BadNativeClassBuilder.GetNative(\"{model.Type}\"))";
    }

    /// <summary>
    /// Generates the source code for a method.
    /// </summary>
    /// <param name="sb">The IndentedTextWriter to write the source code to.</param>
    /// <param name="method">The MethodModel to generate the source code for.</param>
    private void GenerateMethodSource(IndentedTextWriter sb, MethodModel method)
    {
        sb.WriteLine("target.SetProperty(");
        sb.Indent++;
        sb.WriteLine($"\"{method.ApiMethodName}\",");
        sb.WriteLine("new BadInteropFunction(");
        sb.Indent++;
        sb.WriteLine($"\"{method.ApiMethodName}\",");
        sb.WriteLine($"(ctx, args) => {GenerateInvocation(method)},");
        sb.WriteLine("false,");
        sb.Write($"BadNativeClassBuilder.GetNative(\"{method.ReturnType}\")");

        if (method.Parameters.Any(x => !x.IsContext))
        {
            sb.WriteLine(",");
        }
        else
        {
            sb.WriteLine();
        }

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

            if (parameter.HasDefaultValue)
            {
                sb.WriteLine($"{{\"{parameter.Name}\", new BadParameterMetaData(\"{parameter.Type}\", \"{parameter.Description}\\nDefault Value: {parameter.DefaultValue!.Replace("\"", "\\\"")}\")}},"
                            );
            }
            else
            {
                sb.WriteLine($"{{\"{parameter.Name}\", new BadParameterMetaData(\"{parameter.Type}\", \"{parameter.Description}\")}},"
                            );
            }
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

    /// <summary>
    /// Generates the source code for the given ApiModel.
    /// </summary>
    /// <param name="context">The SourceProductionContext to write the source code to.</param>
    /// <param name="apiModel">The ApiModel to generate the source code for.</param>
    /// <param name="isError">Whether the API is an error API.</param>
    /// <returns>The generated source code as a string.</returns>
    public string GenerateModelSource(SourceProductionContext context, ApiModel apiModel, bool isError)
    {
        IndentedTextWriter tw = new IndentedTextWriter(new StringWriter());
        tw.WriteLine("#nullable enable");
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
        tw.WriteLine($"partial class {apiModel.ClassName} : BadScript2.Interop.BadAutoGeneratedInteropApi");
        tw.WriteLine("{");
        tw.Indent++;

        tw.WriteLine($"{(apiModel.ConstructorPrivate ? "private" : "public")} {apiModel.ClassName}() : base(\"{apiModel.ApiName}\") {{ }}"
                    );
        tw.WriteLine();
        tw.WriteLine("protected override void LoadApi(BadTable target)");
        tw.WriteLine("{");
        tw.Indent++;
        tw.WriteLine("AdditionalData(target);");

        if (!isError && apiModel.Methods.Length != 0)
        {
            tw.WriteLine("T? GetParameter<T>(BadObject[] args, int i, T? defaultValue = default(T)) => args.Length>i?args[i].Unwrap<T>():defaultValue;"
                        );

            foreach (MethodModel method in apiModel.Methods)
            {
                GenerateMethodSource(tw, method);
            }
        }

        tw.Indent--;
        tw.WriteLine("}");
        tw.Indent--;
        tw.WriteLine("}");

        tw.Flush();

        string str = tw.InnerWriter.ToString();

        return str;
    }
}