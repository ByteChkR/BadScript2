using BadScript2.Parser;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Helper Class that Builds a Native Class from a Prototype
/// </summary>
public static class BadNativeClassBuilder
{
    private static readonly List<BadClassPrototype> s_NativeTypes = new List<BadClassPrototype>
    {
        CreateNativeType<BadString>("string"),
        CreateNativeType<BadBoolean>("bool"),
        CreateNativeType<BadNumber>("num"),
        CreateNativeType<BadFunction>("Function"),
        CreateNativeType<BadArray>("Array"),
        CreateNativeType<BadTable>("Table"),
        BadScope.Prototype,
    };

    /// <summary>
    ///     Enumeration of all Native Class Prototypes
    /// </summary>
    public static IEnumerable<BadClassPrototype> NativeTypes => s_NativeTypes;


    /// <summary>
    ///     Returns a Native Class Prototype for the given Native Type
    /// </summary>
    /// <param name="name">Type Name</param>
    /// <returns>The Prototype</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the prototype does not exist.</exception>
    public static BadClassPrototype GetNative(string name)
    {
        return s_NativeTypes?.FirstOrDefault(x => x.Name == name) ??
               throw new BadRuntimeException("Native class not found");
    }

    /// <summary>
    ///     Adds a native Type
    /// </summary>
    /// <param name="native">The native type</param>
    public static void AddNative(BadClassPrototype native)
    {
        if (s_NativeTypes.Any(x => x.Name == native.Name))
        {
            return;
        }

        s_NativeTypes?.Add(native);
    }


    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Type Name</param>
    /// <param name="constructor">The Constructor</param>
    /// <typeparam name="T">The BadObject Type</typeparam>
    /// <returns>The Prototype</returns>
    private static BadNativeClassPrototype<T> Create<T>(
        string name,
        Func<BadObject[], BadObject> constructor) where T : BadObject
    {
        return new BadNativeClassPrototype<T>(
            name,
            (_, a) => constructor(a)
        );
    }

    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Name of the Type</param>
    /// <typeparam name="T">BadObject Type</typeparam>
    /// <returns>Class Prototype</returns>
    private static BadNativeClassPrototype<T> CreateNativeType<T>(
        string name) where T : BadObject
    {
        return CreateNativeType<T>(name, BadNativeClassHelper.GetConstructor(name));
    }

    /// <summary>
    ///     Creates a Native Class Prototype from a File
    /// </summary>
    /// <param name="name">Name of the Prototype</param>
    /// <param name="file">File Name of the Script containing the class definition</param>
    /// <returns>New Prototype</returns>
    /// <exception cref="BadRuntimeException">
    ///     Gets raised if the returned type does not have the name provided or the resulting
    ///     Object is not a Class Prototype
    /// </exception>
    private static BadClassPrototype CreateNativeType(string name, string file)
    {
        BadSourceParser parser = new BadSourceParser(BadSourceReader.FromFile(file), BadOperatorTable.Instance);
        BadExecutionContext ctx = new BadExecutionContext(
            new BadScope("<root>", null, BadScopeFlags.AllowThrow | BadScopeFlags.Returnable)
        );
        BadObject? obj = ctx.Run(parser.Parse());
        if (obj is not BadClassPrototype cls)
        {
            throw new BadRuntimeException("Failed to create native type " + name);
        }

        if (cls.Name != name)
        {
            throw new BadRuntimeException("Failed to create native type " + name);
        }

        return cls;
    }

    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Type Name</param>
    /// <param name="constructor">The Constructor</param>
    /// <typeparam name="T">The BadObject Type</typeparam>
    /// <returns>The Prototype</returns>
    private static BadNativeClassPrototype<T> CreateNativeType<T>(
        string name,
        Func<BadObject[], BadObject> constructor) where T : BadObject
    {
        return Create<T>(name, constructor);
    }
}