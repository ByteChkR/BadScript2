﻿using BadHtml;

using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

///<summary>
///	Contains HTML Extensions and APIs for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.Html;

/// <summary>
///     Implements the "BadHtml" API
/// </summary>
[BadInteropApi("BadHtml")]
internal partial class BadHtmlApi
{
    /// <summary>
    /// Runs a BadHtml Template
    /// </summary>
    /// <param name="context">The Current Execution Context</param>
    /// <param name="file">The Template File</param>
    /// <param name="model">The Model</param>
    /// <param name="skipEmptyTextNodes">If True, empty text nodes are omitted from the output html</param>
    /// <returns>>The string result of the html transformation</returns>
    [BadMethod(description: "Runs a BadHtml Template")]
    [return: BadReturn("The string result of the html transformation")]
    private string Run(BadExecutionContext context,
                       [BadParameter(description: "The Template File")]
                       string file,
                       [BadParameter(description: "The Model")]
                       BadObject? model = null,
                       [BadParameter(description: "If True, empty text nodes are omitted from the output html")]
                       bool skipEmptyTextNodes = false)
    {
        BadHtmlTemplate template = BadHtmlTemplate.Create(file);

        BadHtmlTemplateOptions options = new BadHtmlTemplateOptions{Runtime = context.Scope.GetSingleton<BadRuntime>()};
        options.SkipEmptyTextNodes = skipEmptyTextNodes;

        return template.Run(model, options, context.Scope);
    }
}