using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using BadScript2.Common.Logging;
using BadScript2.Common.Logging.Writer;
using BadScript2.Debugger.Scriptable;
using BadScript2.Debugging;
using BadScript2.Interop.Common;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.IO;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;
using BadScript2.Settings;

using HtmlAgilityPack;

namespace BadHtml;

/// <summary>
///     Represents a Html Template
/// </summary>
public class BadHtmlTemplate
{
    private static string? s_ExtensionDirectory;

    /// <summary>
    ///     Static flag that indicates if the BadScript Runtime has been initialized
    /// </summary>
    private static bool s_Initialized;

    /// <summary>
    ///     The Console Log Writer for the BadScript Runtime
    /// </summary>
    private static BadLogWriter? s_ConsoleWriter;

    /// <summary>
    ///     The Backing Field for the source code that was loaded from the template file
    /// </summary>
    private string? m_Source;


    /// <summary>
    ///     Constructs a new BadHtmlTemplate
    /// </summary>
    /// <param name="filePath">The Filename of the Template</param>
    public BadHtmlTemplate(string filePath)
    {
        FilePath = Path.GetFullPath(filePath);
    }

    /// <summary>
    ///     The File Path of the Template
    /// </summary>
    public string FilePath { get; }

    public static string DebuggerPath { get; set; } = "Debugger.bs";

