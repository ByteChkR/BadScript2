using BadScript2.Common;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Runtime.Error;
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
        new BadInterfacePrototype("IDisposable", Array.Empty<BadInterfacePrototype>(), null, DisposableConstraints);

    /// <summary>
    ///     The IEnumerable Interface Prototype
    /// </summary>
    public static readonly BadInterfacePrototype Enumerable =
        new BadInterfacePrototype("IEnumerable", Array.Empty<BadInterfacePrototype>(), null, EnumerableConstraints);

    /// <summary>
    ///     The IEnumerator Interface Prototype
    /// </summary>
    public static readonly BadInterfacePrototype Enumerator =
        new BadInterfacePrototype("IEnumerator", Array.Empty<BadInterfacePrototype>(), null, EnumeratorConstraints);

    /// <summary>
    ///     The IArray Interface Prototype
    /// </summary>
    public static readonly BadInterfacePrototype ArrayLike =
        new BadInterfacePrototype("IArray", new[] { Enumerable }, null, ArrayConstraints);

    public static readonly BadInterfacePrototype ImportHandler =
        new BadInterfacePrototype("IImportHandler", Array.Empty<BadInterfacePrototype>(), null, ImportHandlerConstraints);


    /// <summary>
    ///     Collection of all Native Class Prototypes
    /// </summary>
    private static readonly List<BadClassPrototype> s_NativeTypes = new List<BadClassPrototype>
    {
        BadAnyPrototype.Instance,
        CreateNativeType<BadString>("string"),
        CreateNativeType<BadBoolean>("bool"),
        CreateNativeType<BadNumber>("num"),
        CreateNativeType<BadFunction>("Function"),
        CreateNativeType<BadArray>("Array", ArrayLike),
        CreateNativeType<BadTable>("Table", Enumerable),
        BadScope.Prototype,
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
    private static BadInterfaceConstraint[] DisposableConstraints()
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
    private static BadInterfaceConstraint[] EnumerableConstraints()
    {
        return new BadInterfaceConstraint[]
        {
            new BadInterfaceFunctionConstraint("GetEnumerator", null, Enumerator, Array.Empty<BadFunctionParameter>()),
        };
    }


    private static BadInterfaceConstraint[] ImportHandlerConstraints()
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
        };
    }

    /// <summary>
    ///     The IArray Interface Constraints
    /// </summary>
    /// <returns>The Constraints</returns>
    private static BadInterfaceConstraint[] ArrayConstraints()
    {
        return new BadInterfaceConstraint[]
        {
            //Add(any elem);
            new BadInterfaceFunctionConstraint("Add", null, new[] { new BadFunctionParameter("elem", false, false, false, null, BadAnyPrototype.Instance) }),

            //AddRange(IEnumerable elems);
            new BadInterfaceFunctionConstraint("AddRange", null, new[] { new BadFunctionParameter("elems", false, false, false, null, Enumerable) }),

            //Clear();
            new BadInterfaceFunctionConstraint("Clear", null, Array.Empty<BadFunctionParameter>()),

            //Insert(num index, any elem);
            new BadInterfaceFunctionConstraint(
                "Insert",
                null,
                new[]
                {
                    new BadFunctionParameter("index", false, false, false, null, GetNative("num")),
                    new BadFunctionParameter("elem", false, false, false, null, BadAnyPrototype.Instance),
                }
            ),

            //InsertRange(num index, IEnumerable elems);
            new BadInterfaceFunctionConstraint(
                "InsertRange",
                null,
                new[]
                {
                    new BadFunctionParameter("index", false, false, false, null, GetNative("num")),
                    new BadFunctionParameter("elems", false, false, false, null, Enumerable),
                }
            ),

            //bool Remove(any elem);
            new BadInterfaceFunctionConstraint("Remove", null, GetNative("bool"), new[] { new BadFunctionParameter("elem", false, false, false, null, BadAnyPrototype.Instance) }),

            //RemoveAt(num index);
            new BadInterfaceFunctionConstraint("RemoveAt", null, new[] { new BadFunctionParameter("index", false, false, false, null, GetNative("num")) }),

            //bool Contains(any elem);
            new BadInterfaceFunctionConstraint("Contains", null, GetNative("bool"), new[] { new BadFunctionParameter("elem", false, false, false, null, BadAnyPrototype.Instance) }),

            //Get(num index);
            new BadInterfaceFunctionConstraint("Get", null, BadAnyPrototype.Instance, new[] { new BadFunctionParameter("index", false, false, false, null, GetNative("num")) }),

            //Set(num index, any elem);
            new BadInterfaceFunctionConstraint(
                "Set",
                null,
                new[]
                {
                    new BadFunctionParameter("index", false, false, false, null, GetNative("num")),
                    new BadFunctionParameter("elem", false, false, false, null, BadAnyPrototype.Instance),
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
    private static BadInterfaceConstraint[] EnumeratorConstraints()
    {
        return new BadInterfaceConstraint[]
        {
            new BadInterfaceFunctionConstraint("MoveNext", null, GetNative("bool"), Array.Empty<BadFunctionParameter>()),
            new BadInterfaceFunctionConstraint("GetCurrent", null, BadAnyPrototype.Instance, Array.Empty<BadFunctionParameter>()),
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
        params BadInterfacePrototype[] interfaces) where T : BadObject
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
    /// <param name="name">Name of the Type</param>
    /// <param name="interfaces">The Implemented Interfaces</param>
    /// <typeparam name="T">BadObject Type</typeparam>
    /// <returns>Class Prototype</returns>
    private static BadNativeClassPrototype<T> CreateNativeType<T>(string name, params BadInterfacePrototype[] interfaces) where T : BadObject
    {
        return CreateNativeType<T>(name, BadNativeClassHelper.GetConstructor(name), interfaces);
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
        params BadInterfacePrototype[] interfaces) where T : BadObject
    {
        return Create<T>(name, constructor, interfaces);
    }
}