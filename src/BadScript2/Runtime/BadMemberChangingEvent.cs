using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

/// <summary>
/// Implements the Member Changing Event
/// </summary>
public class BadMemberChangingEvent : BadMemberChangeEvent
{
    /// <summary>
    /// Reference to the Cancel Property
    /// </summary>
    private readonly BadObjectReference m_CancelReference;

    /// <summary>
    /// Constructor for the Member Changing Event
    /// </summary>
    /// <param name="mInstance">The Instance of the Object that this event was fired on.</param>
    /// <param name="mMember">The Member of the Object that was changed.</param>
    /// <param name="mOldValue">The Old Value of the Member.</param>
    /// <param name="mNewValue">The New Value of the Member.</param>
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

    /// <summary>
    /// Returns true if the event was cancelled
    /// </summary>
    public bool Cancel { get; private set; }


    /// <summary>
    /// Returns the Prototype of the Event
    /// </summary>
    /// <returns>The Prototype of the Event</returns>
    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.MemberChangingEventArgs;
    }

    /// <inheritdoc/>
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        if (propName == "Cancel")
        {
            return true;
        }

        return base.HasProperty(propName, caller);
    }

    /// <inheritdoc/>
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        if (propName == "Cancel")
        {
            return m_CancelReference;
        }

        return base.GetProperty(propName, caller);
    }
}