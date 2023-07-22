using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Tests;

public class BadInteropTests
{
    [Test]
    public void Unwrap()
    {
        BadObject num = 10;
        BadObject table = new BadTable();
        Assert.That(num.CanUnwrap(), Is.True);
        Assert.That(table.CanUnwrap(), Is.False);
        Assert.That(num.Unwrap(), Is.EqualTo((decimal)10));
        Assert.That(() => table.Unwrap(), Throws.InstanceOf<BadRuntimeException>());
    }

    [Test]
    public void UnwrapGeneric()
    {
        BadObject num = 10;
        BadObject table = new BadTable();
        Version v = new Version();
        BadObject nativeTest = new BadNative<Version>(v);
        Assert.That(num.Unwrap<decimal>(), Is.EqualTo(10));
        Assert.That(() => table.Unwrap<decimal>(), Throws.InstanceOf<BadRuntimeException>());
        Assert.That(table.Unwrap<BadTable>(), Is.EqualTo(table));
        Assert.That(nativeTest.Unwrap<Version>(), Is.EqualTo(v));
    }

    [Test]
    public void GetExtensions()
    {
        BadInteropExtension.RegisterGlobal("Test", BadObject.Null);
        Assert.That(BadInteropExtension.GetExtensionNames(), Contains.Item((BadObject)"Test"));
        BadInteropExtension.RegisterObject<decimal>("Test1", o => BadObject.Null);
        Assert.That(BadInteropExtension.GetExtensionNames(10), Contains.Item((BadObject)"Test1"));
    }

    [Test]
    public void Enumerable()
    {
        BadInteropEnumerable e =
            new BadInteropEnumerable(System.Linq.Enumerable.Range(0, 10).Select(x => (BadObject)x));
        Assert.That(e.GetPrototype(), Is.InstanceOf<BadClassPrototype>());
        Assert.That(e.HasProperty("GetEnumerator"), Is.True);
        BadObject getEnumerator = e.GetProperty("GetEnumerator").Dereference();
        Assert.That(getEnumerator, Is.InstanceOf<BadDynamicInteropFunction>());
        BadObject enumerator = ((BadDynamicInteropFunction)getEnumerator)
            .Invoke(Array.Empty<BadObject>(), BadExecutionContext.Create())
            .Last();
        Assert.That(enumerator, Is.InstanceOf<BadInteropEnumerator>());
    }

    [Test]
    public void Reflection()
    {
        Version v = new Version();
        BadReflectedObject obj = new BadReflectedObject(v);
        Assert.That(obj.GetPrototype(), Is.InstanceOf<BadClassPrototype>());
        Assert.That(obj.HasProperty("Major"), Is.True);
        Assert.That(obj.GetProperty("Major").Dereference(), Is.EqualTo((BadObject)0));
        BadFunction toString = (BadFunction)obj.GetProperty("ToString").Dereference();
        Assert.That(
            toString.Invoke(Array.Empty<BadObject>(), BadExecutionContext.Create()).Last(),
            Is.EqualTo((BadObject)"0.0")
        );
    }
}