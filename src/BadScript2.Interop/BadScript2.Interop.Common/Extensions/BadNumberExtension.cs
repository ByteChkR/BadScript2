using System.Globalization;

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

    private static BadObject NumberToString(decimal d, IReadOnlyList<BadObject> args)
    {
        if (args.Count == 0)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }

        IBadString format = (IBadString)args[0]; //Type is checked in the function builder

        if (args.Count == 1)
        {
            return d.ToString(format.Value);
        }

        if (args.Count != 2)
        {
            throw new BadRuntimeException("Invalid number of arguments");
        }

        IBadString culture = (IBadString)args[1]; //Type is checked in the function builder

        if (s_Cultures.TryGetValue(
                culture.Value,
                out CultureInfo? cultureInfo
            )) //Cache Culture info to avoid creating it every time
        {
            return d.ToString(format.Value, cultureInfo);
        }

        cultureInfo = CultureInfo.CreateSpecificCulture(culture.Value);
        s_Cultures.Add(culture.Value, cultureInfo);

        return d.ToString(format.Value, cultureInfo);
    }

    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        provider.RegisterObject<decimal>(
            "ToString",
            d => new BadInteropFunction(
                "ToString",
                a => NumberToString( d, a),
                false,
                BadNativeClassBuilder.GetNative("string"),
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