using System.Collections;
using System.Reflection;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
///     Builds a conservative symbol table for compiled function bodies.
///     Only parameters and local variable definitions of the current function are registered.
/// </summary>
internal static class BadFunctionSymbolTableBuilder
{
    public static bool HasNestedFunctionLikeConstruct(BadFunctionExpression expression)
    {
        foreach (BadExpression bodyExpression in expression.Body)
        {
            if (ContainsNestedFunctionLikeConstruct(bodyExpression))
            {
                return true;
            }
        }

        return false;
    }

    public static BadSymbolTable? Build(BadFunctionExpression expression)
    {
        try
        {
            BadSymbolTable table = new BadSymbolTable();
            HashSet<string> scopeDeclaredNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (BadFunctionParameter parameter in expression.Parameters)
            {
                table.RegisterParameter(parameter.Name);
            }

            foreach (BadExpression bodyExpression in expression.Body)
            {
                RegisterLocals(bodyExpression, table, scopeDeclaredNames);
            }

            foreach (BadExpression bodyExpression in expression.Body)
            {
                RegisterCaptures(bodyExpression, table, scopeDeclaredNames);
            }

            return table;
        }
        catch
        {
            // Duplicate or otherwise invalid symbol layouts fall back to the legacy path.
            return null;
        }
    }

    private static bool ContainsNestedFunctionLikeConstruct(BadExpression expression)
    {
        if (expression is BadFunctionExpression or BadClassPrototypeExpression or BadInterfacePrototypeExpression)
        {
            return true;
        }

        foreach (BadExpression child in EnumerateChildExpressions(expression))
        {
            if (ContainsNestedFunctionLikeConstruct(child))
            {
                return true;
            }
        }

        return false;
    }

    private static void RegisterLocals(BadExpression expression, BadSymbolTable table, HashSet<string> scopeDeclaredNames)
    {
        if (expression is BadFunctionExpression namedFunction)
        {
            if (namedFunction.Name != null)
            {
                scopeDeclaredNames.Add(namedFunction.Name.Text);
            }

            return;
        }

        if (expression is BadClassPrototypeExpression classPrototype)
        {
            scopeDeclaredNames.Add(classPrototype.Name);
            return;
        }

        if (expression is BadInterfacePrototypeExpression interfacePrototype)
        {
            scopeDeclaredNames.Add(interfacePrototype.Name);
            return;
        }

        if (expression is BadVariableDefinitionExpression variableDefinition &&
            !table.TryGetSymbol(variableDefinition.Name, out _))
        {
            table.RegisterLocal(variableDefinition.Name, variableDefinition.Position);
        }

        foreach (BadExpression child in EnumerateChildExpressions(expression))
        {
            RegisterLocals(child, table, scopeDeclaredNames);
        }
    }

    private static void RegisterCaptures(BadExpression expression,
                                         BadSymbolTable table,
                                         HashSet<string> scopeDeclaredNames)
    {
        if (expression is BadFunctionExpression or BadClassPrototypeExpression or BadInterfacePrototypeExpression)
        {
            return;
        }

        if (expression is BadVariableExpression variableExpression &&
            expression is not BadVariableDefinitionExpression &&
            !table.TryGetSymbol(variableExpression.Name, out _) &&
            !scopeDeclaredNames.Contains(variableExpression.Name))
        {
            table.RegisterCapture(variableExpression.Name, variableExpression.Position);
        }

        foreach (BadExpression child in EnumerateChildExpressions(expression))
        {
            RegisterCaptures(child, table, scopeDeclaredNames);
        }
    }

    private static IEnumerable<BadExpression> EnumerateChildExpressions(BadExpression expression)
    {
        Type type = expression.GetType();

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length != 0)
            {
                continue;
            }

            object? value;
            try
            {
                value = property.GetValue(expression);
            }
            catch
            {
                continue;
            }

            switch (value)
            {
                case BadExpression child:
                    yield return child;
                    break;
                case IEnumerable enumerable when value is not string:
                    foreach (object? item in enumerable)
                    {
                        if (item is BadExpression childExpression)
                        {
                            yield return childExpression;
                        }
                    }
                    break;
            }
        }
    }
}
