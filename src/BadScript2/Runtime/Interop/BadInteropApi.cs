using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop
{
    public abstract class BadInteropApi
    {
        protected BadInteropApi(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public abstract void Load(BadTable target);
    }
}