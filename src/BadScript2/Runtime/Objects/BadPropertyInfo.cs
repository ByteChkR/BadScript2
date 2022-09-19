using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects;

public class BadPropertyInfo
{
    public readonly BadClassPrototype? Type;
    public bool IsReadOnly;

    public BadPropertyInfo(BadClassPrototype? type = null, bool isReadOnly = false)
    {
        Type = type;
        IsReadOnly = isReadOnly;
    }
}