    private static string ExtensionDirectory
    {
        get
        {
            if (s_ExtensionDirectory != null)
            {
                return s_ExtensionDirectory;
            }

            string? s = BadSettingsProvider.RootSettings.FindProperty<string>("Subsystems.Html.ExtensionDirectory");
            if (s == null)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BadHtml", "Extensions");
            }

            BadFileSystem.Instance.CreateDirectory(s);

            return s;
        }
    }

    /// <summary>
    ///     The Source Code of the Template
    /// </summary>
    public string Source => m_Source ??= BadFileSystem.ReadAllText(FilePath);

    public static void SetExtensionDirectory(string dir)
    {
        s_ExtensionDirectory = dir;
    }

    public static void SkipInitialization()
    {
        s_Initialized = true;
    }

    /// <summary>
    ///     Enables Logging for the BadScript Runtime
    /// </summary>
    /// <param name="enable">Enable Flag</param>
    public static void EnableLogging(bool enable)
    {
        Initialize();
        if (enable)
        {
            s_ConsoleWriter!.Register();
        }
        else
        {
            s_ConsoleWriter!.Unregister();
        }
    }

    /// <summary>
    ///     Initializes the BadScript Runtime
    /// </summary>
    private static void Initialize()
    {
        if (s_Initialized)
        {
            return;
        }

        s_Initialized = true;
        BadSettingsProvider.SetRootSettings(new BadSettings());
        s_ConsoleWriter = new BadConsoleLogWriter();
        s_ConsoleWriter.Register();
        BadLogWriterSettings.Instance.Mask = BadLogMask.GetMask("BadHtml", "BadReflection");
        BadFileSystem.SetFileSystem(new BadSystemFileSystem());
        BadCommonInterop.AddExtensions();
        BadInteropExtension.AddExtension<BadLinqExtensions>();
        BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
        BadExecutionContextOptions.Default.AddApis(BadCommonInterop.Apis);
        BadExecutionContextOptions.Default.AddApi(new BadJsonApi());
        BadExecutionContextOptions.Default.AddApi(new BadIOApi());
    }

    /// <summary>
    ///     Reloads the Source of the Template File Manually
    /// </summary>
    public void Reload()
    {
        m_Source = BadFileSystem.ReadAllText(FilePath);
    }

    /// <summary>
    ///     Implements the BadHtml.FromFile API
    /// </summary>
    /// <param name="path">Template File Path</param>
    /// <param name="args">The Model Object</param>
    /// <returns>The String Result of the Template</returns>
    private static BadObject FromFile(string path, BadObject args)
    {
        BadHtmlTemplate template = new BadHtmlTemplate(path);

        return template.Run(args);
    }

    /// <summary>
    ///     Builds the BadHtml Api
    /// </summary>
    /// <returns>The API Table</returns>
    private BadTable BuildApi()
    {
        BadTable api = new BadTable();

        api.SetFunction<string, BadObject>("FromFile", FromFile);
        api.SetFunction<string>("Require", (ctx, s) => Require(ctx, api, s));
        api.SetProperty("TemplateFile", Path.GetFullPath(FilePath), new BadPropertyInfo(BadNativeClassBuilder.GetNative("string"), true));

        return api;
    }

    private BadObject Require(BadExecutionContext ctx, BadTable api, string ext)
    {
        string path = Path.Combine(ExtensionDirectory, ext + '.' + BadRuntimeSettings.Instance.FileExtension);
        IEnumerable<BadExpression> script = BadSourceParser.Create(path, BadFileSystem.ReadAllText(path)).Parse();
        BadExecutionContext scriptCtx = new BadExecutionContext(ctx.Scope.CreateChild("ExtensionLoadScope", null, null, BadScopeFlags.Returnable));
        api.SetProperty(ext, scriptCtx.Run(script) ?? BadObject.Null, new BadPropertyInfo(null, true));

        return BadObject.Null;
    }


    /// <summary>
    ///     Creates a Script Context for the Template
    /// </summary>
    /// <param name="model">The Model used in the template</param>
    /// <param name="document">The HTML Document that contains the Template Code</param>
    /// <param name="debug">If true, attaches a debugger</param>
    /// <returns>The Created Execution Context</returns>
    private BadExecutionContext CreateContext(object? model, HtmlDocument document, bool debug)
    {
        Initialize();
        BadExecutionContextOptions options = BadExecutionContextOptions.Default.Clone();
        if (debug)
        {
            BadDebugger.Attach(new BadScriptDebugger(options, DebuggerPath));
        }

        BadExecutionContext ctx = options.Build();
        ctx.Scope.DefineVariable("BadHtml", BuildApi());
        if (model is BadObject bo)
        {
            ctx.Scope.DefineVariable("Model", bo);
        }
        else if (model != null)
        {
            ctx.Scope.DefineVariable("Model", new BadReflectedObject(model));
        }

        ctx.Scope.DefineVariable("Document", new BadReflectedObject(document));

        return ctx;
    }

    /// <summary>
    ///     Returns true if the HtmlNodes Child Nodes can be processed.
    /// </summary>
    /// <param name="node">Node</param>
    /// <returns>True if child nodes can be processed</returns>
    private bool CanProcessChildren(HtmlNode node)
    {
        return node.Name != "script" && node.Name != "style" && !IsSpecialNode(node);
    }

    /// <summary>
    ///     Processes an inline script
    ///     Format: $"My Name is {Model.Name}"
    /// </summary>
    /// <param name="input">The Input String</param>
    /// <param name="ctx">The Execution Context</param>
    /// <returns>The Resulting Output</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the format string is invalid.</exception>
    private string ProcessInline(string input, BadExecutionContext ctx)
    {
        string src = $"$\"{input.Replace("\n", "").Replace("\r", "")}\"";
        BadSourceParser parser = BadSourceParser.Create(FilePath, src);
        BadExpression[] exprs = parser.Parse().ToArray();
        BadObject ret = ctx.Execute(exprs).Last();
        if (ret is not IBadString str)
        {
            throw new BadRuntimeException("Invalid Format String Return");
        }

        return str.Value;
    }

    /// <summary>
    ///     Processes an Inline Script
    /// </summary>
    /// <param name="node">Node with the Inline Script</param>
    /// <param name="ctx">The Execution Context</param>
    private void ProcessInlineScript(HtmlNode node, BadExecutionContext ctx)
    {
        if (string.IsNullOrWhiteSpace(node.InnerHtml) || node.Name == "#comment" || !node.InnerHtml.Contains('{'))
        {
            return;
        }

        //Remove the style tag and add it after the transformation
        string originalText = node.InnerHtml;
        List<string> styleTags = new List<string>();
        while (true)
        {
            int next = node.InnerHtml.IndexOf("<style", StringComparison.Ordinal);

            if (next == -1)
            {
                break;
            }

            int nextEnd = node.InnerHtml.IndexOf("</style>", StringComparison.Ordinal);
            styleTags.Add(node.InnerHtml.Substring(next, nextEnd - next + 8));
            node.InnerHtml = node.InnerHtml.Remove(next, nextEnd - next + 8);
        }

        if (!node.InnerHtml.Contains('{'))
        {
            node.InnerHtml = originalText;

            return;
        }

        StringBuilder r = new StringBuilder(ProcessInline(node.InnerHtml, ctx));
        foreach (string styleTag in styleTags)
        {
            r.Append(styleTag);
        }

        node.InnerHtml = r.ToString();
    }

    /// <summary>
    ///     Processes a Script Source File
    /// </summary>
    /// <param name="node">The Script Node containing the "src" attribute</param>
    /// <param name="ctx">The Execution Context</param>
    private void ProcessScriptSource(HtmlNode node, BadExecutionContext ctx)
    {
        string file = node.Attributes["src"].Value;
        BadSourceParser parser = BadSourceParser.Create(Path.GetFullPath(file), File.ReadAllText(file));
        ctx.Run(parser.Parse());
        node.ParentNode.RemoveChild(node);
    }

    /// <summary>
    ///     Processes a Script Block.
    /// </summary>
    /// <param name="node">The Script Node containing the source code</param>
    /// <param name="ctx">The Execution Context</param>
    private void ProcessScriptBlock(HtmlNode node, BadExecutionContext ctx)
    {
        string src = node.InnerText;
        BadSourceParser parser = BadSourceParser.Create(FilePath, src);
        ctx.Execute(parser.Parse()).Last();
        node.ParentNode.RemoveChild(node);
    }

    /// <summary>
    ///     Processes a Script Node (Script Block or Script Source)
    /// </summary>
    /// <param name="node">The Script Node</param>
    /// <param name="ctx">The Execution Context</param>
    private void ProcessScriptNode(HtmlNode node, BadExecutionContext ctx)
    {
        if (node.Attributes.Contains("src"))
        {
            ProcessScriptSource(node, ctx);
        }
        else
        {
            ProcessScriptBlock(node, ctx);
        }
    }

    private string ProcessChildrenIsolated(HtmlNode node, BadExecutionContext ctx)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(node.InnerHtml);
        ProcessNode(doc.DocumentNode, ctx);

        if (ctx.Scope.IsError) throw new BadRuntimeErrorException(ctx.Scope.Error);

        return doc.DocumentNode.InnerHtml;
    }

    private IEnumerable<HtmlNode> ProcessChildNodesIsolated(HtmlNode node, BadExecutionContext ctx)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(node.InnerHtml);
        ProcessNode(doc.DocumentNode, ctx);
        if (ctx.Scope.IsError) throw new BadRuntimeErrorException(ctx.Scope.Error);

        return doc.DocumentNode.ChildNodes;
    }


    private string InvokeFunction(HtmlNode node, BadExecutionContext ctx, BadFunctionParameter[] parameters, BadObject[] args)
    {
        if(args.Length != parameters.Length)
        {
            throw new BadRuntimeException("Invalid Argument Count");
        }
        BadExecutionContext funcCtx = new BadExecutionContext(ctx.Scope.CreateChild("bs:function", null, null));
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i].Initialize(funcCtx);
            funcCtx.Scope.DefineVariable(parameter.Name, args[i], funcCtx.Scope, new BadPropertyInfo(parameter.Type));
        }

        return ProcessChildrenIsolated(node, funcCtx);
    }


    private BadExpression? ParseParameterType(HtmlAttribute attribute)
    {
        if (string.IsNullOrEmpty(attribute.Value))
        {
            return null;
        }

        BadSourceParser parser = BadSourceParser.Create(FilePath, attribute.Value);

        return parser.Parse().FirstOrDefault();
    }

    private void ProcessFunction(HtmlNode node, BadExecutionContext ctx)
    {
        string name = node.Attributes["name"]?.Value ?? throw new BadRuntimeException("Invalid Function Name");
        BadFunctionParameter[] parameter = node.Attributes.Where(x => x.Name.StartsWith("param:"))
            .Select(
                x => new BadFunctionParameter(
                    x.Name.Remove(0, "param:".Length),
                    false,
                    false,
                    false,
                    ParseParameterType(x)
                )
            )
            .ToArray();
        BadInteropFunction func = new BadInteropFunction(
            name,
            (ctx, args) => InvokeFunction(node, ctx, parameter, args),
            parameter
        );
        ctx.Scope.DefineVariable(name, func, ctx.Scope, new BadPropertyInfo(func.GetPrototype(), true));
        node.ParentNode.RemoveChild(node);
    }

    private void ProcessForeach(HtmlNode node, BadExecutionContext ctx)
    {
        string? enumeratorStr = node.Attributes["on"]?.Value;
        string name = node.Attributes["as"]?.Value ?? "item";
        if (enumeratorStr == null)
        {
            throw new BadRuntimeException("Invalid Enumerator");
        }

        BadSourceParser enumeratorParser = BadSourceParser.Create(FilePath, enumeratorStr);
        BadExpression[] enumeratorExprs = enumeratorParser.Parse().ToArray();
        BadObject ret = ctx.Execute(enumeratorExprs).Last().Dereference();
        HtmlNode? parent = node.ParentNode;
        HtmlNode? prevSilbing = node.PreviousSibling;

        if (ret is IBadEnumerable enumerable)
        {
            foreach (BadObject o in enumerable)
            {
                BadExecutionContext loopCtx = new BadExecutionContext(ctx.Scope.CreateChild("bs:foreach", null, null));
                loopCtx.Scope.DefineVariable(name, o, loopCtx.Scope, new BadPropertyInfo(o.GetPrototype(), true));
                foreach (HtmlNode htmlNode in ProcessChildNodesIsolated(node, loopCtx))
                {
                    if (prevSilbing == null)
                    {
                        parent.AppendChild(htmlNode);
                    }
                    else
                    {
                        parent.InsertAfter(htmlNode, prevSilbing);
                    }

                    prevSilbing = htmlNode;
                }
            }
        }
        else if (ret is IBadEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                BadObject o = enumerator.Current ?? throw new BadRuntimeException("Invalid Enumerator");
                BadExecutionContext loopCtx = new BadExecutionContext(ctx.Scope.CreateChild("bs:foreach", null, null));
                loopCtx.Scope.DefineVariable(name, o, loopCtx.Scope, new BadPropertyInfo(o.GetPrototype(), true));
                foreach (HtmlNode htmlNode in ProcessChildNodesIsolated(node, loopCtx))
                {
                    if (prevSilbing == null)
                    {
                        parent.AppendChild(htmlNode);
                    }
                    else
                    {
                        parent.InsertAfter(htmlNode, prevSilbing);
                    }

                    prevSilbing = htmlNode;
                }
            }
        }

        node.ParentNode.RemoveChild(node);
    }

    private void ProcessIf(HtmlNode node, BadExecutionContext ctx)
    {
        string? condition = node.Attributes["test"]?.Value;
        if (condition == null)
        {
            throw new BadRuntimeException("Invalid Condition");
        }

        BadSourceParser conditionParser = BadSourceParser.Create(FilePath, condition);
        BadExpression[] conditionExprs = conditionParser.Parse().ToArray();
        BadObject ret = ctx.Execute(conditionExprs).Last().Dereference();
        if (ret is not IBadBoolean b)
        {
            throw new BadRuntimeException("Invalid Condition Return");
        }

        if (b.Value)
        {
            string result = ProcessChildrenIsolated(node, ctx);
            node.ParentNode.InnerHtml = node.ParentNode.InnerHtml.Replace(node.OuterHtml, result);
        }
        else
        {
            node.ParentNode.RemoveChild(node);
        }
    }

    private bool IsSpecialNode(HtmlNode node)
    {
        return node.Name == "bs:if" || node.Name == "bs:foreach" || node.Name == "bs:function";
    }

    private void ProcessSpecial(HtmlNode node, BadExecutionContext ctx)
    {
        if (node.Name == "bs:if")
        {
            ProcessIf(node, ctx);
        }
        else if (node.Name == "bs:foreach")
        {
            ProcessForeach(node, ctx);
        }
        else if (node.Name == "bs:function")
        {
            ProcessFunction(node, ctx);
        }
        else
        {
            throw new Exception("Unknown Special Node");
        }

        //If name is bs:function then
        //  create a function with the specified name and parameters and add it to the scope
        //  Remove the node from the document and return
        //if name is bs:if then
        //  if the condition is true then
        //      remove the node
        //      process the children
        //  else
        //      remove the node and return
        //if name is bs:for then
        //  create for loop with the specified parameters
        //  remove the node and return
        //if name is bs:while then
        //  create while loop with the specified parameters
        //  remove the node and return
        //if name is bs:foreach then
        //  create foreach loop with the specified parameters
        //  remove the node and return
    }

    /// <summary>
    ///     Processes the Node Text of the Current Node
    /// </summary>
    /// <param name="node">The Current Node</param>
    /// <param name="ctx">The Execution Context</param>
    private void ProcessSelf(HtmlNode node, BadExecutionContext ctx)
    {
        if (node.Name == "style")
        {
            return;
        }

        if (IsSpecialNode(node))
        {
            ProcessSpecial(node, ctx);

            return;
        }

        foreach (HtmlAttribute attribute in node.Attributes)
        {
            if (attribute.Name != "style" && attribute.Value.Contains('{'))
            {
                attribute.Value = ProcessInline(attribute.Value, ctx);
            }
        }


        if (node.Name == "script" && node.Attributes.Contains("lang") && node.Attributes["lang"].Value == "bs2")
        {
            ProcessScriptNode(node, ctx);
        }
        else
        {
            ProcessInlineScript(node, ctx);
        }
    }

    /// <summary>
    ///     Processes the Current Node by first processing the children and then the node itself.
    /// </summary>
    /// <param name="node">The Current Node</param>
    /// <param name="ctx">The Execution Context</param>
    private void ProcessNode(HtmlNode node, BadExecutionContext ctx)
    {
        if (CanProcessChildren(node))
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                HtmlNode? child = node.ChildNodes[i];
                if (child != null)
                {
                    ProcessNode(child, ctx);
                }
            }
        }

        ProcessSelf(node, ctx);
    }

    /// <summary>
    ///     Runs the Template Transformation
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="document">The HTML Document</param>
    /// <returns>String Result of the Transformation</returns>
    private string Run(BadExecutionContext ctx, HtmlDocument document)
    {
        BadLogger.Log($"Running Template {FilePath}", "BadHtml");
        ProcessNode(document.DocumentNode, ctx);

        return document.DocumentNode.OuterHtml;
    }

    /// <summary>
    ///     Runs the Template Transformation
    /// </summary>
    /// <param name="model">The Model used in the Transformation</param>
    /// <param name="debug">If true, attaches a debugger</param>
    /// <returns>String Result of the Transformation</returns>
    public string Run(object? model = null, bool debug = false)
    {
        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(Source);
        BadExecutionContext ctx = CreateContext(model, document, debug);

        string s = Run(ctx, document);
        if (debug)
        {
            BadDebugger.Detach();
        }

        return s;
    }

    /// <summary>
    ///     Runs the Template Transformation
    /// </summary>
    /// <param name="filePath">The File Path of the Template</param>
    /// <param name="model">The Model used in the Transformation</param>
    /// <param name="debug">If true, attaches a debugger</param>
    /// <returns>String Result of the Transformation</returns>
    public static string Run(string filePath, object? model = null, bool debug = false)
    {
        return new BadHtmlTemplate(filePath).Run(model, debug);
    }
}