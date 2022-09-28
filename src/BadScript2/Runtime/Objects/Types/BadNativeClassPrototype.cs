namespace BadScript2.Runtime.Objects.Types
{
    /// <summary>
    ///     Implements a Native Class Prototype
    /// </summary>
    public abstract class BadNativeClassPrototype : BadClassPrototype
    {
        /// <summary>
        ///     The Constructor for the Class
        /// </summary>
        private readonly Func<BadExecutionContext, BadObject[], BadObject> m_Func;

        /// <summary>
        ///     Creates a new Native Class Prototype
        /// </summary>
        /// <param name="name">Class Name</param>
        /// <param name="func">Class Constructor</param>
        protected BadNativeClassPrototype(
            string name,
            Func<BadExecutionContext, BadObject[], BadObject> func) : base(name, null)
        {
            m_Func = func;
        }

        /// <summary>
        ///     Creates an instance of the Class
        /// </summary>
        /// <param name="caller">Caller Context</param>
        /// <param name="args">Constructor Arguments</param>
        /// <returns>Enumeration of BadObjects that were created by the exeuction of the constructor</returns>
        public IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, BadObject[] args)
        {
            yield return m_Func(caller, args);
        }

        public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
        {
            return CreateInstance(caller, Array.Empty<BadObject>());
        }
    }

    /// <summary>
    ///     Implements a Native Class Prototype
    /// </summary>
    /// <typeparam name="T">Native Type</typeparam>
    public class BadNativeClassPrototype<T> : BadNativeClassPrototype
        where T : BadObject
    {
        /// <summary>
        ///     Creates a new Native Class Prototype
        /// </summary>
        /// <param name="name">Class Name</param>
        /// <param name="func">Class Constructor</param>
        public BadNativeClassPrototype(
            string name,
            Func<BadExecutionContext, BadObject[], BadObject> func) : base(name, func) { }

        public override bool IsAssignableFrom(BadObject obj)
        {
            if (obj == Null)
            {
                return true;
            }

            if (obj is T)
            {
                return true;
            }

            return false;
        }
    }
}