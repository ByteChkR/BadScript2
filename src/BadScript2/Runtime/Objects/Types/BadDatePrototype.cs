using System.Globalization;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Objects.Types;

public class BadDatePrototype : BadNativeClassPrototype<BadDate>
{

    private static Dictionary<string, BadObjectReference> s_StaticMembers = new Dictionary<string, BadObjectReference>
    {
        {
            "Now", BadObjectReference.Make("Date.Now", p => BadDate.Now)
        },
        {
            "UtcNow", BadObjectReference.Make("Date.UtcNow", p => BadDate.UtcNow)
        },
        {
            "Parse", BadObjectReference.Make("Date.Parse", p => new BadInteropFunction("Parse", Parse, true, BadDate.Prototype))
        }
    };
    private static BadObject Parse(BadExecutionContext ctx, BadObject[] args)
    {
        if (args.Length == 1 || (args.Length == 2 && args[1] == Null))
        {
            return DateTimeOffset.Parse(args[0].ToString());
        }
        if (args.Length == 2)
        {
            if(args[1] is IBadString str)
                return DateTimeOffset.Parse(args[0].ToString(), CultureInfo.GetCultureInfo(str.Value));
        }
        
        throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count");
    }

    private static BadObject DateConstructor(BadExecutionContext ctx, BadObject[] args)
    {
        if (args.Length == 0)
        {
            return BadDate.Now;
        }
        if(args.Length == 1)
        {
            if(args[0] is IBadNumber n)
            {
                return new BadDate(DateTimeOffset.FromUnixTimeMilliseconds((long)n.Value));
            }
            if(args[0] is BadString s)
            {
                if(DateTimeOffset.TryParse(s.Value, out DateTimeOffset dt))
                {
                    return new BadDate(dt);
                }
            }
        }
        if(args.Length == 3)
        {
            if(args[0] is IBadNumber y && args[1] is IBadNumber m && args[2] is IBadNumber d)
            {
                return new BadDate(new DateTimeOffset((int)y.Value, (int)m.Value, (int)d.Value, 0, 0, 0, TimeSpan.Zero));
            }
        }
        if(args.Length == 6)
        {
            if(args[0] is IBadNumber y && args[1] is IBadNumber m && args[2] is IBadNumber d && args[3] is IBadNumber h && args[4] is IBadNumber min && args[5] is IBadNumber s)
            {
                return new BadDate(new DateTimeOffset((int)y.Value, (int)m.Value, (int)d.Value, (int)h.Value, (int)min.Value, (int)s.Value, TimeSpan.Zero));
            }
        }
        if(args.Length == 7)
        {
            if(args[0] is IBadNumber y && args[1] is IBadNumber m && args[2] is IBadNumber d && args[3] is IBadNumber h && args[4] is IBadNumber min && args[5] is IBadNumber s && args[6] is IBadNumber ms)
            {
                return new BadDate(new DateTimeOffset((int)y.Value, (int)m.Value, (int)d.Value, (int)h.Value, (int)min.Value, (int)s.Value, (int)ms.Value, TimeSpan.Zero));
            }
        }
        if (args.Length == 8)
        {
            if(args[0] is IBadNumber y && args[1] is IBadNumber m && args[2] is IBadNumber d && args[3] is IBadNumber h && args[4] is IBadNumber min && args[5] is IBadNumber s && args[6] is IBadNumber ms && args[7] is IBadTime time)
            {
                return new BadDate(new DateTimeOffset((int)y.Value, (int)m.Value, (int)d.Value, (int)h.Value, (int)min.Value, (int)s.Value, (int)ms.Value, time.Value));
            }
        }
        
        throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count");
    }
    public BadDatePrototype() : base("Date", DateConstructor, s_StaticMembers, null)
    {
    }
}