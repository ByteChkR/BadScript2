using BadScript2.Common;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements Array Extensions
/// </summary>
public class BadArrayExtension : BadInteropExtension
{
	protected override void AddExtensions(BadInteropExtensionProvider provider)
	{
		provider.RegisterObject<BadArray>("Add",
			a => new BadDynamicInteropFunction<BadObject>("Add",
				(_, o) => Add(a, o),
				"elem"));

		provider.RegisterObject<BadArray>("Clear",
			a => new BadDynamicInteropFunction("Clear",
				_ => Clear(a)));

		provider.RegisterObject<BadArray>("AddRange",
			a => new BadDynamicInteropFunction<BadArray>("AddRange",
				(_, elems) =>
				{
					a.InnerArray.AddRange(elems.InnerArray);

					return BadObject.Null;
				},
				"elems"));

		provider.RegisterObject<BadArray>("Insert",
			a => new BadDynamicInteropFunction<decimal, BadObject>("Insert",
				(_, i, o) => Insert(a, i, o),
				"elem"));

		provider.RegisterObject<BadArray>("InsertRange",
			a => new BadDynamicInteropFunction<decimal, BadArray>("InsertRange",
				(_, num, elems) =>
				{
					a.InnerArray.InsertRange((int)num, elems.InnerArray);

					return BadObject.Null;
				},
				"index",
				"elems"));

		provider.RegisterObject<BadArray>("Remove",
			a => new BadDynamicInteropFunction<BadObject>("Remove",
				(_, o) => Remove(a, o),
				"elem"));

		provider.RegisterObject<BadArray>("Contains",
			a => new BadDynamicInteropFunction<BadObject>("Contains",
				(_, o) => Contains(a, o),
				"elem"));

		provider.RegisterObject<BadArray>("RemoveAt",
			a => new BadDynamicInteropFunction<decimal>("RemoveAt",
				(_, o) => RemoveAt(a, o),
				"index"));

		provider.RegisterObject<BadArray>("Get",
			a => new BadDynamicInteropFunction<decimal>("Get",
				(_, o) => Get(a, o),
				"index"));

		provider.RegisterObject<BadArray>("Set",
			a => new BadDynamicInteropFunction<decimal, BadObject>("Set",
				(_, i, v) => Set(a, i, v),
				"index"));

		provider.RegisterObject<BadArray>("GetEnumerator",
			a => new BadDynamicInteropFunction("GetEnumerator",
				_ => GetEnumerator(a)));

		provider.RegisterObject<BadArray>(BadStaticKeys.ArrayAccessOperatorName,
			a => new BadDynamicInteropFunction<decimal>(BadStaticKeys.ArrayAccessOperatorName,
				(_, i) => BadObjectReference.Make($"{a}[{i}]", () => Get(a, i), (v, _) => Set(a, i, v)),
				"index"));
		provider.RegisterObject<BadArray>(BadStaticKeys.ArrayAccessReverseOperatorName,
			a => new BadDynamicInteropFunction<decimal>(BadStaticKeys.ArrayAccessReverseOperatorName,
				(_, i) => BadObjectReference.Make($"{a}[^{i}]",
					() => Get(a, a.InnerArray.Count - i),
					(v, _) => Set(a, a.InnerArray.Count - i, v)),
				"index"));

		provider.RegisterObject<BadArray>("Length", a => BadObject.Wrap((decimal)a.InnerArray.Count));
	}

    /// <summary>
    ///     Returns an enumerator of the array
    /// </summary>
    /// <param name="array">The Array</param>
    /// <returns>Enumerator</returns>
    private BadObject GetEnumerator(BadArray array)
	{
		return new BadInteropEnumerator(array.InnerArray.GetEnumerator());
	}

    /// <summary>
    ///     Clears the array
    /// </summary>
    /// <param name="arg">The Array</param>
    /// <returns>NULL</returns>
    private BadObject Clear(BadArray arg)
	{
		arg.InnerArray.Clear();

		return BadObject.Null;
	}

    /// <summary>
    ///     Adds an element to the array
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Element</param>
    /// <returns>NULL</returns>
    private BadObject Add(BadArray arg, BadObject obj)
	{
		arg.InnerArray.Add(obj);

		return BadObject.Null;
	}

    /// <summary>
    ///     Inserts an element at the given index
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="index">Index</param>
    /// <param name="obj">Element</param>
    /// <returns>NULL</returns>
    private BadObject Insert(BadArray arg, decimal index, BadObject obj)
	{
		arg.InnerArray.Insert((int)index, obj);

		return BadObject.Null;
	}

    /// <summary>
    ///     Returns True if the array contains the given element
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Element</param>
    /// <returns>Boolean</returns>
    private BadObject Contains(BadArray arg, BadObject obj)
	{
		return arg.InnerArray.Contains(obj);
	}

    /// <summary>
    ///     Removes an element from the array
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Element</param>
    /// <returns>NULL</returns>
    private BadObject Remove(BadArray arg, BadObject obj)
	{
		arg.InnerArray.Remove(obj);

		return BadObject.Null;
	}

    /// <summary>
    ///     Removes an element at the given index
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Index</param>
    /// <returns>NULL</returns>
    private BadObject RemoveAt(BadArray arg, decimal obj)
	{
		int index = (int)obj;
		arg.InnerArray.RemoveAt(index);

		return BadObject.Null;
	}

    /// <summary>
    ///     Returns a value from the array
    /// </summary>
    /// <param name="arg">Array</param>
    /// <param name="obj">Index</param>
    /// <returns>Element at Index</returns>
    private BadObject Get(BadArray arg, decimal obj)
	{
		int index = (int)obj;

		return arg.InnerArray[index];
	}

    /// <summary>
    ///     Sets a value in the array
    /// </summary>
    /// <param name="arg">The Array</param>
    /// <param name="obj">The Index</param>
    /// <param name="value">The value</param>
    /// <returns>NULL</returns>
    private BadObject Set(BadArray arg, decimal obj, BadObject value)
	{
		int index = (int)obj;
		arg.InnerArray[index] = value;

		return BadObject.Null;
	}
}
