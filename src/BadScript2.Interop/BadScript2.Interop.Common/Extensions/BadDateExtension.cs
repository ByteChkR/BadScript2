using System.Globalization;
using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Extensions;

public class BadDateExtension : BadInteropExtension
{
    /// <summary>
    ///     Culture Info Cache
    /// </summary>
    private static readonly Dictionary<string, CultureInfo> s_Cultures = new Dictionary<string, CultureInfo>();
    /// <summary>
    /// Wrapper for DateTimeOffset.ToString
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="d">The DateTimeOffset to convert</param>
    /// <param name="args">The arguments to the function</param>
    /// <returns>The string representation of the DateTimeOffset</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the arguments are invalid</exception>
    private static BadObject DateToString(BadExecutionContext ctx, DateTimeOffset d, IReadOnlyList<BadObject> args)
    {
        if (args.Count == 0)
        {
            return d.ToString();
        }

        IBadString format = (IBadString)args[0]; //Type is checked in the function builder

        if (args.Count == 1)
        {
            CultureInfo? defaultCulture = ctx.Scope.GetSingleton<BadRuntime>()
                                              ?.Culture ??
                                          CultureInfo.InvariantCulture;

            return d.ToString(format.Value, defaultCulture);
        }

        if (args.Count != 2)
        {
            throw new BadRuntimeException("Invalid number of arguments");
        }

        IBadString culture = (IBadString)args[1]; //Type is checked in the function builder

        if (s_Cultures.TryGetValue(culture.Value,
                out CultureInfo? cultureInfo
            )) //Cache Culture info to avoid creating it every time
        {
            return d.ToString(format.Value, cultureInfo);
        }

        cultureInfo = CultureInfo.CreateSpecificCulture(culture.Value);
        s_Cultures.Add(culture.Value, cultureInfo);

        return d.ToString(format.Value, cultureInfo);
    }
    
