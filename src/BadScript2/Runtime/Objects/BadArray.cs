using System.Collections;
using System.Text;

using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects;

/// <summary>
///     Implements a Dynamic List/Array for the BadScript Language
/// </summary>
public class BadArray : BadObject, IBadEnumerable

{
    /// <summary>
    ///     Creates a new Instance of the BadScript Array
    /// </summary>
    /// <param name="innerArray">The Initial Elements</param>
    public BadArray(List<BadObject> innerArray)
    {
        InnerArray = innerArray;
    }

    /// <summary>
    ///     Creates a new Instance of the BadScript Array
    /// </summary>
    public BadArray() : this(new List<BadObject>()) { }

    /// <summary>
    ///     The Inner Array
    /// </summary>
    public List<BadObject> InnerArray { get; }

    public IEnumerator<BadObject> GetEnumerator()
    {
        foreach (BadObject o in InnerArray)
        {
            yield return o;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.GetNative("Array");
    }


    public override string ToSafeString(List<BadObject> done)
    {
        done.Add(this);
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        sb.AppendLine();
        foreach (BadObject element in InnerArray)
        {
            string str = "{...}";
            if (!done.Contains(element))
            {
                str = element.ToSafeString(done)!.Trim();
            }

            if (str.Contains("\n"))
            {
                str = str.Replace("\n", "\n\t");
            }

            sb.AppendLine($"\t{str}");
        }

        sb.AppendLine("]");

        return sb.ToString();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        sb.AppendLine();
        foreach (BadObject element in InnerArray)
        {
            if (element is BadScope)
            {
                sb.AppendLine("RECURSION_PROTECT");

                continue;
            }

            string str = element.ToString()!.Trim();

            if (str.Contains("\n"))
            {
                str = str.Replace("\n", "\n\t");
            }

            sb.AppendLine($"\t{str}");
        }

        sb.AppendLine("]");

        return sb.ToString();
    }
}