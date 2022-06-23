namespace BadScript2.Runtime.Objects;

public static class BadObjectExtensions
{
    public static BadObject Dereference(this BadObject obj)
    {
        while (obj is BadObjectReference r)
        {
            obj = r.Resolve();
        }

        return obj;
    }
}