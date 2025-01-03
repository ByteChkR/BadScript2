using System.Globalization;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Runtime.Objects.Types;

public class BadTimePrototype : BadNativeClassPrototype<BadTime>
{
    private static Dictionary<string, BadObjectReference> s_StaticMembers = new Dictionary<string, BadObjectReference>
    {
        {
            "Zero", BadObjectReference.Make("Time.Zero", p => BadTime.Zero)
        },
        {
            "Parse", BadObjectReference.Make("Time.Parse", p => new BadInteropFunction("Parse", Parse, true, BadDate.Prototype))
        }
    };
    private static BadObject Parse(BadExecutionContext ctx, BadObject[] args)
    {
        if (args.Length == 1 || (args.Length == 2 && args[1] == Null))
        {
            return TimeSpan.Parse(args[0].ToString());
        }
        if (args.Length == 2)
        {
            if(args[1] is IBadString str)
                return TimeSpan.Parse(args[0].ToString(), CultureInfo.GetCultureInfo(str.Value));
        }
        
        throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count");
    }

    private static BadObject TimeConstructor(BadExecutionContext ctx, BadObject[] args)
    {
        if (args.Length == 0)
        {
            return BadTime.Zero;
        }
        if(args.Length == 1)
        {
            if(args[0] is IBadNumber n)
            {
                return new BadTime(TimeSpan.FromMilliseconds((double)n.Value));
            }
            if(args[0] is BadString s)
            {
                if(TimeSpan.TryParse(s.Value, out TimeSpan dt))
                {
                    return new BadTime(dt);
                }
            }
        }
        if(args.Length == 3)
        {
            if(args[0] is IBadNumber h && args[1] is IBadNumber m && args[2] is IBadNumber s)
            {
                return new BadTime(new TimeSpan((int)h.Value, (int)m.Value, (int)s.Value));
            }
        }
        if(args.Length == 4)
        {
            if(args[0] is IBadNumber h && args[1] is IBadNumber m && args[2] is IBadNumber s && args[3] is IBadNumber ms)
            {
                return new BadTime(new TimeSpan((int)h.Value, (int)m.Value, (int)s.Value, (int)ms.Value));
            }
        }
        throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count");
    }
    public BadTimePrototype() : base("Time", TimeConstructor, s_StaticMembers, null)
    {
    }
}