using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;
namespace BadScript2.Runtime;

public class BadMemberChangedEvent : BadMemberChangeEvent
{
    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.MemberChangedEventArgs;
    }

    public BadMemberChangedEvent(BadObject mInstance, BadMemberInfo mMember, BadObject mOldValue, BadObject mNewValue) : base(mInstance, mMember, mOldValue, mNewValue)
    {
    }
}