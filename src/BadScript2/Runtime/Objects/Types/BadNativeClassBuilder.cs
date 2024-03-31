using BadScript2.Common;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Helper Class that Builds a Native Class from a Prototype
/// </summary>
public static class BadNativeClassBuilder
{
    /// <summary>
    ///     The IDisposible Interface Prototype
    /// </summary>
    public static readonly BadInterfacePrototype Disposable =
        new BadInterfacePrototype("IDisposable", (typeArgs) => Array.Empty<BadInterfacePrototype>(), null, DisposableConstraints, Array.Empty<string>());

    /// <summary>
    ///     The IEnumerable Interface Prototype
    /// </summary>
    public static readonly BadInterfacePrototype Enumerable =
        new BadInterfacePrototype("IEnumerable", (typeArgs) => Array.Empty<BadInterfacePrototype>(), null, EnumerableConstraints, new []{"T"});

    /// <summary>
    ///     The IEnumerator Interface Prototype
    /// </summary>
    public static readonly BadInterfacePrototype Enumerator =
        new BadInterfacePrototype("IEnumerator", (typeArgs) => Array.Empty<BadInterfacePrototype>(), null, EnumeratorConstraints, new []{"T"});

    /// <summary>
    ///     The IArray Interface Prototype
    /// </summary>
    public static readonly BadInterfacePrototype ArrayLike =
        new BadInterfacePrototype("IArray", (typeArgs) => new[] { (BadInterfacePrototype)Enumerable.CreateGeneric(typeArgs) }, null, ArrayConstraints, new []{"T"});

    public static readonly BadInterfacePrototype ImportHandler =
        new BadInterfacePrototype("IImportHandler", (typeArgs) => Array.Empty<BadInterfacePrototype>(), null, ImportHandlerConstraints, Array.Empty<string>());

    private static readonly Dictionary<string, BadObjectReference> s_NumberStaticMembers = new Dictionary<string, BadObjectReference>
    {
        {
            "Parse", BadObjectReference.Make(
                "num.Parse",
                () => new BadInteropFunction(
                    "Parse",
                    ParseNumber,
                    true,
                    GetNative("num"),
                    new BadFunctionParameter("str", false, true, false, null, GetNative("string"))
                )
            )
        }
    };
    
    private static readonly Dictionary<string, BadObjectReference> s_BooleanStaticMembers = new Dictionary<string, BadObjectReference>
    {
        {
            "Parse", BadObjectReference.Make(
                "bool.Parse",
                () => new BadInteropFunction(
                    "Parse",
                    ParseBoolean,
                    true,
                    GetNative("bool"),
                    new BadFunctionParameter("str", false, true, false, null, GetNative("string"))
                )
            )
        }
    };
    
    private static readonly Dictionary<string, BadObjectReference> s_StringStaticMembers = new Dictionary<string, BadObjectReference>
    {
        { "Empty", BadObjectReference.Make("string.Empty", () => BadString.Empty) },
        { "IsNullOrEmpty" ,BadObjectReference.Make(
            "string.IsNullOrEmpty",
            () => new BadInteropFunction(
                "IsNullOrEmpty",
                StringIsNullOrEmpty,
                true,
                GetNative("bool"),
                new BadFunctionParameter("str", false, false, false, null, GetNative("string"))
            )
        )}
    };

    
    private static BadObject StringIsNullOrEmpty(BadExecutionContext ctx, BadObject[] arg)
    {
        if (arg[0] is not IBadString str)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Type");
        }

