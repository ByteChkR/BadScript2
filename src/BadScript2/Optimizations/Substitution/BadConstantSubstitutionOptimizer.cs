using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains the BadScript2 Constant Substitution Optimizations
/// </summary>
namespace BadScript2.Optimizations.Substitution;

/// <summary>
///     Contains the Implementation of the Constant Substitution Optimization
///     This optimization replaces expressions that only contain constants and constant variables with the result of the
///     expression.
/// </summary>
public static class BadConstantSubstitutionOptimizer
{
    //Try to find expressions that only reference constants AND variables that are marked constants.
    // => traverse all expressions and keep track of all variables that are marked as constant.
    // => if we find an expression that only contains constants and variables that are marked as constant, we can optimize it.

    private static bool OnlyContainsConstantsAndConstantVariables(BadConstantSubstitutionOptimizerScope scope,
                                                                  BadExpression expr)
    {
        foreach (BadExpression expression in expr.GetDescendantsAndSelf())
        {
            if ((expression is BadVariableExpression vExpr && scope.IsConstant(vExpr.Name)) ||
                expression.IsConstant ||
                expression is BadBinaryExpression ||
                expression is BadConstantExpression)
            {
                continue;
            }

            return false;
        }

        return true;
    }

    /// <summary>
    ///     Substitutes all variables in the expression with their constant value.
    /// </summary>
    /// <param name="scope">The scope to use</param>
    /// <param name="expr">The expression to substitute</param>
    /// <returns>The substituted expression</returns>
    private static BadExpression Substitute(BadConstantSubstitutionOptimizerScope scope, BadExpression expr)
    {
        switch (expr)
        {
            case BadVariableExpression vExpr:
            {
                BadExpression constant = scope.GetConstant(vExpr.Name);
                BadLogger.Log($"Substituting {expr} => {constant}", "Optimize");

                return constant;
            }
            case BadBinaryExpression binExpr:
                binExpr.SetLeft(Substitute(scope, binExpr.Left));
                binExpr.SetRight(Substitute(scope, binExpr.Right));

                break;
        }

        return expr;
    }

