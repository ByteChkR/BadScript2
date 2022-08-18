using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Functions;

public class BadFunctionParameter
{
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
    }

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
    }

    public BadClassPrototype? Type { get; }

    public BadExpression? TypeExpr { get; }
    public string Name { get; }
    public bool IsOptional { get; }
    public bool IsNullChecked { get; }
    public bool IsRestArgs { get; }

    public BadFunctionParameter Initialize(BadExecutionContext context)
    {
        BadClassPrototype? type = null;
        if (TypeExpr != null)
        {
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
        }

        return new BadFunctionParameter(Name, IsOptional, IsNullChecked, IsRestArgs, TypeExpr, type);
    }


    public static implicit operator BadFunctionParameter(string s)
    {
        return new BadFunctionParameter(s, false, false, false, null);
    }

    public override string ToString()
    {
        return Name + (IsOptional ? "?" : "") + (IsNullChecked ? "!" : "") + (IsRestArgs ? "*" : "");
    }
}