        return string.IsNullOrEmpty(str.Value);
    }
    private static BadObject ParseNumber(BadExecutionContext ctx, BadObject[] arg)
    {
        if (arg[0] is not IBadString str)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Type");
        }

        if (decimal.TryParse(str.Value, out decimal d))
        {
            return d;
        }

        throw BadRuntimeException.Create(ctx.Scope, $"The Supplied String '{str.Value}' is not a valid number");
    }
    
    private static BadObject ParseBoolean(BadExecutionContext ctx, BadObject[] arg)
    {
        if (arg[0] is not IBadString str)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Type");
        }

        if (bool.TryParse(str.Value, out bool d))
        {
            return d;
        }

        throw BadRuntimeException.Create(ctx.Scope, $"The Supplied String '{str.Value}' is not a valid boolean");
    }
        
    /// <summary>
    ///     Collection of all Native Class Prototypes
    /// </summary>
    private static readonly List<BadClassPrototype> s_NativeTypes = new List<BadClassPrototype>
    {
        BadAnyPrototype.Instance,
        CreateNativeType<BadString>("string", s_StringStaticMembers, () => Array.Empty<BadInterfacePrototype>()),
        CreateNativeType<BadBoolean>("bool", s_BooleanStaticMembers, () => Array.Empty<BadInterfacePrototype>()),
        CreateNativeType<BadNumber>("num", s_NumberStaticMembers, () => Array.Empty<BadInterfacePrototype>()),
        CreateNativeType<BadFunction>("Function", () => Array.Empty<BadInterfacePrototype>()),
        CreateNativeType<BadArray>("Array", () => new []{(BadInterfacePrototype)ArrayLike.CreateGeneric(new []{BadAnyPrototype.Instance})}),
        CreateNativeType<BadTable>("Table", () => new []{(BadInterfacePrototype)Enumerable.CreateGeneric(new []{BadAnyPrototype.Instance})}),
        BadScope.Prototype,
        BadClassPrototype.Prototype,
        BadRuntimeError.Prototype,
        Enumerable,
        Enumerator,
        Disposable,
        ArrayLike,
        ImportHandler,
    };

    /// <summary>
    ///     Enumeration of all Native Class Prototypes
    /// </summary>
    public static IEnumerable<BadClassPrototype> NativeTypes => s_NativeTypes;

    /// <summary>
    ///     The IDisposible Interface Constraints
    /// </summary>
    /// <returns>The Constraints</returns>
    private static BadInterfaceConstraint[] DisposableConstraints(BadObject[] typeParams)
    {
        return new BadInterfaceConstraint[]
        {
            new BadInterfaceFunctionConstraint("Dispose", null, Array.Empty<BadFunctionParameter>()),
        };
    }

    /// <summary>
    ///     The IEnumerable Interface Constraints
    /// </summary>
    /// <returns>The Constraints</returns>
    private static BadInterfaceConstraint[] EnumerableConstraints(BadObject[] typeParams)
    {
        return new BadInterfaceConstraint[]
        {
            new BadInterfaceFunctionConstraint("GetEnumerator", null, (BadClassPrototype)Enumerator.CreateGeneric(typeParams), Array.Empty<BadFunctionParameter>()),
        };
    }


    private static BadInterfaceConstraint[] ImportHandlerConstraints(BadObject[] typeParams)
    {
        return new BadInterfaceConstraint[]
        {
            new BadInterfaceFunctionConstraint(
                "GetHash",
                null,
                GetNative("string"),
                new[]
                {
                    new BadFunctionParameter("path", false, true, false, null, GetNative("string")),
                }
            ),
            new BadInterfaceFunctionConstraint(
                "Has",
                null,
                GetNative("bool"),
                new[]
                {
                    new BadFunctionParameter("path", false, true, false, null, GetNative("string")),
                }
            ),
            new BadInterfaceFunctionConstraint(
                "Get",
                null,
                BadAnyPrototype.Instance,
                new[]
                {
                    new BadFunctionParameter("path", false, true, false, null, GetNative("string")),
                }
            ),
            new BadInterfaceFunctionConstraint(
                "IsTransient",
                null,
                GetNative("bool"),
                Array.Empty<BadFunctionParameter>()
            ),
        };
    }

    /// <summary>
    ///     The IArray Interface Constraints
    /// </summary>
    /// <returns>The Constraints</returns>
    private static BadInterfaceConstraint[] ArrayConstraints(BadObject[] typeParams)
    {
        BadClassPrototype? listType = typeParams.Length > 0 ? (BadClassPrototype)typeParams[0] : BadAnyPrototype.Instance;
        return new BadInterfaceConstraint[]
        {
            //Add(any elem);
            new BadInterfaceFunctionConstraint("Add", null, new[] { new BadFunctionParameter("elem", false, false, false, null, listType) }),

            //AddRange(IEnumerable elems);
            new BadInterfaceFunctionConstraint("AddRange", null, new[] { new BadFunctionParameter("elems", false, false, false, null, (BadClassPrototype)Enumerable.CreateGeneric(typeParams)) }),

            //Clear();
            new BadInterfaceFunctionConstraint("Clear", null, Array.Empty<BadFunctionParameter>()),

            //Insert(num index, any elem);
            new BadInterfaceFunctionConstraint(
                "Insert",
                null,
                new[]
                {
                    new BadFunctionParameter("index", false, false, false, null, GetNative("num")),
                    new BadFunctionParameter("elem", false, false, false, null, listType),
                }
            ),

            //InsertRange(num index, IEnumerable elems);
            new BadInterfaceFunctionConstraint(
                "InsertRange",
                null,
                new[]
                {
                    new BadFunctionParameter("index", false, false, false, null, GetNative("num")),
                    new BadFunctionParameter("elems", false, false, false, null, (BadClassPrototype)Enumerable.CreateGeneric(typeParams)),
                }
            ),

            //bool Remove(any elem);
            new BadInterfaceFunctionConstraint("Remove", null, GetNative("bool"), new[] { new BadFunctionParameter("elem", false, false, false, null, listType) }),

            //RemoveAt(num index);
            new BadInterfaceFunctionConstraint("RemoveAt", null, new[] { new BadFunctionParameter("index", false, false, false, null, GetNative("num")) }),

            //bool Contains(any elem);
            new BadInterfaceFunctionConstraint("Contains", null, GetNative("bool"), new[] { new BadFunctionParameter("elem", false, false, false, null, listType) }),

            //Get(num index);
            new BadInterfaceFunctionConstraint("Get", null, listType, new[] { new BadFunctionParameter("index", false, false, false, null, GetNative("num")) }),

            //Set(num index, any elem);
            new BadInterfaceFunctionConstraint(
                "Set",
                null,
                new[]
                {
                    new BadFunctionParameter("index", false, false, false, null, GetNative("num")),
                    new BadFunctionParameter("elem", false, false, false, null, listType),
                }
            ),

            //op_ArrayAccess(num index);
            new BadInterfaceFunctionConstraint(
                BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
                null,
                BadAnyPrototype.Instance,
                new[] { new BadFunctionParameter("index", false, false, false, null, GetNative("num")) }
            ),

            //op_ArrayAccessReverse(num index);
            new BadInterfaceFunctionConstraint(
                BadStaticKeys.ARRAY_ACCESS_REVERSE_OPERATOR_NAME,
                null,
                BadAnyPrototype.Instance,
                new[] { new BadFunctionParameter("index", false, false, false, null, GetNative("num")) }
            ),
            new BadInterfacePropertyConstraint("Length", null, GetNative("num")),
        };
    }

    /// <summary>
    ///     The IEnumerator Interface Constraints
    /// </summary>
    /// <returns>The Constraints</returns>
    private static BadInterfaceConstraint[] EnumeratorConstraints(BadObject[] typeParams)
    {
        BadClassPrototype? type = (BadClassPrototype)(typeParams.Length > 0 ? typeParams[0] : BadAnyPrototype.Instance);
        return new BadInterfaceConstraint[]
        {
            new BadInterfaceFunctionConstraint("MoveNext", null, GetNative("bool"), Array.Empty<BadFunctionParameter>()),
            new BadInterfaceFunctionConstraint("GetCurrent", null, type, Array.Empty<BadFunctionParameter>()),
        };
    }


    /// <summary>
    ///     Returns a Native Class Prototype for the given Native Type
    /// </summary>
    /// <param name="name">Type Name</param>
    /// <returns>The Prototype</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the prototype does not exist.</exception>
    public static BadClassPrototype GetNative(string name)
    {
        return s_NativeTypes.FirstOrDefault(x => x.Name == name) ??
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

        s_NativeTypes.Add(native);
    }


    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Type Name</param>
    /// <param name="constructor">The Constructor</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <typeparam name="T">The BadObject Type</typeparam>
    /// <returns>The Prototype</returns>
    private static BadNativeClassPrototype<T> Create<T>(
        string name,
        Func<BadObject[], BadObject> constructor,
        Func<BadInterfacePrototype[]> interfaces) where T : BadObject
    {
        return new BadNativeClassPrototype<T>(
            name,
            (_, a) => constructor(a),
            interfaces
        );
    }
    
    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Type Name</param>
    /// <param name="constructor">The Constructor</param>
    /// <param name="staticMembers">The Static Members</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <typeparam name="T">The BadObject Type</typeparam>
    /// <returns>The Prototype</returns>
    private static BadNativeClassPrototype<T> Create<T>(
        string name,
        Func<BadObject[], BadObject> constructor,
        Dictionary<string, BadObjectReference> staticMembers,
        Func<BadInterfacePrototype[]> interfaces) where T : BadObject
    {
        return new BadNativeClassPrototype<T>(
            name,
            (_, a) => constructor(a),
            staticMembers,
            interfaces
        );
    }

    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Name of the Type</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <typeparam name="T">BadObject Type</typeparam>
    /// <returns>Class Prototype</returns>
    private static BadNativeClassPrototype<T> CreateNativeType<T>(string name, Func<BadInterfacePrototype[]> interfaces) where T : BadObject
    {
        return CreateNativeType<T>(name, BadNativeClassHelper.GetConstructor(name), interfaces);
    }
    

    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Name of the Type</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <param name="staticMembers">The Static Members</param>
    /// <typeparam name="T">BadObject Type</typeparam>
    /// <returns>Class Prototype</returns>
    private static BadNativeClassPrototype<T> CreateNativeType<T>(string name, Dictionary<string, BadObjectReference> staticMembers, Func<BadInterfacePrototype[]> interfaces) where T : BadObject
    {
        return CreateNativeType<T>(name, BadNativeClassHelper.GetConstructor(name), staticMembers, interfaces);
    }


    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Type Name</param>
    /// <param name="constructor">The Constructor</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <typeparam name="T">The BadObject Type</typeparam>
    /// <returns>The Prototype</returns>
    private static BadNativeClassPrototype<T> CreateNativeType<T>(
        string name,
        Func<BadObject[], BadObject> constructor,
        Func<BadInterfacePrototype[]> interfaces) where T : BadObject
    {
        return Create<T>(name, constructor, interfaces);
    }
    
    /// <summary>
    ///     Creates a Native Class Prototype
    /// </summary>
    /// <param name="name">Type Name</param>
    /// <param name="constructor">The Constructor</param>
    /// <param name="staticMembers">The Static Members</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <typeparam name="T">The BadObject Type</typeparam>
    /// <returns>The Prototype</returns>
    private static BadNativeClassPrototype<T> CreateNativeType<T>(
        string name,
        Func<BadObject[], BadObject> constructor,
        Dictionary<string, BadObjectReference> staticMembers,
        Func<BadInterfacePrototype[]> interfaces) where T : BadObject
    {
        return Create<T>(name, constructor, staticMembers, interfaces);
    }
}