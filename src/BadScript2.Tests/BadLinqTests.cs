using System.Collections;

using BadScript2.Runtime.Objects;
using BadScript2.Utility.Linq;

namespace BadScript2.Tests;

public class BadLinqTests
{
    /// <summary>
    ///     Tests the LINQ.Select Function
    /// </summary>
    [Test]
    public void SelectTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);

        foreach (object? o in d.Select("x=>x*2"))
        {
            Assert.Multiple(
                () =>
                {
                    Assert.That(o, Is.InstanceOf<decimal>());
                    Assert.That((decimal)o % 2, Is.EqualTo(0));
                }
            );
        }
    }

    /// <summary>
    ///     Tests the LINQ.Where Function
    /// </summary>
    [Test]
    public void WhereTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);

        foreach (object? o in d.Where("x=>x%2==0"))
        {
            Assert.Multiple(
                () =>
                {
                    Assert.That(o, Is.InstanceOf<int>());
                    Assert.That((int)o % 2, Is.EqualTo(0));
                }
            );
        }
    }

    /// <summary>
    ///     Tests the LINQ.Last Function
    /// </summary>
    [Test]
    public void LastTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object o = d.Last("x=>x%2==0");
        Assert.That(o, Is.InstanceOf<int>());
        Assert.That(o, Is.EqualTo(98));
    }

    /// <summary>
    ///     Tests the Linq.First Function
    /// </summary>
    [Test]
    public void FirstTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object o = d.First("x=>x%2==0");
        Assert.That(o, Is.InstanceOf<int>());
        Assert.That(o, Is.EqualTo(0));
    }

    /// <summary>
    ///     Tests the Linq.LastOrDefault Function
    /// </summary>
    [Test]
    public void LastOrDefaultTest()
    {
        IEnumerable d = Enumerable.Empty<BadObject>();
        object? o = d.LastOrDefault("x=>x%2==0");
        Assert.That(o, Is.Null);
    }

    /// <summary>
    ///     Tests the Linq.FirstOrDefault Function
    /// </summary>
    [Test]
    public void FirstOrDefaultTest()
    {
        IEnumerable d = Enumerable.Empty<BadObject>();
        object? o = d.FirstOrDefault("x=>x%2==0");

        Assert.That(o, Is.Null);
    }

    /// <summary>
    ///     Tests the Linq.Take Function
    /// </summary>
    [Test]
    public void TakeTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = d.Take(10).Cast<object>().ToArray();
        Assert.That(o, Has.Length.EqualTo(10));
    }

    /// <summary>
    ///     Tests the Linq.Skip Function
    /// </summary>
    [Test]
    public void SkipTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = d.Skip(10).Cast<object>().ToArray();
        Assert.That(o, Has.Length.EqualTo(90));
    }

    /// <summary>
    ///     Tests the Linq.TakeLast Function
    /// </summary>
    [Test]
    public void TakeLastTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = d.TakeLast(10).Cast<object>().ToArray();
        Assert.That(o, Has.Length.EqualTo(10));
    }

    /// <summary>
    ///     Tests the Linq.SkipLast Function
    /// </summary>
    [Test]
    public void SkipLastTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = d.SkipLast(10).Cast<object>().ToArray();
        Assert.That(o, Has.Length.EqualTo(90));
    }


    /// <summary>
    ///     Tests the Linq.SelectMany Function
    /// </summary>
    [Test]
    public void SelectManyTest()
    {
        List<BadObject> a = new BadObject[]
        {
            1,
            2,
        }.ToList();
        IEnumerable d = new BadObject[]
        {
            new BadArray(a),
            new BadArray(a),
        };
        object[] o = d.SelectMany("x=>x").Cast<object>().ToArray();
        Assert.That(o, Has.Length.EqualTo(4));
    }

    /// <summary>
    ///     Tests the Linq.SkipWhile Function
    /// </summary>
    [Test]
    public void SkipWhileTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = d.SkipWhile("x=>x < 10").Cast<object>().ToArray();
        Assert.That(o, Has.Length.EqualTo(90));
    }

    /// <summary>
    ///     Test the Linq.TakeWhile Function
    /// </summary>
    [Test]
    public void TakeWhileTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);
        object[] o = d.TakeWhile("x=>x < 10").Cast<object>().ToArray();
        Assert.That(o, Has.Length.EqualTo(10));
    }

    /// <summary>
    ///     Tests the Linq.All Function
    /// </summary>
    [Test]
    public void AllTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);

        // ReSharper disable once PossibleMultipleEnumeration
        bool o = d.All("x=>x < 10");

        // ReSharper disable once PossibleMultipleEnumeration
        bool o1 = d.All("x=>x < 100");
        Assert.Multiple(
            () =>
            {
                Assert.That(o, Is.False);
                Assert.That(o1, Is.True);
            }
        );
    }

    /// <summary>
    ///     Tests the Linq.Any Function
    /// </summary>
    [Test]
    public void AnyTest()
    {
        IEnumerable d = Enumerable.Range(0, 100);

        // ReSharper disable once PossibleMultipleEnumeration
        bool o = d.Any("x=>x < 0");

        // ReSharper disable once PossibleMultipleEnumeration
        bool o1 = d.Any("x=>x < 100");
        Assert.That(o, Is.False);
        Assert.That(o1, Is.True);
    }
}