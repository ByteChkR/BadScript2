using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Functions;

/// <summary>
///     Provides function parameter info
/// </summary>
public class BadFunctionParameter
{
    /// <summary>
    ///     Creates a new Function Parameter Info
    /// </summary>
    /// <param name="name">Name of the Parameter</param>
    /// <param name="isOptional">Indicates if this parameter is optional</param>
    /// <param name="isNullChecked">Indicates if this parameter is null checked by the runtime</param>
    /// <param name="isRestArgs">Indicates if this parameter is the rest parameter of the function</param>
    public BadFunctionParameter(
        string name,
        bool isOptional,
        bool isNullChecked,
        bool isRestArgs)
    {
        Name = name;
        IsOptional = isOptional;
        IsNullChecked = isNullChecked;
        IsRestArgs = isRestArgs;
        TypeExpr = null;
        Type = BadAnyPrototype.Instance;
    }

    /// <summary>
    ///     Creates a new Function Parameter Info
    /// </summary>
    /// <param name="name">Name of the Parameter</param>
    /// <param name="isOptional">Indicates if this parameter is optional</param>
    /// <param name="isNullChecked">Indicates if this parameter is null checked by the runtime</param>
    /// <param name="isRestArgs">Indicates if this parameter is the rest parameter of the function</param>
    /// <param name="typeExpr">The Expression that returns the type of the parameter if evaluated</param>
    /// <param name="type">The Class Prototype of the Parameter</param>
    public BadFunctionParameter(
        string name,
        bool isOptional,
        bool isNullChecked,
        bool isRestArgs,
        BadExpression? typeExpr,
        BadClassPrototype? type = null)
    {
        Name = name;
        IsOptional = isOptional;
        IsNullChecked = isNullChecked;
        IsRestArgs = isRestArgs;
        TypeExpr = typeExpr;
        Type = type;

        if (type == null && typeExpr == null)
        {
            Type = BadAnyPrototype.Instance;
        }
    }

    /// <summary>
    ///     The Class Prototype of the Parameter
    /// </summary>
    public BadClassPrototype? Type { get; }

    /// <summary>
    ///     The Expression that returns the type of the parameter if evaluated
    /// </summary>
    public BadExpression? TypeExpr { get; }

    /// <summary>
    ///     The Name of the Parameter
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Indicates if this parameter is optional
    /// </summary>
    public bool IsOptional { get; }

    /// <summary>
    ///     Indicates if this parameter is null checked by the runtime
    /// </summary>
    public bool IsNullChecked { get; }

    /// <summary>
    ///     Indicates if this parameter is the rest parameter of the function
    /// </summary>
    public bool IsRestArgs { get; }

    /// <summary>
    ///     Initializes the Function Parameter
    /// </summary>
    /// <param name="context">The Execution context</param>
    /// <returns>The Function Parameter</returns>
    /// <exception cref="BadRuntimeException">
    ///     Gets raised if the Type Expression is not null and not a
    ///     <see cref="BadClassPrototype" />
    /// </exception>
    public BadFunctionParameter Initialize(BadExecutionContext context)
    {
        BadClassPrototype? type = Type;

        if (TypeExpr == null)
        {
            return new BadFunctionParameter(Name, IsOptional, IsNullChecked, IsRestArgs, TypeExpr, type);
        }

        BadObject obj = BadObject.Null;

        foreach (BadObject o in TypeExpr.Execute(context))
        {
            obj = o;
        }

        obj = obj.Dereference();

        if (obj is not BadClassPrototype proto)
        {
            throw new BadRuntimeException("Type expression must return a class prototype.");
        }

        type = proto;

        return new BadFunctionParameter(Name, IsOptional, IsNullChecked, IsRestArgs, TypeExpr, type);
    }


    /// <summary>
    ///     Converts a string to a function parameter. all properties set to false and no type is specified
    /// </summary>
    /// <param name="s">The Parameter name</param>
    /// <returns>The Function Parameter</returns>
    public static implicit operator BadFunctionParameter(string s)
    {
        return new BadFunctionParameter(s, false, false, false, null);
    }

    /// <summary>
    ///     Returns the string representation of the Function Parameter
    /// </summary>
    /// <returns>The String Representation</returns>
    public override string ToString()
    {
        return Type == null
            ? $"{Name}{(IsOptional ? "?" : "")}{(IsNullChecked ? "!" : "")}{(IsRestArgs ? "*" : "")}"
            : $"{Type.Name} {Name}{(IsOptional ? "?" : "")}{(IsNullChecked ? "!" : "")}{(IsRestArgs ? "*" : "")}";
    }
}