using System.Globalization;

namespace BadScript2.Interop.Common.Versioning;

/// <summary>
///     Imlpements Versioning Extensions
/// </summary>
public static class BadVersionExtensions
{
    /// <summary>
    ///     Changes the Version Object by the given Change String
    /// </summary>
    /// <param name="version">Version Object</param>
    /// <param name="changeStr">Change String</param>
    /// <returns>New Version Object</returns>
    public static Version ChangeVersion(this Version version, string changeStr)
    {
        string[] subVersions = changeStr.Split('.');
        int[] wrapValues = { ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue };
        int[] original = { version.Major, version.Minor, version.Build, version.Revision };
        int[] versions = { version.Major, version.Minor, version.Build, version.Revision };
        bool[] changeReset = new bool[4];

        for (int i = 4 - 1; i >= 0; i--)
        {
            string current = subVersions[i];

            if (current.StartsWith("("))
            {
                int j = 0;

                for (; j < current.Length; j++)
                {
                    if (current[j] == ')')
                    {
                        break;
                    }
                }

                if (j == current.Length)
                {
                    continue; //Broken. No number left. better ignore
                }

                string max = current.Substring(1, j - 1);

                if (max == "~")
                {
                    changeReset[i] = true;
                }
                else if (int.TryParse(max, out int newMax))
                {
                    if (i == 0)
                    {
                        continue; //Can not wrap the last digit
                    }

                    wrapValues[i] = newMax;
                }

                current = current.Remove(0, j + 1);
            }

            if (i != 0) //Check if we wrapped
            {
                if (versions[i] >= wrapValues[i])
                {
                    versions[i] = 0;
                    versions[i - 1]++;
                }
            }

            switch (current)
            {
                case "+":
                    versions[i]++;

                    break;
                case "-" when versions[i] != 0:
                    versions[i]--;

                    break;
                default:
                {
                    if (current.ToLower(CultureInfo.InvariantCulture) == "x")
                    {
                        //Do nothing, X stands for leave the value as is, except the next lower version part wrapped around.
                    }
                    else if (current.StartsWith("{") && current.EndsWith("}"))
                    {
                        string format = current.Remove(current.Length - 1, 1)
                                               .Remove(0, 1);

                        string value = DateTime.Now.ToString(format);

                        if (long.TryParse(value, out long newValue))
                        {
                            versions[i] = (int)(newValue % ushort.MaxValue);
                        }
                    }
                    else if (int.TryParse(current, out int v))
                    {
                        versions[i] = v;
                    }

                    break;
                }
            }
        }

        ApplyChangeReset(changeReset, original, versions);

        return new Version(versions[0],
                           versions[1] < 0 ? 0 : versions[1],
                           versions[2] < 0 ? 0 : versions[2],
                           versions[3] < 0 ? 0 : versions[3]
                          );
    }


    /// <summary>
    ///     Applies the Change Reset to the Version Array
    /// </summary>
    /// <param name="changeReset">Version Reset Array</param>
    /// <param name="original">The Original Version Components</param>
    /// <param name="versions">The Version Components</param>
    private static void ApplyChangeReset(IReadOnlyList<bool> changeReset,
                                         IReadOnlyList<int> original,
                                         IList<int> versions)
    {
        for (int j = 0; j < changeReset.Count; j++)
        {
            if (!changeReset[j] || versions[j] == original[j])
            {
                continue;
            }

            for (int i = j + 1; i < versions.Count; i++)
            {
                if (!changeReset[i])
                {
                    versions[i] = 0;
                }
            }
        }
    }
}