namespace BadScript2.IO.Virtual;

public abstract class BadVirtualNode
{
    public readonly string Name;

    protected BadVirtualNode(string name, BadVirtualDirectory? parent)
    {
        Name = name;
        Parent = parent;
    }

    public BadVirtualDirectory? Parent { get; }
    public abstract bool HasChildren { get; }
    public abstract IEnumerable<BadVirtualNode> Children { get; }
}