using BadHtml;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Html;

public class BadHtmlApi : BadInteropApi
{
    public BadHtmlApi() : base("BadHtml") { }

    protected override void LoadApi(BadTable target)
    {
        target.SetProperty(
            "Run",
            new BadInteropFunction(
                "Run",
                RunTemplate,
                false,
                new BadFunctionParameter("file", false, true, false, null, BadNativeClassBuilder.GetNative("string")),
                new BadFunctionParameter("model", true, false, false),
                new BadFunctionParameter("skipEmptyTextNodes", true, true, false, null, BadNativeClassBuilder.GetNative("bool"))
            )
        );
    }

    private BadObject RunTemplate(BadExecutionContext context, BadObject[] args)
    {
        BadObject fileObj = args[0];
        BadObject model = BadObject.Null;
        if (args.Length >= 2)
        {
            model = args[1];
        }

        if (fileObj is not IBadString file)
        {
            throw BadRuntimeException.Create(context.Scope, "Invalid file path: " + fileObj);
        }

        BadHtmlTemplateOptions options = new BadHtmlTemplateOptions();
        if (args.Length == 3)
        {
            if (args[2] is not IBadBoolean skipEmptyText)
            {
                throw BadRuntimeException.Create(context.Scope, "Invalid skipEmptyTextNodes: " + args[2]);
            }

            options.SkipEmptyTextNodes = skipEmptyText.Value;
        }

        BadHtmlTemplate template = BadHtmlTemplate.Create(file.Value);

        return template.Run(model, options, context.Scope);
    }
}