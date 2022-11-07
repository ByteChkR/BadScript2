using System.Collections;

using BadScript2.Runtime.Objects;
using BadScript2.Utility.Linq;

namespace BadScript2.Tests;

public class BadLinqTests
{
    [Test]
    public void SelectTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        foreach (object? o in BadLinqExtensions.Select(d, "x=>x*2"))
        {
            Assert.That(o, Is.InstanceOf<decimal>());
            Assert.That((decimal)o % 2 == 0, Is.True);
        }
    }

    [Test]
    public void WhereTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        foreach (object? o in BadLinqExtensions.Where(d, "x=>x%2==0"))
        {
            Assert.That(o, Is.InstanceOf<int>());
            Assert.That((int)o % 2 == 0, Is.True);
        }
    }

    [Test]
    public void LastTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object o = BadLinqExtensions.Last(d, "x=>x%2==0");
        Assert.That(o, Is.InstanceOf<int>());
        Assert.That(o, Is.EqualTo(98));
    }

    [Test]
    public void FirstTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object o = BadLinqExtensions.First(d, "x=>x%2==0");
        Assert.That(o, Is.InstanceOf<int>());
        Assert.That(o, Is.EqualTo(0));
    }

    [Test]
    public void LastOrDefaultTest()
    {
        IEnumerable d = Enumerable.Empty<BadObject>();
        object? o = BadLinqExtensions.LastOrDefault(d, "x=>x%2==0");
        Assert.That(o, Is.Null);
    }

    [Test]
    public void FirstOrDefaultTest()
    {
        IEnumerable d = Enumerable.Empty<BadObject>();
        object? o = BadLinqExtensions.FirstOrDefault(d, "x=>x%2==0");

        Assert.That(o, Is.Null);
    }

    [Test]
    public void TakeTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = BadLinqExtensions.Take(d, 10).Cast<object>().ToArray();
        Assert.That(o.Length, Is.EqualTo(10));
    }

    [Test]
    public void SkipTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = BadLinqExtensions.Skip(d, 10).Cast<object>().ToArray();
        Assert.That(o.Length, Is.EqualTo(90));
    }

    [Test]
    public void TakeLastTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = BadLinqExtensions.TakeLast(d, 10).Cast<object>().ToArray();
        Assert.That(o.Length, Is.EqualTo(10));
    }

    [Test]
    public void SkipLastTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = BadLinqExtensions.SkipLast(d, 10).Cast<object>().ToArray();
        Assert.That(o.Length, Is.EqualTo(90));
    }


    [Test]
    public void SelectManyTest()
    {
        List<BadObject> a = new BadObject[] { 1, 2 }.ToList();
        IEnumerable d = new BadObject[] { new BadArray(a), new BadArray(a) };
        object[] o = BadLinqExtensions.SelectMany(d, "x=>x").Cast<object>().ToArray();
        Assert.That(o.Length, Is.EqualTo(4));
    }

    [Test]
    public void SkipWhileTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = BadLinqExtensions.SkipWhile(d, "x=>x < 10").Cast<object>().ToArray();
        Assert.That(o.Length, Is.EqualTo(90));
    }

    [Test]
    public void TakeWhileTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = BadLinqExtensions.TakeWhile(d, "x=>x < 10").Cast<object>().ToArray();
        Assert.That(o.Length, Is.EqualTo(10));
    }

    [Test]
    public void AllTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        bool o = BadLinqExtensions.All(d, "x=>x < 10");
        bool o1 = BadLinqExtensions.All(d, "x=>x < 100");
        Assert.That(o, Is.False);
        Assert.That(o1, Is.True);
    }

    [Test]
    public void AnyTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        bool o = BadLinqExtensions.Any(d, "x=>x < 0");
        bool o1 = BadLinqExtensions.Any(d, "x=>x < 100");
        Assert.That(o, Is.False);
        Assert.That(o1, Is.True);
    }
}