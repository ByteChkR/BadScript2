namespace BadScript2.Runtime.Objects;

public static class BadObjectExtensions
{

    public static string ToSafeString(this BadObject obj) => obj.ToSafeString(new List<BadObject>());
    public static BadObject Dereference(this BadObject obj)
    {
        while (obj is BadObjectReference r)
        {
            obj = r.Resolve();
        }

        return obj;
    }
}