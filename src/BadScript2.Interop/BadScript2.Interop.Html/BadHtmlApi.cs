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

/// <summary>
///     Implements the "BadHtml" API
/// </summary>
public class BadHtmlApi : BadInteropApi
{
    /// <summary>
    ///     Creates a new API Instance
    /// </summary>
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
                new BadFunctionParameter(
                    "skipEmptyTextNodes",
                    true,
                    true,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("bool")
                )
            )
        );
    }

    /// <summary>
    ///     Runs a BadHtml Template
    /// </summary>
    /// <param name="context">The Execution Context</param>
    /// <param name="args">The arguments passed from the caller</param>
    /// <returns>The string result of the html transformation</returns>
    /// <exception cref="BadRuntimeException">
    ///     Gets thrown if the file argument is not a IBadString or the skipEmptyTextNodes
    ///     argument is not a IBadBoolean
    /// </exception>
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