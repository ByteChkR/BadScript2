using BadScript2.Parser;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Objects.Types;

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
    };

    public static IEnumerable<BadClassPrototype> NativeTypes => s_NativeTypes;

    public static BadClassPrototype GetNative(string name)
    {
        return s_NativeTypes?.FirstOrDefault(x => x.Name == name) ??
               throw new BadRuntimeException("Native class not found");
    }

    public static void AddNative(BadClassPrototype native)
    {
        s_NativeTypes?.Add(native);
    }


    private static BadNativeClassPrototype<T> Create<T>(
        string name,
        Func<BadObject[], BadObject> constructor) where T : BadObject
    {
        return new BadNativeClassPrototype<T>(
            name,
            (c, a) => constructor(a)
        );
    }

    private static BadNativeClassPrototype<T> CreateNativeType<T>(
        string name) where T : BadObject
    {
        return CreateNativeType<T>(name, BadNativeClassHelper.GetConstructor(name));
    }

    private static BadClassPrototype CreateNativeType(string name, string file)
    {
        BadSourceParser parser = new BadSourceParser(BadSourceReader.FromFile(file), BadOperatorTable.Default);
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

    private static BadNativeClassPrototype<T> CreateNativeType<T>(
        string name,
        Func<BadObject[], BadObject> constructor) where T : BadObject
    {
        return Create<T>(name, constructor);
    }
}