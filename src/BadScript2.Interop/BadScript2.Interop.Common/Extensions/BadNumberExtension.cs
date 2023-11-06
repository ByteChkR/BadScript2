using System.Globalization;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Extensions;

public class BadNumberExtension : BadInteropExtension
{
    private static readonly Dictionary<string, CultureInfo> s_Cultures = new Dictionary<string, CultureInfo>();

    private static BadObject NumberToString(BadExecutionContext ctx, decimal d, BadObject[] args)
    {
        if (args.Length == 0)
        {
            return d.ToString();
        }

        IBadString format = (IBadString)args[0]; //Type is checked in the function builder

        if (args.Length == 1)
        {
            return d.ToString(format.Value);
        }

        if (args.Length != 2)
        {
            throw new BadRuntimeException("Invalid number of arguments");
        }

        IBadString culture = (IBadString)args[1]; //Type is checked in the function builder

        if (!s_Cultures.TryGetValue(
                culture.Value,
                out CultureInfo? cultureInfo
            )) //Cache Culture info to avoid creating it every time
        {
            cultureInfo = CultureInfo.CreateSpecificCulture(culture.Value);
            s_Cultures.Add(culture.Value, cultureInfo);
        }

        return d.ToString(format.Value, cultureInfo);
    }

    protected override void AddExtensions()
    {
        RegisterObject<decimal>(
            "ToString",
            d => new BadInteropFunction(
                "ToString",
                (c, a) => NumberToString(c, d, a),
                false,
                new BadFunctionParameter("format", true, true, false, null, BadNativeClassBuilder.GetNative("string")),
                new BadFunctionParameter(
                    "culture",
                    true,
                    true,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                )
            )
        );
    }
}