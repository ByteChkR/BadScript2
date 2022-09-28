namespace BadScript2.IO.Virtual
{
    /// <summary>
    ///     Base class for all Virtual Filesystem Nodes
    /// </summary>
    public abstract class BadVirtualNode
    {
        /// <summary>
        ///     Node Name
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     Creates a new Virtual Node
        /// </summary>
        /// <param name="name">Node Name</param>
        /// <param name="parent">Parent Directory</param>
        protected BadVirtualNode(string name, BadVirtualDirectory? parent)
        {
            Name = name;
            Parent = parent;
        }

        /// <summary>
        ///     Parent Directory
        /// </summary>
        public BadVirtualDirectory? Parent { get; }

        /// <summary>
        ///     Is true if the node contains any children
        /// </summary>
        public abstract bool HasChildren { get; }

        /// <summary>
        ///     List of all child nodes
        /// </summary>
        public abstract IEnumerable<BadVirtualNode> Children { get; }
    }
}