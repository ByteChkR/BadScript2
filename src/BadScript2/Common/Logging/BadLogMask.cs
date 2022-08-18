namespace BadScript2.Common.Logging;

public class BadLogMask
{
    private static readonly Dictionary<string, ulong> s_Masks = new Dictionary<string, ulong>
        { { "None", 0 }, { "Default", 1 } };

    public static readonly BadLogMask None = new BadLogMask(0);
    public static readonly BadLogMask All = new BadLogMask(~0UL);
    public static readonly BadLogMask Default = new BadLogMask(1);


    private readonly ulong Mask;

    private BadLogMask(ulong mask)
    {
        Mask = mask;
    }


    public static BadLogMask Register(string name)
    {
        ulong value = (ulong)1 << s_Masks.Count;
        s_Masks.Add(name, value);

        return new BadLogMask(value);
    }

    public static implicit operator BadLogMask(ulong mask)
    {
        return new BadLogMask(mask);
    }

    public static implicit operator BadLogMask(string name)
    {
        if (name == "None")
        {
            return None;
        }

        if (name == "All")
        {
            return All;
        }

        if (!s_Masks.ContainsKey(name))
        {
            return Register(name);
        }

        return new BadLogMask(s_Masks[name]);
    }

    private static bool IsPowerOfTwo(ulong number)
    {
        if (number == 0)
        {
            return false;
        }

        for (ulong power = 1; power > 0; power = power << 1)
        {
            // This for loop used shifting for powers of 2, meaning
            // that the value will become 0 after the last shift
            // (from binary 1000...0000 to 0000...0000) then, the 'for'
            // loop will break out.

            if (power == number)
            {
                return true;
            }

            if (power > number)
            {
                return false;
            }
        }

        return false;
    }


    public static BadLogMask GetMask(params BadLogMask[] names)
    {
        ulong mask = 0;
        foreach (BadLogMask name in names)
        {
            mask |= name.Mask;
        }

        return mask;
    }

    public static implicit operator ulong(BadLogMask mask)
    {
        return mask.Mask;
    }

    public string[] GetNames()
    {
        if (Mask == None.Mask)
        {
            return new[] { "None" };
        }

        if (Mask == All.Mask)
        {
            return new[] { "All" };
        }

        if (IsPowerOfTwo(Mask))
        {
            if (s_Masks.Any(x => x.Value == Mask))
            {
                return new[] { s_Masks.First(x => x.Value == Mask).Key };
            }

            return Array.Empty<string>();
        }

        List<string> names = new List<string>();

        foreach (KeyValuePair<string, ulong> kvp in s_Masks)
        {
            if (kvp.Value != 0 && (kvp.Value & Mask) == kvp.Value)
            {
                names.Add(kvp.Key);
            }
        }

        if (names.Count == 0)
        {
            return Array.Empty<string>();
        }

        return names.ToArray();
    }

    public static implicit operator string(BadLogMask mask)
    {
        return string.Join(' ', mask.GetNames());
    }

    public bool IsContainedIn(BadLogMask other)
    {
        return (Mask & other.Mask) == Mask;
    }

    public bool IsExactly(BadLogMask other)
    {
        return Mask == other.Mask;
    }

    public bool Contains(BadLogMask mask)
    {
        return mask.IsContainedIn(this);
    }

    public override string ToString()
    {
        return this;
    }
}