    /// <summary>
    /// Wrapper for TimeSpan.ToString
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="d">The TimeSpan to convert</param>
    /// <param name="args">The arguments to the function</param>
    /// <returns>The string representation of the TimeSpan</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the arguments are invalid</exception>
    private static BadObject TimeToString(BadExecutionContext ctx, TimeSpan d, IReadOnlyList<BadObject> args)
    {
        if (args.Count == 0)
        {
            return d.ToString();
        }

        IBadString format = (IBadString)args[0]; //Type is checked in the function builder

        if (args.Count == 1)
        {
            CultureInfo? defaultCulture = ctx.Scope.GetSingleton<BadRuntime>()
                                              ?.Culture ??
                                          CultureInfo.InvariantCulture;

            return d.ToString(format.Value, defaultCulture);
        }

        if (args.Count != 2)
        {
            throw new BadRuntimeException("Invalid number of arguments");
        }

        IBadString culture = (IBadString)args[1]; //Type is checked in the function builder

        if (s_Cultures.TryGetValue(culture.Value,
                out CultureInfo? cultureInfo
            )) //Cache Culture info to avoid creating it every time
        {
            return d.ToString(format.Value, cultureInfo);
        }

        cultureInfo = CultureInfo.CreateSpecificCulture(culture.Value);
        s_Cultures.Add(culture.Value, cultureInfo);

        return d.ToString(format.Value, cultureInfo);
    }
    /// <inheritdoc />
    protected override void AddExtensions(BadInteropExtensionProvider provider)
    {
        #region Date
        provider.RegisterObject<DateTimeOffset>("ToString",
            d => new BadInteropFunction("ToString",
                (c, a) => DateToString(c, d, a),
                false,
                BadNativeClassBuilder.GetNative("string"),
                new BadFunctionParameter("format",
                    true,
                    true,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                ),
                new BadFunctionParameter("culture",
                    true,
                    true,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                )
            )
        );
        provider.RegisterObject<DateTimeOffset>("Year", d => d.Year);
        provider.RegisterObject<DateTimeOffset>("Month", d => d.Month);
        provider.RegisterObject<DateTimeOffset>("Day", d => d.Day);
        provider.RegisterObject<DateTimeOffset>("Hour", d => d.Hour);
        provider.RegisterObject<DateTimeOffset>("Minute", d => d.Minute);
        provider.RegisterObject<DateTimeOffset>("Second", d => d.Second);
        provider.RegisterObject<DateTimeOffset>("Millisecond", d => d.Millisecond);
        provider.RegisterObject<DateTimeOffset>("DayOfWeek", d => (int)d.DayOfWeek);
        provider.RegisterObject<DateTimeOffset>("DayOfYear", d => d.DayOfYear);
        provider.RegisterObject<DateTimeOffset>("TimeOfDay", d => d.TimeOfDay);
        provider.RegisterObject<DateTimeOffset>("UnixTimeMilliseconds", d => d.ToUnixTimeMilliseconds());
        provider.RegisterObject<DateTimeOffset>("UnixTimeSeconds", d => d.ToUnixTimeSeconds());
        provider.RegisterObject<DateTimeOffset>("Offset", d => d.Offset);
        provider.RegisterObject<DateTimeOffset>("ToShortTimeString", d => new BadInteropFunction("ToShortTimeString",
            args =>args.Length < 1 ? d.Date.ToShortTimeString() : TimeZoneInfo.ConvertTimeBySystemTimeZoneId(d.DateTime,((IBadString)args[0]).Value).ToShortTimeString(),
            false,
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("timeZone",
                true,
                false,
                false,
                null,
                BadNativeClassBuilder.GetNative("string")
            )
        ) );
        
        provider.RegisterObject<DateTimeOffset>("ToShortDateString", d => new BadInteropFunction("ToShortDateString",
            args =>args.Length < 1 ? d.Date.ToShortDateString() : TimeZoneInfo.ConvertTimeBySystemTimeZoneId(d.DateTime,((IBadString)args[0]).Value).ToShortDateString(),
            false,
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("timeZone",
                true,
                false,
                false,
                null,
                BadNativeClassBuilder.GetNative("string")
            )
        ) );
        
        provider.RegisterObject<DateTimeOffset>("ToLongTimeString", d => new BadInteropFunction("ToLongTimeString",
            args =>args.Length < 1 ? d.Date.ToLongTimeString() : TimeZoneInfo.ConvertTimeBySystemTimeZoneId(d.DateTime,((IBadString)args[0]).Value).ToLongTimeString(),
            false,
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("timeZone",
                true,
                false,
                false,
                null,
                BadNativeClassBuilder.GetNative("string")
            )
        ) );
        
        provider.RegisterObject<DateTimeOffset>("ToLongDateString", d => new BadInteropFunction("ToLongDateString",
            args =>args.Length < 1 ? d.Date.ToLongDateString() : TimeZoneInfo.ConvertTimeBySystemTimeZoneId(d.DateTime,((IBadString)args[0]).Value).ToLongDateString(),
            false,
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("timeZone",
                true,
                false,
                false,
                null,
                BadNativeClassBuilder.GetNative("string")
            )
        ) );
        
        provider.RegisterObject<DateTimeOffset>("Format", d => new BadInteropFunction("Format",
            (ctx, args) =>
            {
                string format = ((IBadString)args[0]).Value;

                CultureInfo? c = ctx.Scope.GetSingleton<BadRuntime>()
                                     ?.Culture ??
                                 CultureInfo.InvariantCulture;

                if (args.Length == 3 && args[2] is IBadString str)
                {
                    c = new CultureInfo(str.Value);
                }

                string? timeZone =
                    args.Length < 2 ? null : (args[1] as IBadString)?.Value;
                if (timeZone != null)
                {
                    return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(d.DateTime, timeZone).ToString(format, c);
                }
                return d.ToString(format, c);
            },
            false,
            BadNativeClassBuilder.GetNative("string"),
            new BadFunctionParameter("format",
                false,
                true,
                false,
                null,
                BadNativeClassBuilder.GetNative("string")
            ),
            new BadFunctionParameter("timeZone",
                true,
                false,
                false,
                null,
                BadNativeClassBuilder.GetNative("string")
            ),
            new BadFunctionParameter("culture",
                true,
                false,
                false,
                null,
                BadNativeClassBuilder.GetNative("string")
            )
        ));
        
        provider.RegisterObject<DateTimeOffset>("WeekOfYear", d => new BadInteropFunction("WeekOfYear",
            (ctx, args) =>
            {
                CultureInfo? c = ctx.Scope.GetSingleton<BadRuntime>()
                                     ?.Culture ??
                                 CultureInfo.InvariantCulture;

                if (args.Length == 1 && args[0] is IBadString str)
                {
                    c = new CultureInfo(str.Value);
                }

                return c.Calendar.GetWeekOfYear(d.DateTime,
                    c.DateTimeFormat.CalendarWeekRule,
                    c.DateTimeFormat.FirstDayOfWeek
                );
            },
            false,
            BadNativeClassBuilder.GetNative("num"),
            new BadFunctionParameter("culture",
                true,
                false,
                false,
                null,
                BadNativeClassBuilder.GetNative("string")
            )
        ));

        provider.RegisterObject<DateTimeOffset>(
            "AddYears",
            d =>
                new BadDynamicInteropFunction<decimal>(
                    "AddYears", (_, y) => d.AddYears((int)y),
                    BadDate.Prototype,
                    new BadFunctionParameter(
                        "years",
                        false,
                        true,
                        false,
                        null,
                        BadNativeClassBuilder.GetNative("num")
                    )
                )
        );
        
        provider.RegisterObject<DateTimeOffset>(
            "AddMonths",
            d =>
                new BadDynamicInteropFunction<decimal>(
                    "AddMonths", (_, y) => d.AddMonths((int)y),
                    BadDate.Prototype,
                    new BadFunctionParameter(
                        "months",
                        false,
                        true,
                        false,
                        null,
                        BadNativeClassBuilder.GetNative("num")
                    )
                )
        );
        
        provider.RegisterObject<DateTimeOffset>(
            "AddDays",
            d =>
                new BadDynamicInteropFunction<decimal>(
                    "AddDays", (_, y) => d.AddDays((int)y),
                    BadDate.Prototype,
                    new BadFunctionParameter(
                        "days",
                        false,
                        true,
                        false,
                        null,
                        BadNativeClassBuilder.GetNative("num")
                    )
                )
        );
        
        provider.RegisterObject<DateTimeOffset>(
            "AddHours",
            d =>
                new BadDynamicInteropFunction<decimal>(
                    "AddHours", (_, y) => d.AddHours((int)y),
                    BadDate.Prototype,
                    new BadFunctionParameter(
                        "hours",
                        false,
                        true,
                        false,
                        null,
                        BadNativeClassBuilder.GetNative("num")
                    )
                )
        );
        
        provider.RegisterObject<DateTimeOffset>(
            "AddMinutes",
            d =>
                new BadDynamicInteropFunction<decimal>(
                    "AddMinutes", (_, y) => d.AddMinutes((int)y),
                    BadDate.Prototype,
                    new BadFunctionParameter(
                        "minutes",
                        false,
                        true,
                        false,
                        null,
                        BadNativeClassBuilder.GetNative("num")
                    )
                )
        );
        
        provider.RegisterObject<DateTimeOffset>(
            "AddSeconds",
            d =>
                new BadDynamicInteropFunction<decimal>(
                    "AddSeconds", (_, y) => d.AddSeconds((int)y),
                    BadDate.Prototype,
                    new BadFunctionParameter(
                        "seconds",
                        false,
                        true,
                        false,
                        null,
                        BadNativeClassBuilder.GetNative("num")
                    )
                )
        );
        
        provider.RegisterObject<DateTimeOffset>(
            "AddMilliseconds",
            d =>
                new BadDynamicInteropFunction<decimal>(
                    "AddMilliseconds", (_, y) => d.AddMilliseconds((int)y),
                    BadDate.Prototype,
                    new BadFunctionParameter(
                        "ms",
                        false,
                        true,
                        false,
                        null,
                        BadNativeClassBuilder.GetNative("num")
                    )
                )
        );
        
        provider.RegisterObject<DateTimeOffset>(
            "AddTicks",
            d =>
                new BadDynamicInteropFunction<decimal>(
                    "AddTicks", (_, y) => d.AddTicks((long)y),
                    BadDate.Prototype,
                    new BadFunctionParameter(
                        "ticks",
                        false,
                        true,
                        false,
                        null,
                        BadNativeClassBuilder.GetNative("num")
                    )
                )
        );

        Func<DateTimeOffset, BadObject> addFunc = d =>
            new BadDynamicInteropFunction<TimeSpan>(
                "Add", (_, y) => d.Add(y),
                BadDate.Prototype,
                new BadFunctionParameter(
                    "time",
                    false,
                    true,
                    false,
                    null,
                    BadTime.Prototype
                )
            );
        provider.RegisterObject(BadStaticKeys.ADD_OPERATOR_NAME, addFunc);
        provider.RegisterObject("Add", addFunc);


        Func<DateTimeOffset, BadObject> subtractFunc = d =>
            new BadDynamicInteropFunction<TimeSpan>(
                "Subtract", (_, y) => d.Subtract(y),
                BadDate.Prototype,
                new BadFunctionParameter(
                    "time",
                    false,
                    true,
                    false,
                    null,
                    BadTime.Prototype
                )
            );
        provider.RegisterObject(BadStaticKeys.SUBTRACT_OPERATOR_NAME, subtractFunc);
        provider.RegisterObject("Subtract", subtractFunc);
        
        provider.RegisterObject<DateTimeOffset>(
            "ToUniversalTime",
            d => new BadInteropFunction("ToUniversalTime",
                args => d.ToUniversalTime(),
                false,
                BadDate.Prototype
            )
        );
        
        provider.RegisterObject<DateTimeOffset>(BadStaticKeys.EQUAL_OPERATOR_NAME, d => new BadDynamicInteropFunction<BadObject>(
            BadStaticKeys.EQUAL_OPERATOR_NAME, 
            (_, o) => o is IBadDate other && d == other.Value, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadAnyPrototype.Instance)
        ));
        
