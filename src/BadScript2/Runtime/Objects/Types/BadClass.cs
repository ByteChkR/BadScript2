using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;

namespace BadScript2.Runtime.Objects.Types
{
    public class BadClass : BadObject
    {
        private readonly BadClass? m_BaseClass;
        private readonly BadTable m_Table;

        public readonly string Name;
        public readonly BadClassPrototype Prototype;

        public BadClass(string name, BadTable table, BadClass? baseClass, BadClassPrototype prototype)
        {
            Name = name;
            m_Table = table;
            m_BaseClass = baseClass;
            Prototype = prototype;
        }

        public BadClass? SuperClass { get; private set; }


        public bool InheritsFrom(BadClassPrototype proto)
        {
            return Prototype == proto || (m_BaseClass != null && m_BaseClass.InheritsFrom(proto));
        }

        public void SetThis()
        {
            SetThis(this);
        }

        private void SetThis(BadClass thisInstance)
        {
            SuperClass = thisInstance;
            m_Table.GetProperty("this").Set(thisInstance, new BadPropertyInfo(thisInstance.Prototype, true));
            m_BaseClass?.SetThis(thisInstance);
        }

        public override BadClassPrototype GetPrototype()
        {
            return Prototype;
        }

        public override bool HasProperty(BadObject propName)
        {
            if (m_Table.InnerTable.ContainsKey(propName))
            {
                return true;
            }

            if (m_BaseClass != null)
            {
                return m_BaseClass.HasProperty(propName);
            }

            return BadInteropExtension.HasObject(GetType(), propName);
        }

        public override BadObjectReference GetProperty(BadObject propName)
        {
            if (!HasProperty(propName))
            {
                throw new BadRuntimeException($"Property {propName} not found in class {Name} or any of its base classes");
            }

            if (m_Table.InnerTable.ContainsKey(propName))
            {
                return BadObjectReference.Make(
                    $"{Name}.{propName}",
                    () => m_Table.InnerTable[propName],
                    (o, t) =>
                    {
                        if (m_Table.InnerTable.ContainsKey(propName))
                        {
                            BadPropertyInfo info = m_Table.GetPropertyInfo(propName);
                            if (m_Table.InnerTable[propName] != Null && info.IsReadOnly)
                            {
                                throw new BadRuntimeException($"{Name}.{propName} is read-only");
                            }

                            if (info.Type != null && !info.Type.IsAssignableFrom(o))
                            {
                                throw new BadRuntimeException(
                                    $"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{info.Type.Name}'"
                                );
                            }
                        }

                        m_Table.InnerTable[propName] = o;
                    }
                );
            }

            if (m_BaseClass != null)
            {
                return m_BaseClass.GetProperty(propName);
            }

            return BadInteropExtension.GetObjectReference(GetType(), propName, SuperClass ?? this);
        }


        public override string ToSafeString(List<BadObject> done)
        {
            done.Add(this);

            return
                $"class {Name}\n{m_Table.ToSafeString(done)}";
        }
    }
}