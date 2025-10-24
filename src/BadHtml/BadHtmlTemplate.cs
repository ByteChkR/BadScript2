using System.IO;

using BadScript2;
using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

using HtmlAgilityPack;

namespace BadHtml;

/// <summary>
///     Implements a Html Template
/// </summary>
public class BadHtmlTemplate
{

	/// <summary>
	///     The Source code of the Template(gets loaded on first template run)
	/// </summary>
	private readonly string m_Source;

	/// <summary>
	///     Constructs a new Template
	/// </summary>
	/// <param name="filePath">File path of the template</param>
	/// <param name="fileSystem">The Filesystem that is used to Load the Template</param>
	private BadHtmlTemplate(string filePath, string source)
    {
        FilePath = filePath;
        m_Source = source;
    }


	/// <summary>
	///     The Filepath of the Template
	/// </summary>
	public string FilePath { get; }

	/// <summary>
	///     Runs the Template with the specified arguments
	/// </summary>
	/// <param name="model">The Model used within the template</param>
	/// <param name="options">The Template Options</param>
	/// <param name="caller">Optional Caller (if executed from within badscript, is used to get a full stacktrace on errors)</param>
	/// <returns>The Html Document that was generated</returns>
	public HtmlDocument RunTemplate(object? model = null,
	                                BadHtmlTemplateOptions? options = null,
	                                BadScope? caller = null)
    {
        options ??= new BadHtmlTemplateOptions();

        // ReSharper disable once UseObjectOrCollectionInitializer
        HtmlDocument input = new HtmlDocument();
        input.OptionUseIdAttribute = true;
        input.LoadHtml(m_Source);

        // ReSharper disable once UseObjectOrCollectionInitializer
        HtmlDocument output = new HtmlDocument();
        output.OptionUseIdAttribute = true;
        output.LoadHtml("");

        BadExecutionContext executionContext =
	        (options.Runtime ?? new BadRuntime()).CreateContext(Path.GetDirectoryName(FilePath
	                                                            ) ??
	                                                            "/"
	        );
        executionContext.Scope.SetCaller(caller);

        if (model != null)
        {
            BadObject mod = model as BadObject ?? BadObject.Wrap(model);

            executionContext.Scope.DefineVariable("Model",
                                                  mod,
                                                  executionContext.Scope,
                                                  new BadPropertyInfo(BadAnyPrototype.Instance, true)
                                                 );
        }

        foreach (HtmlNode node in input.DocumentNode.ChildNodes)
        {
            BadHtmlContext ctx =
                new BadHtmlContext(node, output.DocumentNode, executionContext, FilePath, m_Source, options);
            BadHtmlNodeTransformer.Transform(ctx);
        }

        return output;
    }

	/// <summary>
	///     Runs the Template with the specified arguments
	/// </summary>
	/// <param name="model">The Model used within the template</param>
	/// <param name="options">The Template Options</param>
	/// <param name="caller">Optional Caller (if executed from within badscript, is used to get a full stacktrace on errors)</param>
	/// <returns>The Html Source of the Generated Document</returns>
	public string Run(object? model = null, BadHtmlTemplateOptions? options = null, BadScope? caller = null)
    {
        return RunTemplate(model, options, caller)
               .DocumentNode.OuterHtml;
    }

	/// <summary>
	///     Creates a new Template
	/// </summary>
	/// <param name="file">Template File Path</param>
	/// <param name="source">The Source that the Template is loaded from</param>
	/// <returns>A Html Template Instance</returns>
	public static BadHtmlTemplate Create(string file, string source)
    {
        return new BadHtmlTemplate(file, source);
    }
}