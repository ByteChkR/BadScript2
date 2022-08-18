using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects;

public class BadPropertyInfo
{
    public bool IsReadOnly;
    public readonly BadClassPrototype? Type;

    public BadPropertyInfo(BadClassPrototype? type = null, bool isReadOnly = false)
    {
        Type = type;
        IsReadOnly = isReadOnly;
    }
}