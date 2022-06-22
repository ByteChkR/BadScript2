using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Variables
{
    public class BadVariableDefinitionExpression : BadVariableExpression
    {
        public readonly bool IsReadOnly;
        public readonly BadExpression? TypeExpression;


        public BadVariableDefinitionExpression(
            string name,
            BadSourcePosition position,
            BadExpression? typeExpression = null,
            bool isReadOnly = false) : base(
            name,
            position
        )
        {
            IsReadOnly = isReadOnly;
            TypeExpression = typeExpression;
        }

        public override string ToString()
        {
            return $"{BadStaticKeys.VariableDefinitionKey} {Name}";
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadClassPrototype? type = null;
            if (TypeExpression != null)
            {
                BadObject obj = BadObject.Null;
                foreach (BadObject o in TypeExpression.Execute(context))
                {
                    obj = o;

                    yield return o;
                }

                if (context.Scope.IsError)
                {
                    yield break;
                }

                obj = obj.Dereference();

                if (obj is not BadClassPrototype proto)
                {
                    throw new BadRuntimeException("Type expression must be a class prototype", Position);
                }

                type = proto;
            }

            context.Scope.DefineVariable(BadObject.Wrap(Name), BadObject.Null, new BadPropertyInfo(type, IsReadOnly));

            foreach (BadObject o in base.InnerExecute(context))
            {
                yield return o;
            }
        }
    }
}