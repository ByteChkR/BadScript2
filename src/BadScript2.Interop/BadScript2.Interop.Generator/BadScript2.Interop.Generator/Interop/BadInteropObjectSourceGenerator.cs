using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BadScript2.Interop.Generator.Model;
using Microsoft.CodeAnalysis;

namespace BadScript2.Interop.Generator.Interop;

public class BadInteropObjectSourceGenerator
{
    private readonly SourceProductionContext m_Context;

    public BadInteropObjectSourceGenerator(SourceProductionContext context)
    {
        m_Context = context;
    }

    private string GenerateConstructor(ObjectModel model)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"new {model.ClassName}Wrapper(new {model.ClassName}(");
        var method = model.Constructor;
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
        sb.Append("))");
        
        return sb.ToString();
    }
    private string GenerateInvocation(MethodModel method, string className)
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

        sb.Append($"(({className})Value).{method.MethodName}(");
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

    private string GenerateParameterSource(ParameterModel model)
    {
        return
            $"new BadFunctionParameter(\"{model.Name}\", {model.HasDefaultValue.ToString().ToLower()}, {(!model.IsNullable).ToString().ToLower()}, {model.IsRestArgs.ToString().ToLower()}, null, BadNativeClassBuilder.GetNative(\"{model.Type}\"))";
    }

    private void GeneratePropertySource(IndentedTextWriter sb, PropertyModel model, string className)
    {
        if(model.IsReadOnly)
        {
            sb.WriteLine($"Properties[\"{model.ApiParameterName}\"] = BadObjectReference.Make(\"{model.ApiParameterName}\", p => Wrap((({className})Value).{model.ParameterName}));");
        }
        else
        {
            sb.WriteLine($"Properties[\"{model.ApiParameterName}\"] = ");
            sb.Indent++;
            sb.WriteLine($"BadObjectReference.Make(\"{model.ApiParameterName}\",");
            sb.Indent++;
            sb.WriteLine($"p => Wrap((({className})Value).{model.ParameterName}),");
            sb.WriteLine($"(v, p, i) => (({className})Value).{model.ParameterName} = v.Unwrap<{model.ParameterType}>()");
            sb.Indent--;
            sb.WriteLine(");");
            sb.Indent--;
        }
    }
    private void GenerateMethodSource(IndentedTextWriter sb, MethodModel method, string className)
    {
        sb.WriteLine($"Properties[\"{method.ApiMethodName}\"] = ");
        sb.Indent++;
        sb.WriteLine($"BadObjectReference.Make(\"{method.ApiMethodName}\",");
        sb.Indent++;
        sb.WriteLine($"p => new BadInteropFunction(");
        sb.Indent++;
        sb.WriteLine($"\"{method.ApiMethodName}\",");
        sb.WriteLine($"(ctx, args) => {GenerateInvocation(method, className)},");
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
        sb.Indent--;
    }

    public string GenerateModelSource(SourceProductionContext context, ObjectModel apiModel, bool isError)
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
        tw.WriteLine($"public partial class {apiModel.ClassName}Wrapper : {(string.IsNullOrEmpty(apiModel.BaseClassName) ? $"BadScript2.Runtime.Objects.Native.BadNative<{apiModel.ClassName}>": apiModel.BaseClassName)}");
        tw.WriteLine("{");
        tw.Indent++;

        tw.WriteLine("private static BadClassPrototype? s_Prototype;");
        tw.WriteLine("private static BadClassPrototype CreatePrototype()");
        tw.WriteLine("{");
        tw.Indent++;
        tw.WriteLine("T? GetParameter<T>(BadObject[] args, int i, T? defaultValue = default(T)) => args.Length>i?args[i].Unwrap<T>():defaultValue;");
        tw.WriteLine($"return new BadNativeClassPrototype<{apiModel.ClassName}Wrapper>(\"{apiModel.ObjectName}\", (ctx, args) => {GenerateConstructor(apiModel)}, {(string.IsNullOrEmpty(apiModel.BaseClassName) ? "null" : $"{apiModel.BaseClassName}.Prototype")});");
        tw.Indent--;
        tw.WriteLine("}");
        tw.WriteLine("public static BadClassPrototype Prototype => s_Prototype ??= CreatePrototype();");

        if(string.IsNullOrEmpty(apiModel.BaseClassName))
        {
            tw.WriteLine("protected readonly Dictionary<string, BadObjectReference> Properties = new Dictionary<string, BadObjectReference>();");
            tw.WriteLine();
        }
        tw.WriteLine($"public {apiModel.ClassName}Wrapper({apiModel.ClassName} value) : base(value)");
        tw.WriteLine("{");
        tw.Indent++;
        if (!isError)
        {
            tw.WriteLine("T? GetParameter<T>(BadObject[] args, int i, T? defaultValue = default(T)) => args.Length>i?args[i].Unwrap<T>():defaultValue;"
            );

            foreach (PropertyModel method in apiModel.Properties)
            {
                GeneratePropertySource(tw, method, apiModel.ClassName);
            }
            foreach (MethodModel method in apiModel.Methods)
            {
                GenerateMethodSource(tw, method, apiModel.ClassName);
            }
        }

        tw.Indent--;
        tw.WriteLine("}");
        
        tw.WriteLine("public override bool HasProperty(string propName, BadScript2.Runtime.BadScope? caller = null)");
        tw.WriteLine("{");
        tw.Indent++;
        tw.WriteLine("return Properties.ContainsKey(propName) || base.HasProperty(propName, caller);");
        tw.Indent--;
        tw.WriteLine("}");
        
        tw.WriteLine("public override BadObjectReference GetProperty(string propName, BadScript2.Runtime.BadScope? caller = null)");
        tw.WriteLine("{");
        tw.Indent++;
        tw.WriteLine("if(Properties.TryGetValue(propName, out BadObjectReference? ret))");
        tw.WriteLine("{");
        tw.Indent++;
        tw.WriteLine("return ret;");
        tw.Indent--;
        tw.WriteLine("}");
        tw.WriteLine("return base.GetProperty(propName, caller);");
        tw.Indent--;
        tw.WriteLine("}");
        

        tw.WriteLine("public override BadClassPrototype GetPrototype() => Prototype;");
        tw.WriteLine($"public static implicit operator {apiModel.ClassName}Wrapper({apiModel.ClassName} obj) => new {apiModel.ClassName}Wrapper(obj);");
        tw.WriteLine($"public static implicit operator {apiModel.ClassName}({apiModel.ClassName}Wrapper obj) => ({apiModel.ClassName})obj.Value;");
        
        tw.Indent--;
        tw.WriteLine("}");

        tw.Flush();

        string str = tw.InnerWriter.ToString();

        return str;
    }
}