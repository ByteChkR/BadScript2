using System.Text;

using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects
{
    /// <summary>
    ///     Implements a Table Structure for the BadScript Language
    /// </summary>
    public class BadTable : BadObject
    {
        /// <summary>
        ///     The Inner Table for this Object
        /// </summary>
        public readonly Dictionary<BadObject, BadObject> InnerTable;

        /// <summary>
        ///     A Table of additional property information
        /// </summary>
        public readonly Dictionary<BadObject, BadPropertyInfo> PropertyInfos;

        /// <summary>
        ///     Creates a new Table Object
        /// </summary>
        public BadTable()
        {
            InnerTable = new Dictionary<BadObject, BadObject>();
            PropertyInfos = new Dictionary<BadObject, BadPropertyInfo>();
        }

        /// <summary>
        ///     Creates a new Table Object
        /// </summary>
        /// <param name="table">The Initial Values of the Table</param>
        public BadTable(Dictionary<BadObject, BadObject> table)
        {
            InnerTable = table;
            PropertyInfos = new Dictionary<BadObject, BadPropertyInfo>();
            foreach (KeyValuePair<BadObject, BadObject> kvp in InnerTable)
            {
                PropertyInfos[kvp.Key] = new BadPropertyInfo();
            }
        }


        public override BadClassPrototype GetPrototype()
        {
            return BadNativeClassBuilder.GetNative("Table");
        }

        /// <summary>
        ///     Returns Property Information for a given Key
        /// </summary>
        /// <param name="propName">Property Name</param>
        /// <returns>Property Info</returns>
        public BadPropertyInfo GetPropertyInfo(BadObject propName)
        {
            return PropertyInfos[propName];
        }

        /// <summary>
        ///     Removes a Property from the Table
        /// </summary>
        /// <param name="key">Property Key</param>
        public void RemoveKey(BadObject key)
        {
            PropertyInfos.Remove(key);
            InnerTable.Remove(key);
        }


        public override bool HasProperty(BadObject propName)
        {
            return InnerTable.ContainsKey(propName) || BadInteropExtension.HasObject<BadTable>(propName);
        }

        public override BadObjectReference GetProperty(BadObject propName)
        {
            if (BadInteropExtension.HasObject<BadTable>(propName) && !InnerTable.ContainsKey(propName))
            {
                return BadInteropExtension.GetObjectReference(GetType(), propName, this);
            }

            return BadObjectReference.Make(
                $"BadTable.{propName}",
                () => InnerTable[propName],
                (o, t) =>
                {
                    if (InnerTable.ContainsKey(propName))
                    {
                        BadPropertyInfo info = GetPropertyInfo(propName);
                        if (InnerTable[propName] != Null && info.IsReadOnly)
                        {
                            throw new BadRuntimeException($"{propName} is read-only");
                        }

                        if (info.Type != null && !info.Type.IsAssignableFrom(o))
                        {
                            throw new BadRuntimeException(
                                $"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{info.Type.Name}'"
                            );
                        }
                    }
                    else
                    {
                        PropertyInfos[propName] = t ?? new BadPropertyInfo();
                        if (t?.Type != null && !t.Type.IsAssignableFrom(o))
                        {
                            throw new BadRuntimeException(
                                $"Cannot assign object {o.GetType().Name} to property '{propName}' of type '{t.Type.Name}'"
                            );
                        }
                    }

                    InnerTable[propName] = o;
                }
            );
        }


        public override string ToSafeString(List<BadObject> done)
        {
            done.Add(this);
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendLine();
            foreach (KeyValuePair<BadObject, BadObject> kvp in InnerTable)
            {
                if (kvp.Key is BadScope || kvp.Value is BadScope)
                {
                    sb.AppendLine("RECURSION_PROTECT");

                    continue;
                }

                string kStr = "{...}";
                if (!done.Contains(kvp.Key))
                {
                    kStr = kvp.Key.ToSafeString(done)!.Trim();
                }

                string vStr = "{...}";
                if (!done.Contains(kvp.Value))
                {
                    vStr = kvp.Value.ToSafeString(done)!.Trim();
                }

                if (kStr.Contains("\n"))
                {
                    kStr = kStr.Replace("\n", "\n\t");
                }

                if (vStr.Contains("\n"))
                {
                    vStr = vStr.Replace("\n", "\n\t");
                }

                sb.AppendLine($"\t{kStr}: {vStr}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}