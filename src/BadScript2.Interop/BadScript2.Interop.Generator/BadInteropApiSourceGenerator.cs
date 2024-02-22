using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BadScript2.Interop.Generator;

public class BadInteropApiSourceGenerator
{
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

    public static string GenerateModelSource(ApiModel apiModel)
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
}