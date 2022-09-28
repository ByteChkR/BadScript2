using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;

namespace BadScript2.Runtime.Objects.Types
{
    /// <summary>
    ///     Implements a Class Prototype created from Bad Expressions(e.g. Source Code)
    /// </summary>
    public class BadExpressionClassPrototype : BadClassPrototype
    {
        /// <summary>
        ///     The Class Body(Members & Functions)
        /// </summary>
        private readonly BadExpression[] m_Body;

        /// <summary>
        ///     The Parent scope this class prototype was created in.
        /// </summary>
        private readonly BadScope m_ParentScope;

        /// <summary>
        ///     Creates a new BadExpressionClassPrototype
        /// </summary>
        /// <param name="name">Name of the Type</param>
        /// <param name="parentScope">The Parent scope this class prototype was created in.</param>
        /// <param name="body">The Class Body(Members & Functions)</param>
        /// <param name="baseClass">The Base class of the prototype</param>
        public BadExpressionClassPrototype(
            string name,
            BadScope parentScope,
            BadExpression[] body,
            BadClassPrototype? baseClass) : base(name, baseClass)
        {
            m_ParentScope = parentScope;
            m_Body = body;
        }


        public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
        {
            BadClass? baseInstance = null;
            BadExecutionContext ctx = new BadExecutionContext(
                m_ParentScope.CreateChild($"class instance {Name}", caller.Scope)
            );
            ctx.Scope.SetFlags(BadScopeFlags.None);
            if (BaseClass != null)
            {
                BadObject obj = Null;
                foreach (BadObject o in BaseClass.CreateInstance(caller, false))
                {
                    obj = o;
                }

                if (obj is not BadClass cls)
                {
                    throw new BadRuntimeException("Base class is not a class");
                }

                baseInstance = cls;
                ctx.Scope.GetTable().SetProperty("base", baseInstance, new BadPropertyInfo(BaseClass, true));
            }

            foreach (BadObject o in ctx.Execute(m_Body))
            {
                yield return o;
            }

            BadClass thisInstance = new BadClass(Name, ctx.Scope.GetTable(), baseInstance, this);

            if (setThis)
            {
                thisInstance.SetThis();
            }

            yield return thisInstance;
        }
    }
}