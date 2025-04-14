using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime;

/// <summary>
/// Represents a base class for member change events.
/// </summary>
public abstract class BadMemberChangeEvent : BadObject
{
    /// <summary>
    /// The instance of the object that changed.
    /// </summary>
    private readonly BadObject m_Instance;
    /// <summary>
    /// The member that changed.
    /// </summary>
    private readonly BadMemberInfo m_Member;
    /// <summary>
    /// The new value of the member.
    /// </summary>
    private readonly BadObject m_NewValue;
    /// <summary>
    /// The old value of the member.
    /// </summary>
    private readonly BadObject m_OldValue;

    /// <summary>
    /// Creates a new instance of the <see cref="BadMemberChangeEvent"/> class.
    /// </summary>
    /// <param name="mInstance">The Instance of the Object that this event was fired on.</param>
    /// <param name="mMember">The Member of the Object that was changed.</param>
    /// <param name="mOldValue">The Old Value of the Member.</param>
    /// <param name="mNewValue">The New Value of the Member.</param>
    protected BadMemberChangeEvent(BadObject mInstance, BadMemberInfo mMember, BadObject mOldValue, BadObject mNewValue)
    {
        m_Instance = mInstance;
        m_Member = mMember;
        m_OldValue = mOldValue;
        m_NewValue = mNewValue;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        switch (propName)
        {
            case "Instance":
                return BadObjectReference.Make("MemberChangeEvent.Instance", (p) => m_Instance);
            case "Member":
                return BadObjectReference.Make("MemberChangeEvent.Member", (p) => m_Member);
            case "OldValue":
                return BadObjectReference.Make("MemberChangeEvent.OldValue", (p) => m_OldValue);
            case "NewValue":
                return BadObjectReference.Make("MemberChangeEvent.NewValue", (p) => m_NewValue);
            default:
                return base.GetProperty(propName, caller);
        }
    }

    /// <inheritdoc/>
    public override string ToSafeString(List<BadObject> done)
    {
        return $"{GetType().Name}: " + m_Member.ToSafeString(done);
    }
}