namespace BadScript2.Common.Logging;

/// <summary>
///     Implements a Mask for Log Messages
/// </summary>
public class BadLogMask
{
	/// <summary>
	///     The Masks that are "known"
	/// </summary>
	private static readonly Dictionary<string, ulong> s_Masks = new Dictionary<string, ulong>
    {
        { "None", 0 }, { "Default", 1 },
    };

	/// <summary>
	///     Static Helper for the "None" Mask
	/// </summary>
	public static readonly BadLogMask None = new BadLogMask(0);

	/// <summary>
	///     Static Helper for the "All" Mask
	/// </summary>
	public static readonly BadLogMask All = new BadLogMask(~0UL);

	/// <summary>
	///     Static Helper for the "Default" Mask
	/// </summary>
	public static readonly BadLogMask Default = new BadLogMask(1);


	/// <summary>
	///     The Mask Flags
	/// </summary>
	private readonly ulong m_Mask;

	/// <summary>
	///     Creates a new Log Mask
	/// </summary>
	/// <param name="mask">The Mask Flags</param>
	private BadLogMask(ulong mask)
    {
        m_Mask = mask;
    }


	/// <summary>
	///     Registers a new Mask Name
	/// </summary>
	/// <param name="name">The name of the mask</param>
	/// <returns>An instance of the log mask object</returns>
	public static BadLogMask Register(string name)
    {
        ulong value = (ulong)1 << (s_Masks.Count - 1);
        s_Masks.Add(name, value);

        return new BadLogMask(value);
    }

	/// <summary>
	///     Automatically converts a mask flag into a log mask object
	/// </summary>
	/// <param name="mask">The mask flag</param>
	/// <returns>An instance of the log mask object</returns>
	public static implicit operator BadLogMask(ulong mask)
    {
        return new BadLogMask(mask);
    }

	/// <summary>
	///     Automatically converts a mask name into a log mask object
	/// </summary>
	/// <param name="name">The mask Name</param>
	/// <returns>An instance of the log mask object</returns>
	public static implicit operator BadLogMask(string name)
    {
        switch (name)
        {
            case "None":
                return None;
            case "All":
                return All;
        }

        return !s_Masks.ContainsKey(name) ? Register(name) : new BadLogMask(s_Masks[name]);
    }

	/// <summary>
	///     Returns true if the specified number is a power of two
	/// </summary>
	/// <param name="number">The number to be checked</param>
	/// <returns>True if the number is a power of two</returns>
	private static bool IsPowerOfTwo(ulong number)
    {
        if (number == 0)
        {
            return false;
        }

        for (ulong power = 1; power > 0; power <<= 1)
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


	/// <summary>
	///     Returns a combined mask of all masks provided
	/// </summary>
	/// <param name="masks">Masks that make up the resulting mask</param>
	/// <returns>The resulting mask</returns>
	public static BadLogMask GetMask(params BadLogMask[] masks)
    {
        return masks.Aggregate<BadLogMask, ulong>(0, (current, name) => current | name.m_Mask);
    }

	/// <summary>
	///     Returns the mask flags of the mask object
	/// </summary>
	/// <param name="mask">The mask instance</param>
	/// <returns>Mask Flags</returns>
	public static implicit operator ulong(BadLogMask mask)
    {
        return mask.m_Mask;
    }

	/// <summary>
	///     Returns a list of masks that are contained in this mask
	/// </summary>
	/// <returns>List of mask names</returns>
	public string[] GetNames()
    {
        if (m_Mask == None.m_Mask)
        {
            return new[] { "None" };
        }

        if (m_Mask == All.m_Mask)
        {
            return new[] { "All" };
        }

        if (IsPowerOfTwo(m_Mask))
        {
            if (s_Masks.Any(x => x.Value == m_Mask))
            {
                return new[]
                {
                    s_Masks.First(x => x.Value == m_Mask)
                           .Key,
                };
            }

            return Array.Empty<string>();
        }

        List<string> names =
            (from kvp in s_Masks where kvp.Value != 0 && (kvp.Value & m_Mask) == kvp.Value select kvp.Key).ToList();

        return names.Count == 0 ? Array.Empty<string>() : names.ToArray();
    }

	/// <summary>
	///     Returns the name of the masks that are contained in this mask
	/// </summary>
	/// <param name="mask">The mask instance</param>
	/// <returns>Mask String Representation([Mask1], [Mask2], ..)</returns>
	public static implicit operator string(BadLogMask mask)
    {
        return string.Join(" ", mask.GetNames());
    }

	/// <summary>
	///     Returns true if this mask is contained in the other mask
	/// </summary>
	/// <param name="other">The other mask</param>
	/// <returns>True if this mask is contained in other</returns>
	public bool IsContainedIn(BadLogMask other)
    {
        return (m_Mask & other.m_Mask) == m_Mask;
    }

	/// <summary>
	///     Returns true if this mask is identical to the other mask
	/// </summary>
	/// <param name="other">The other mask</param>
	/// <returns>True if masks are identical</returns>
	public bool IsExactly(BadLogMask other)
    {
        return m_Mask == other.m_Mask;
    }

	/// <summary>
	///     Returns true if the other mask is contained in this mask
	/// </summary>
	/// <param name="mask">The other mask</param>
	/// <returns>True if the other mask is contained in this</returns>
	public bool Contains(BadLogMask mask)
    {
        return mask.IsContainedIn(this);
    }

	/// <summary>
	///     Returns string representation of the mask
	/// </summary>
	/// <returns>String representation of the mask</returns>
	public override string ToString()
    {
        return this;
    }
}