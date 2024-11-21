using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

public class BadMemberChangingEvent : BadMemberChangeEvent
{
    private readonly BadObjectReference m_CancelReference;

    public BadMemberChangingEvent(BadObject mInstance, BadMemberInfo mMember, BadObject mOldValue, BadObject mNewValue)
        : base(mInstance, mMember, mOldValue, mNewValue)
    {
        m_CancelReference = BadObjectReference.Make("MemberChangingEvent.Cancel",
                                                    (p) => new BadInteropFunction("Cancel",
                                                         (ctx, args) =>
                                                         {
                                                             Cancel = true;

                                                             return Null;
                                                         },
                                                         false,
                                                         BadAnyPrototype.Instance
                                                        )
                                                   );
    }

    public bool Cancel { get; private set; }


    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.MemberChangingEventArgs;
    }

    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        if (propName == "Cancel")
        {
            return true;
        }

        return base.HasProperty(propName, caller);
    }

    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        if (propName == "Cancel")
        {
            return m_CancelReference;
        }

        return base.GetProperty(propName, caller);
    }
}