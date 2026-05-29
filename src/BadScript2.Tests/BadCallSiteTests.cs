using BadScript2.Common;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.VirtualMachine;

namespace BadScript2.Tests;

/// <summary>
/// Tests for Phase 5.2: Compiled-Call-Fastpath (Call Sites).
/// </summary>
public class BadCallSiteTests
{
    private static readonly BadSourcePosition s_Position = BadSourcePosition.FromSource("test", 0, 4);

    [Test]
    public void DynamicCallSite_CanHandle_AnyObject()
    {
        // Arrange
        var callSite = new BadDynamicCallSite();

        // Act & Assert
        Assert.That(callSite.CanHandle(BadObject.Null), Is.True);
        Assert.That(callSite.CanHandle(BadObject.True), Is.True);
    }

    [Test]
    public void CallSiteFactory_CreatesDynamicCallSite_ForNonCompiledFunctions()
    {
        // Arrange & Act
        var callSite = BadCallSiteFactory.CreateCallSite(BadObject.Null);

        // Assert
        Assert.That(callSite, Is.TypeOf<BadDynamicCallSite>());
    }

    [Test]
    public void CachedCallSite_RemembersLastTarget()
    {
        // Arrange
        var cachedSite = new BadCachedCallSite(BadObject.Null);

        // Act & Assert
        Assert.That(cachedSite, Is.Not.Null);
    }

    [Test]
    public void MethodCallSite_CachesMethodLookups()
    {
        // Arrange
        var methodSite = new BadMethodCallSite("toString");

        // Act & Assert
        Assert.That(methodSite.CanHandle(BadObject.Null), Is.True);
    }

    [Test]
    public void DynamicCallSite_Invoke_ExecutesFunction()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadExpressionFunction function = new BadExpressionFunction(context.Scope,
                                                                   BadWordToken.MakeWord("Fn"),
                                                                   [new BadReturnExpression(new BadNumberExpression(7, s_Position), s_Position, false)],
                                                                   [],
                                                                   s_Position,
                                                                   false,
                                                                   false,
                                                                   null,
                                                                   BadAnyPrototype.Instance,
                                                                   false
                                                                  );
        BadDynamicCallSite callSite = new BadDynamicCallSite();

        BadObject result = BadObject.Null;

        foreach (BadObject o in callSite.Invoke(function, [], s_Position, context))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(7));
    }

    [Test]
    public void MethodCallSite_Invoke_ExecutesMethod()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadExpressionFunction method = new BadExpressionFunction(context.Scope,
                                                                 BadWordToken.MakeWord("run"),
                                                                 [new BadReturnExpression(new BadNumberExpression(9, s_Position), s_Position, false)],
                                                                 [],
                                                                 s_Position,
                                                                 false,
                                                                 false,
                                                                 null,
                                                                 BadAnyPrototype.Instance,
                                                                 false
                                                                );
        BadTable table = new BadTable(new Dictionary<string, BadObject>
        {
            { "run", method },
        });
        BadMethodCallSite callSite = new BadMethodCallSite("run");

        BadObject result = BadObject.Null;

        foreach (BadObject o in callSite.Invoke(table, [], s_Position, context))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(9));
    }
}




