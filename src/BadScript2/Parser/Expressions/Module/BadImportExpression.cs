using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Module;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Module;

/// <summary>
///     A Import Expression that is used to import a module from a specified path
/// </summary>
public class BadImportExpression : BadExpression
{
    /// <summary>
    ///     The Name of the Import
    /// </summary>
    public readonly string Name;

    /// <summary>
    ///     The Path to import
    /// </summary>
    public readonly string Path;

    /// <summary>
    ///     Creates a new Import Expression
    /// </summary>
    /// <param name="name">The Name of the Import</param>
    /// <param name="path">The Path to import</param>
    /// <param name="position">The Source Position of the Expression</param>
    public BadImportExpression(string name, string path, BadSourcePosition position) : base(false, position)
    {
        Name = name;
        Path = path;
    }

    /// <inheritdoc />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        yield break;
    }

    /// <summary>
    ///     Imports a module from the specified path and assigns it to the specified name
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="name">The Name of the Import</param>
    /// <param name="path">The Path to import</param>
    /// <exception cref="BadRuntimeException">If the Module Importer is not found</exception>
    public static IEnumerable<BadObject> Import(BadExecutionContext ctx, string name, string path)
    {
        BadModuleImporter? importer = ctx.Scope.GetSingleton<BadModuleImporter>();
        if (importer == null)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Module Importer not found");
        }

        IEnumerable<BadObject> result = importer.Get(path);
        BadObject r = BadObject.Null;
        foreach (BadObject o in result)
        {
            r = o;

            yield return o;
        }

        r = r.Dereference();

        yield return r;
        if (ctx.Scope.HasLocal(name, ctx.Scope, false))
        {
            throw BadRuntimeException.Create(ctx.Scope, $"Variable '{name}' already defined in current scope.");
        }

        ctx.Scope.DefineVariable(name, r, ctx.Scope, new BadPropertyInfo(r.GetPrototype(), true));
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        return Import(context, Name, Path);
    }
}