        provider.RegisterObject<DateTimeOffset>(BadStaticKeys.NOT_EQUAL_OPERATOR_NAME, d => new BadDynamicInteropFunction<BadObject>(
            BadStaticKeys.NOT_EQUAL_OPERATOR_NAME, 
            (_, o) => o is not IBadDate other || d != other.Value, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadAnyPrototype.Instance)
        ));

        provider.RegisterObject<DateTimeOffset>(BadStaticKeys.GREATER_OPERATOR_NAME, d => new BadDynamicInteropFunction<DateTimeOffset>(
            BadStaticKeys.GREATER_OPERATOR_NAME, 
            (_, o) => d > o, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadDate.Prototype)
        ));

        provider.RegisterObject<DateTimeOffset>(BadStaticKeys.GREATER_EQUAL_OPERATOR_NAME, d => new BadDynamicInteropFunction<DateTimeOffset>(
            BadStaticKeys.GREATER_EQUAL_OPERATOR_NAME, 
            (_, o) => d >= o, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadDate.Prototype)
        ));

        provider.RegisterObject<DateTimeOffset>(BadStaticKeys.LESS_OPERATOR_NAME, d => new BadDynamicInteropFunction<DateTimeOffset>(
            BadStaticKeys.LESS_OPERATOR_NAME, 
            (_, o) => d < o, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadDate.Prototype)
        ));

        provider.RegisterObject<DateTimeOffset>(BadStaticKeys.LESS_EQUAL_OPERATOR_NAME, d => new BadDynamicInteropFunction<DateTimeOffset>(
            BadStaticKeys.LESS_EQUAL_OPERATOR_NAME, 
            (_, o) => d <= o, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadDate.Prototype)
        ));

        
        provider.RegisterObject<DateTimeOffset>(
            "ToLocalTime",
            d => new BadInteropFunction("ToLocalTime",
                args => d.ToLocalTime(),
                false,
                BadDate.Prototype
            )
        );
        
        provider.RegisterObject<DateTimeOffset>(
            "ToOffset",
            d => new BadInteropFunction("ToOffset",
                args => d.ToOffset(((IBadTime)args[0]).Value),
                false,
                BadDate.Prototype,
                new BadFunctionParameter(
                    "offset",
                    false,
                    true,
                    false,
                    null,
                    BadTime.Prototype
                )
            )
        );
        #endregion

        #region Time

        provider.RegisterObject<TimeSpan>(BadStaticKeys.EQUAL_OPERATOR_NAME, d => new BadDynamicInteropFunction<BadObject>(
            BadStaticKeys.EQUAL_OPERATOR_NAME, 
            (_, o) => o is IBadTime other && d == other.Value, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadAnyPrototype.Instance)
        ));
        
        provider.RegisterObject<TimeSpan>(BadStaticKeys.NOT_EQUAL_OPERATOR_NAME, d => new BadDynamicInteropFunction<BadObject>(
            BadStaticKeys.NOT_EQUAL_OPERATOR_NAME, 
            (_, o) => o is not IBadTime other || d != other.Value, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadAnyPrototype.Instance)
        ));

        provider.RegisterObject<TimeSpan>(BadStaticKeys.GREATER_OPERATOR_NAME, d => new BadDynamicInteropFunction<TimeSpan>(
            BadStaticKeys.GREATER_OPERATOR_NAME, 
            (_, o) => d > o, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadTime.Prototype)
        ));

        provider.RegisterObject<TimeSpan>(BadStaticKeys.GREATER_EQUAL_OPERATOR_NAME, d => new BadDynamicInteropFunction<TimeSpan>(
            BadStaticKeys.GREATER_EQUAL_OPERATOR_NAME, 
            (_, o) => d >= o, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadTime.Prototype)
        ));

        provider.RegisterObject<TimeSpan>(BadStaticKeys.LESS_OPERATOR_NAME, d => new BadDynamicInteropFunction<TimeSpan>(
            BadStaticKeys.LESS_OPERATOR_NAME, 
            (_, o) => d < o, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadTime.Prototype)
        ));

        provider.RegisterObject<TimeSpan>(BadStaticKeys.LESS_EQUAL_OPERATOR_NAME, d => new BadDynamicInteropFunction<TimeSpan>(
            BadStaticKeys.LESS_EQUAL_OPERATOR_NAME, 
            (_, o) => d <= o, 
            BadNativeClassBuilder.GetNative("bool"),
            new BadFunctionParameter("right", false, false, false, null, BadTime.Prototype)
        ));
        provider.RegisterObject<TimeSpan>("ToString",
            d => new BadInteropFunction("ToString",
                (c, a) => TimeToString(c, d, a),
                false,
                BadNativeClassBuilder.GetNative("string"),
                new BadFunctionParameter("format",
                    true,
                    true,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                ),
                new BadFunctionParameter("culture",
                    true,
                    true,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                )
            )
        );
        
        provider.RegisterObject<TimeSpan>("Hours", d => d.Hours);
        provider.RegisterObject<TimeSpan>("Minutes", d => d.Minutes);
        provider.RegisterObject<TimeSpan>("Seconds", d => d.Seconds);
        provider.RegisterObject<TimeSpan>("Milliseconds", d => d.Milliseconds);
        provider.RegisterObject<TimeSpan>("Ticks", d => d.Ticks);
        provider.RegisterObject<TimeSpan>("TotalHours", d => (decimal)d.TotalHours);
        provider.RegisterObject<TimeSpan>("TotalMinutes", d => (decimal)d.TotalMinutes);
        provider.RegisterObject<TimeSpan>("TotalSeconds", d => (decimal)d.TotalSeconds);
        provider.RegisterObject<TimeSpan>("TotalMilliseconds", d => (decimal)d.TotalMilliseconds);
        provider.RegisterObject<TimeSpan>("Negate", d => new BadInteropFunction("Time.Negate", (_,_) => d.Negate(), false, BadTime.Prototype));
        provider.RegisterObject<TimeSpan>("Add", d => new BadDynamicInteropFunction<TimeSpan>("Time.Add",
            (_, y) => d.Add(y),
            BadTime.Prototype,
            new BadFunctionParameter("time", false, true, false, null, BadTime.Prototype)
        ));
        provider.RegisterObject<TimeSpan>("Subtract", d => new BadDynamicInteropFunction<TimeSpan>("Time.Subtract",
            (_, y) => d.Subtract(y),
            BadTime.Prototype,
            new BadFunctionParameter("time", false, true, false, null, BadTime.Prototype)
        ));
        #endregion
    }
}