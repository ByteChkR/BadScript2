using System.Text;

using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects;

public class BadArray : BadObject
{
    public readonly List<BadObject> InnerArray;

    public BadArray(List<BadObject> innerArray)
    {
        InnerArray = innerArray;
    }

    public BadArray() : this(new List<BadObject>()) { }

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