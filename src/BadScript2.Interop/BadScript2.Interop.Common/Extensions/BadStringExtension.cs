using BadScript2.Common;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Common.Extensions;

/// <summary>
///     Implements String Extensions
/// </summary>
public class BadStringExtension : BadInteropExtension
{
    /// <summary>
    ///     Split a string into an array
    /// </summary>
    /// <param name="str">String to Split</param>
    /// <param name="splitChar">The Split Character</param>
    /// <param name="skipEmpty">If true, empty parts will be skipped</param>
    /// <returns>Array of parts</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the arguments are invalid</exception>
    private BadObject StringSplit(string str, BadObject splitChar, BadObject skipEmpty)
	{
		if (splitChar is not IBadString splitStr)
		{
			throw new BadRuntimeException("splitChar must be a string");
		}

		bool skip = false;

		if (skipEmpty is not IBadBoolean skipB)
		{
			if (skipEmpty != BadObject.Null)
			{
				throw new BadRuntimeException("skipEmpty must be a boolean");
			}
		}
		else
		{
			skip = skipB.Value;
		}

		return new BadArray(str.Split(new[]
				{
					splitStr.Value
				},
				skip ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None)
			.Select(x => (BadObject)x)
			.ToList());
	}

	protected override void AddExtensions()
	{
		RegisterObject<string>("ToLower",
			o => new BadDynamicInteropFunction("ToLower",
				_ => o.ToLower()));
		RegisterObject<string>("ToUpper",
			o => new BadDynamicInteropFunction("ToUpper",
				_ => o.ToUpper()));

		RegisterObject<string>("IsLetters", s => s.All(char.IsLetter));
		RegisterObject<string>("IsDigits", s => s.All(char.IsDigit));
		RegisterObject<string>("IsWhiteSpace", s => s.All(char.IsWhiteSpace));

		RegisterObject<string>(BadStaticKeys.ArrayAccessOperatorName,
			s => new BadDynamicInteropFunction<decimal>(BadStaticKeys.ArrayAccessOperatorName,
				(_, i) => s[(int)i].ToString(),
				"index"));
		RegisterObject<string>("Length", a => BadObject.Wrap((decimal)a.Length));
		RegisterObject<string>("Format",
			s => new BadInteropFunction("Format",
				args => string.Format(s, args.Cast<object?>().ToArray()),
				false,
				new BadFunctionParameter("args", false, false, true)));

		RegisterObject<string>("Split",
			s => new BadInteropFunction("Split",
				args => StringSplit(s, args[0], args.Length == 2 ? args[1] : BadObject.Null),
				false,
				"splitStr",
				new BadFunctionParameter("skipEmpty", true, false, false)));

		//Substring
		RegisterObject<string>("Substring",
			s => new BadDynamicInteropFunction<decimal, decimal>("Substring",
				(_, start, end) => s.Substring((int)start, (int)end),
				"start",
				"end"));

		//IndexOf
		RegisterObject<string>("IndexOf",
			s => new BadDynamicInteropFunction<string>("IndexOf",
				(_, str) => (decimal)s.IndexOf(str, StringComparison.Ordinal)));

		RegisterObject<string>("Contains",
			s => new BadDynamicInteropFunction<string>("Contains",
				(_, str) => s.Contains(str)));

		//LastIndexOf
		RegisterObject<string>("LastIndexOf",
			s => new BadDynamicInteropFunction<string>("LastIndexOf",
				(_, str) => (decimal)s.LastIndexOf(str, StringComparison.Ordinal)));

		//Replace
		RegisterObject<string>("Replace",
			s => new BadDynamicInteropFunction<string, string>("Replace",
				(_, oldStr, newStr) => s.Replace(oldStr, newStr)));

		//Trim
		RegisterObject<string>("Trim",
			s => new BadDynamicInteropFunction("Trim",
				_ => s.Trim()));

		//TrimStart
		RegisterObject<string>("TrimStart",
			s => new BadDynamicInteropFunction("TrimStart",
				_ => s.TrimStart()));

		//TrimEnd
		RegisterObject<string>("TrimEnd",
			s => new BadDynamicInteropFunction("TrimEnd",
				_ => s.TrimEnd()));

		//PadLeft
		RegisterObject<string>("PadLeft",
			s => new BadDynamicInteropFunction<decimal>("PadLeft",
				(_, padding) => s.PadLeft((int)padding),
				"padding"));

		//PadRight
		RegisterObject<string>("PadRight",
			s => new BadDynamicInteropFunction<decimal>("PadRight",
				(_, padding) => s.PadRight((int)padding),
				"padding"));

		//Remove
		RegisterObject<string>("Remove",
			s => new BadDynamicInteropFunction<decimal, decimal>("Remove",
				(_, start, count) => s.Remove((int)start, (int)count),
				"start",
				"count"));

		//Insert
		RegisterObject<string>("Insert",
			s => new BadDynamicInteropFunction<decimal, string>("Insert",
				(_, index, str) => s.Insert((int)index, str),
				"index",
				"str"));

		//EndsWith
		RegisterObject<string>("EndsWith",
			s => new BadDynamicInteropFunction<string>("EndsWith",
				(_, str) => s.EndsWith(str, StringComparison.Ordinal)));

		//StartsWith
		RegisterObject<string>("StartsWith",
			s => new BadDynamicInteropFunction<string>("StartsWith",
				(_, str) => s.StartsWith(str, StringComparison.Ordinal)));
	}
}
