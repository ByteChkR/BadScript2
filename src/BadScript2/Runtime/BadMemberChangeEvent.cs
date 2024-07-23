using BadScript2.Runtime.Objects;
namespace BadScript2.Runtime;

public abstract class BadMemberChangeEvent : BadObject
{
    private readonly BadObject m_Instance;
    private readonly BadMemberInfo m_Member;
    private readonly BadObject m_NewValue;
    private readonly BadObject m_OldValue;

    protected BadMemberChangeEvent(BadObject mInstance, BadMemberInfo mMember, BadObject mOldValue, BadObject mNewValue)
    {
        m_Instance = mInstance;
        m_Member = mMember;
        m_OldValue = mOldValue;
        m_NewValue = mNewValue;
    }

    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        switch (propName)
        {
            case "Instance":
            case "Member":
            case "OldValue":
            case "NewValue":
                return true;
            default:
                return base.HasProperty(propName, caller);
        }
    }

    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        switch (propName)
        {
            case "Instance":
                return BadObjectReference.Make("MemberChangeEvent.Instance", () => m_Instance);
            case "Member":
                return BadObjectReference.Make("MemberChangeEvent.Member", () => m_Member);
            case "OldValue":
                return BadObjectReference.Make("MemberChangeEvent.OldValue", () => m_OldValue);
            case "NewValue":
                return BadObjectReference.Make("MemberChangeEvent.NewValue", () => m_NewValue);
            default:
                return base.GetProperty(propName, caller);
        }
    }
    public override string ToSafeString(List<BadObject> done)
    {
        return $"{GetType().Name}: " + m_Member.ToSafeString(done);
    }
}