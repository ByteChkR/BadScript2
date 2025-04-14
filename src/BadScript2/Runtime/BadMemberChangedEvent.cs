using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

/// <summary>
/// This class represents a member changed event.
/// </summary>
public class BadMemberChangedEvent : BadMemberChangeEvent
{
    /// <summary>
    /// Creates a new instance of the <see cref="BadMemberChangedEvent"/> class.
    /// </summary>
    /// <param name="mInstance">The Instance of the Object that this event was fired on.</param>
    /// <param name="mMember">The Member of the Object that was changed.</param>
    /// <param name="mOldValue">The Old Value of the Member.</param>
    /// <param name="mNewValue">The New Value of the Member.</param>
    public BadMemberChangedEvent(BadObject mInstance, BadMemberInfo mMember, BadObject mOldValue, BadObject mNewValue) :
        base(mInstance, mMember, mOldValue, mNewValue) { }

    /// <inheritdoc/>
    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.MemberChangedEventArgs;
    }
}