    /// <summary>
    ///     Substitutes all variables in the expressions with their constant value.
    /// </summary>
    /// <param name="scope">The scope to use</param>
    /// <param name="expressions">The expressions to substitute</param>
    /// <returns>The substituted expression</returns>
    private static IEnumerable<BadExpression> Optimize(BadConstantSubstitutionOptimizerScope scope,
                                                       IEnumerable<BadExpression> expressions)
    {
        foreach (BadExpression expr in expressions)
        {
            switch (expr)
            {
                case BadClassPrototypeExpression proto:
                {
                    BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                    //Evaluate static body
                    BadExpression[] newStaticBody = Optimize(childScope, proto.StaticBody)
                        .ToArray();

                    proto.SetStaticBody(newStaticBody);

                    BadConstantSubstitutionOptimizerScope instanceScope = childScope.CreateChildScope();

                    //Evaluate instance body
                    BadExpression[] newBody = Optimize(instanceScope, proto.Body)
                        .ToArray();
                    proto.SetBody(newBody);

                    yield return proto;

                    continue;
                }
                case BadWhileExpression whileExpr:
                {
                    BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                    BadExpression[] newBody = Optimize(childScope, whileExpr.Body)
                        .ToArray();
                    whileExpr.SetBody(newBody);

                    yield return whileExpr;

                    continue;
                }
                case BadForExpression forExpr:
                {
                    BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                    BadExpression[] newBody = Optimize(childScope, forExpr.Body)
                        .ToArray();
                    forExpr.SetBody(newBody);

                    yield return forExpr;

                    continue;
                }
                case BadForEachExpression forEachExpr:
                {
                    BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                    BadExpression[] newBody = Optimize(childScope, forEachExpr.Body)
                        .ToArray();
                    forEachExpr.SetBody(newBody);

                    yield return forEachExpr;

                    continue;
                }
                case BadIfExpression ifExpr:
                {
                    foreach (KeyValuePair<BadExpression, BadExpression[]> branch in
                             ifExpr.ConditionalBranches.ToArray())
                    {
                        BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                        BadExpression[] newBody = Optimize(childScope, branch.Value)
                            .ToArray();
                        ifExpr.ConditionalBranches[branch.Key] = newBody;
                    }

                    if (ifExpr.ElseBranch != null)
                    {
                        BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                        BadExpression[] newBody = Optimize(childScope, ifExpr.ElseBranch)
                            .ToArray();
                        ifExpr.SetElseBranch(newBody);
                    }

                    yield return ifExpr;

                    continue;
                }
                case BadSwitchExpression switchExpr:
                {
                    foreach (KeyValuePair<BadExpression, BadExpression[]> branch in switchExpr.Cases.ToArray())
                    {
                        BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                        BadExpression[] newBody = Optimize(childScope, branch.Value)
                            .ToArray();
                        switchExpr.Cases[branch.Key] = newBody;
                    }

                    if (switchExpr.DefaultCase != null)
                    {
                        BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                        BadExpression[] newBody = Optimize(childScope, switchExpr.DefaultCase)
                            .ToArray();
                        switchExpr.SetDefaultCase(newBody);
                    }

                    yield return switchExpr;

                    continue;
                }
                case BadInvocationExpression invoc:
                {
                    List<BadExpression> args = (from arg in invoc.Arguments
                                                let childScope = scope.CreateChildScope()
                                                select Optimize(childScope,
                                                                new[] { arg }
                                                               )
                                                    .ToArray()
                                                into newBody
                                                select newBody[0]).ToList();

                    invoc.SetArgs(args);

                    yield return invoc;

                    continue;
                }
                case BadFunctionExpression func:
                {
                    BadConstantSubstitutionOptimizerScope childScope = scope.CreateChildScope();

                    BadExpression[] newBody = Optimize(childScope, func.Body)
                        .ToArray();
                    func.SetBody(newBody);

                    yield return func;

                    continue;
                }
                case BadBinaryExpression binExpr:
                {
                    bool canBeOptimized = OnlyContainsConstantsAndConstantVariables(scope, binExpr);

                    if (canBeOptimized)
                    {
                        BadLogger.Log($"Optimizing Expression: '{expr}' with Constant Substitution", "Optimize");

                        BadObject obj = Substitute(scope, binExpr)
                                        .Execute(null!)
                                        .Last();

                        BadLogger.Log($"Optimized Expression: '{expr}' => '{obj}' using Constant Substitution",
                                      "Optimize"
                                     );

                        yield return new BadConstantExpression(binExpr.Position, obj);

                        continue;
                    }

                    break;
                }
                case BadReturnExpression { Right: not null } rExpr:
                {
                    bool canBeOptimized = OnlyContainsConstantsAndConstantVariables(scope, rExpr.Right);

                    if (canBeOptimized && rExpr.Right is not IBadNativeExpression)
                    {
                        BadLogger.Log($"Optimizing Expression: '{expr}' with Constant Substitution", "Optimize");

                        BadObject obj = Substitute(scope, rExpr.Right)
                                        .Execute(null!)
                                        .Last();

                        BadLogger.Log($"Optimized Expression: '{expr}' => '{obj}' using Constant Substitution",
                                      "Optimize"
                                     );
                        rExpr.SetRight(new BadConstantExpression(rExpr.Position, obj));
                    }

                    break;
                }
                case BadAssignExpression { Left: BadVariableDefinitionExpression vDef } vAssign:
                {
                    bool canBeOptimized = OnlyContainsConstantsAndConstantVariables(scope, vAssign.Right);

                    if (canBeOptimized && vAssign.Right is not IBadNativeExpression)
                    {
                        BadLogger.Log($"Optimizing Expression: '{expr}' with Constant Substitution", "Optimize");

                        BadObject obj = Substitute(scope, vAssign.Right)
                                        .Execute(null!)
                                        .Last();

                        BadLogger.Log($"Optimized Expression: '{expr}' => '{obj}' using Constant Substitution",
                                      "Optimize"
                                     );
                        vAssign.Right = new BadConstantExpression(vAssign.Position, obj);
                    }

                    if (vDef.IsReadOnly && canBeOptimized)
                    {
                        scope.AddConstant(vDef.Name, vAssign.Right);
                    }

                    break;
                }
            }

            yield return expr;
        }
    }

    /// <summary>
    ///     Runs the Constant Substitution Optimization on the given expressions
    /// </summary>
    /// <param name="expressions">The expressions to optimize</param>
    /// <returns>The optimized expressions</returns>
    public static IEnumerable<BadExpression> Optimize(IEnumerable<BadExpression> expressions)
    {
        return Optimize(new BadConstantSubstitutionOptimizerScope(), expressions);
    }
}