using BadScript2.Parser;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Error;

namespace BadScript2.Runtime.Objects.Types.Interface;
public interface IBadGenericObject
{
    bool IsResolved { get; }
    bool IsGeneric { get; }
    string GenericName { get; }
    IReadOnlyCollection<string> GenericParameters { get; }
    BadObject CreateGeneric(BadObject[] args);
}
/// <summary>
///     Implements a BadScript Interface Prototype
/// </summary>
public class BadInterfacePrototype : BadClassPrototype, IBadGenericObject
{
    private readonly Dictionary<int, BadInterfacePrototype> s_GenericCache = new Dictionary<int, BadInterfacePrototype>();
    /// <summary>
    ///     The Prototype for the Interface Prototype
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadClassPrototype>(
        "Interface",
        (_, _) => throw new BadRuntimeException("Interfaces cannot be extended")
    );

    public bool IsGeneric => GenericParameters.Count != 0;

    public string GenericName { get; }
    /// <summary>
    ///     Backing Function of the Constraints Property
    /// </summary>
    private readonly Func<BadObject[], BadInterfaceConstraint[]> m_ConstraintsFunc;
    
    private readonly Func<BadObject[], BadInterfacePrototype[]> m_InterfacesFunc;
    private BadInterfacePrototype[]? m_Interfaces;

    /// <summary>
    ///     The Constraints of this Interface
    /// </summary>
    private BadInterfaceConstraint[]? m_Constraints;

    /// <summary>
    ///     Creates a new Interface Prototype
    /// </summary>
    /// <param name="name">The Name of the Interface</param>
    /// <param name="interfaces">The Interfaces this Interface extends</param>
    /// <param name="metaData">The Meta Data of the Interface</param>
    /// <param name="constraints">The Constraints of the Interface</param>
    public BadInterfacePrototype(
        string name,
        Func<BadObject[], BadInterfacePrototype[]> interfaces,
        BadMetaData? metaData,
        Func<BadObject[], BadInterfaceConstraint[]> constraints,
        string[] genericParameters) : base(
        name,
        metaData
    )
    {
        m_ConstraintsFunc = constraints;
        GenericParameters = genericParameters;
        m_InterfacesFunc = interfaces;
        if(IsGeneric)
        {
            GenericName = $"{name}<{string.Join(", ", genericParameters)}>";
        }
        else
        {
            GenericName = name;
        }
    }
    
    /// <summary>
    ///     Creates a new Interface Prototype
    /// </summary>
    /// <param name="name">The Name of the Interface</param>
    /// <param name="interfaces">The Interfaces this Interface extends</param>
    /// <param name="metaData">The Meta Data of the Interface</param>
    /// <param name="constraints">The Constraints of the Interface</param>
    private BadInterfacePrototype(
        string name,
        Func<BadObject[], BadInterfacePrototype[]> interfaces,
        BadMetaData? metaData,
        Func<BadObject[], BadInterfaceConstraint[]> constraints,
        string[] genericParameters,
        BadInterfacePrototype genericDefinition,
        string genericName) : base(
        name,
        metaData
    )
    {
        GenericName = genericName;
        m_ConstraintsFunc = constraints;
        m_InterfacesFunc = interfaces;
        IsResolved = true;
        GenericParameters = genericParameters;
        m_GenericDefinition = genericDefinition;
    }


    protected override BadClassPrototype? BaseClass { get; }

    /// <inheritdoc />
    public override bool IsAbstract => true;

    public override IReadOnlyCollection<BadInterfacePrototype> Interfaces => m_Interfaces ??= m_InterfacesFunc(Array.Empty<BadObject>());

    /// <summary>
    ///     The Constraints of this Interface
    /// </summary>
    public IEnumerable<BadInterfaceConstraint> Constraints => m_Constraints ??= m_ConstraintsFunc(Array.Empty<BadObject>());

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc />
    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        throw BadRuntimeException.Create(caller.Scope, "Interfaces cannot be instantiated");
    }

    /// <inheritdoc />
    public override bool IsSuperClassOf(BadClassPrototype proto)
    {
        return GenericParameters.Count != 0 && proto is BadInterfacePrototype iProto && iProto.m_GenericDefinition == this || 
               base.IsSuperClassOf(proto) || 
               proto.Interfaces.Any(IsSuperClassOf);
    }


    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        if(IsGeneric)
            return IsResolved ? $"interface {GenericName}" : $"interface {Name}<{string.Join(", ", GenericParameters)}>";
        return $"interface {Name}";
    }

    public bool IsResolved { get; }
    private readonly BadInterfacePrototype? m_GenericDefinition;
    public IReadOnlyCollection<string> GenericParameters { get; }

    public BadObject CreateGeneric(BadObject[] args)
    {
        if(GenericParameters.Count != args.Length)
            throw new BadRuntimeException("Invalid Generic Argument Count");
        if(IsResolved)
            throw new BadRuntimeException("Interface is already resolved");

        int hash = args[0].GetHashCode();
        //Add the other arguments to the hash
        for(int i = 1; i < args.Length; i++)
        {
            hash = (hash * 397) ^ args[i].GetHashCode();
        }
        
        if(s_GenericCache.TryGetValue(hash, out BadInterfacePrototype? cached))
        {
            return cached;
        }

        var types = args.Cast<BadClassPrototype>().ToArray();
        var result= new BadInterfacePrototype(Name, _ => m_InterfacesFunc(args), MetaData, _ => m_ConstraintsFunc(args), GenericParameters.ToArray(), this,
            $"{Name}<{string.Join(", ", types.Select(x=> x is IBadGenericObject g ? g.GenericName : x.Name))}>");
        s_GenericCache[hash] = result;
        return result;
    }
}