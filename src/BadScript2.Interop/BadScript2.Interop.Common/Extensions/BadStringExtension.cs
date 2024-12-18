using BadScript2.Common;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

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
    private static BadObject StringSplit(string str, BadObject splitChar, BadObject skipEmpty)
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

        return new BadArray(str.Split(new[] { splitStr.Value },
                                      skip ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None
                                     )
                               .Select(x => (BadObject)x)
                               .ToList()
                           );
    }


    /// <inheritdoc />
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<string>("ToLower",
                                        o => new BadDynamicInteropFunction("ToLower",
                                                                           _ => o.ToLower(),
                                                                           BadNativeClassBuilder.GetNative("string")
                                                                          )
                                       );

        provider.RegisterObject<string>("ToUpper",
                                        o => new BadDynamicInteropFunction("ToUpper",
                                                                           _ => o.ToUpper(),
                                                                           BadNativeClassBuilder.GetNative("string")
                                                                          )
                                       );

        provider.RegisterObject<string>("IsLetters", s => s.All(char.IsLetter));
        provider.RegisterObject<string>("IsDigits", s => s.All(char.IsDigit));
        provider.RegisterObject<string>("IsWhiteSpace", s => s.All(char.IsWhiteSpace));

        provider.RegisterObject<string>(BadStaticKeys.ARRAY_ACCESS_OPERATOR_NAME,
                                        s => new BadDynamicInteropFunction<decimal>(BadStaticKeys
                                                 .ARRAY_ACCESS_OPERATOR_NAME,
                                             (_, i) => s[(int)i]
                                                 .ToString(),
                                             BadNativeClassBuilder.GetNative("string"),
                                             "index"
                                            )
                                       );
        provider.RegisterObject<string>("Length", a => BadObject.Wrap((decimal)a.Length));

        provider.RegisterObject<string>("Format",
                                        s => new BadInteropFunction("Format",
                                                                    args => string.Format(s,
                                                                         args.Cast<object?>()
                                                                             .ToArray()
                                                                        ),
                                                                    false,
                                                                    BadNativeClassBuilder.GetNative("string"),
                                                                    new BadFunctionParameter("args", false, false, true)
                                                                   )
                                       );

        provider.RegisterObject<string>("Split",
                                        s => new BadInteropFunction("Split",
                                                                    args => StringSplit(s,
                                                                         args[0],
                                                                         args.Length == 2 ? args[1] : BadObject.Null
                                                                        ),
                                                                    false,
                                                                    BadNativeClassBuilder.GetNative("Array"),
                                                                    "splitStr",
                                                                    new BadFunctionParameter("skipEmpty",
                                                                         true,
                                                                         false,
                                                                         false
                                                                        )
                                                                   )
                                       );

        //Substring
        provider.RegisterObject<string>("Substring",
                                        s => new BadDynamicInteropFunction<decimal, decimal>("Substring",
                                             (_, start, end) => s.Substring((int)start, (int)end),
                                             BadNativeClassBuilder.GetNative("string"),
                                             "start",
                                             "end"
                                            )
                                       );

        //IndexOf
        provider.RegisterObject<string>("IndexOf",
                                        s => new BadDynamicInteropFunction<string>("IndexOf",
                                             (_, str) => (decimal)s.IndexOf(str, StringComparison.Ordinal),
                                             BadNativeClassBuilder.GetNative("num")
                                            )
                                       );

        provider.RegisterObject<string>("Contains",
                                        s => new BadDynamicInteropFunction<string>("Contains",
                                             (_, str) => s.Contains(str),
                                             BadNativeClassBuilder.GetNative("bool")
                                            )
                                       );

        //LastIndexOf
        provider.RegisterObject<string>("LastIndexOf",
                                        s => new BadDynamicInteropFunction<string>("LastIndexOf",
                                             (_, str) => (decimal)s.LastIndexOf(str, StringComparison.Ordinal),
                                             BadNativeClassBuilder.GetNative("num")
                                            )
                                       );

        //Replace
        provider.RegisterObject<string>("Replace",
                                        s => new BadDynamicInteropFunction<string, string>("Replace",
                                             (_, oldStr, newStr) => s.Replace(oldStr, newStr),
                                             BadNativeClassBuilder.GetNative("string")
                                            )
                                       );

        //Trim
        provider.RegisterObject<string>("Trim",
                                        s => new BadDynamicInteropFunction("Trim",
                                                                           _ => s.Trim(),
                                                                           BadNativeClassBuilder.GetNative("string")
                                                                          )
                                       );

        //TrimStart
        provider.RegisterObject<string>("TrimStart",
                                        s => new BadDynamicInteropFunction("TrimStart",
                                                                           _ => s.TrimStart(),
                                                                           BadNativeClassBuilder.GetNative("string")
                                                                          )
                                       );

        //TrimEnd
        provider.RegisterObject<string>("TrimEnd",
                                        s => new BadDynamicInteropFunction("TrimEnd",
                                                                           _ => s.TrimEnd(),
                                                                           BadNativeClassBuilder.GetNative("string")
                                                                          )
                                       );

        //PadLeft
        provider.RegisterObject<string>("PadLeft",
                                        s => new BadDynamicInteropFunction<decimal>("PadLeft",
                                             (_, padding) => s.PadLeft((int)padding),
                                             BadNativeClassBuilder.GetNative("string"),
                                             "padding"
                                            )
                                       );

        //PadRight
        provider.RegisterObject<string>("PadRight",
                                        s => new BadDynamicInteropFunction<decimal>("PadRight",
                                             (_, padding) => s.PadRight((int)padding),
                                             BadNativeClassBuilder.GetNative("string"),
                                             "padding"
                                            )
                                       );

        //Remove
        provider.RegisterObject<string>("Remove",
                                        s => new BadDynamicInteropFunction<decimal, decimal>("Remove",
                                             (_, start, count) => s.Remove((int)start, (int)count),
                                             BadNativeClassBuilder.GetNative("string"),
                                             "start",
                                             "count"
                                            )
                                       );

        //Insert
        provider.RegisterObject<string>("Insert",
                                        s => new BadDynamicInteropFunction<decimal, string>("Insert",
                                             (_, index, str) => s.Insert((int)index, str),
                                             BadNativeClassBuilder.GetNative("string"),
                                             "index",
                                             "str"
                                            )
                                       );

        //EndsWith
        provider.RegisterObject<string>("EndsWith",
                                        s => new BadDynamicInteropFunction<string>("EndsWith",
                                             (_, str) => s.EndsWith(str, StringComparison.Ordinal),
                                             BadNativeClassBuilder.GetNative("bool")
                                            )
                                       );

        //StartsWith
        provider.RegisterObject<string>("StartsWith",
                                        s => new BadDynamicInteropFunction<string>("StartsWith",
                                             (_, str) => s.StartsWith(str, StringComparison.Ordinal),
                                             BadNativeClassBuilder.GetNative("bool")
                                            )
                                       );
    }
}