using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types;
namespace BadScript2.Runtime.Objects;

/// <summary>
///     Implements the base functionality for a BadScript Reference
/// </summary>
public abstract class BadObjectReference : BadObject
{
	/// <summary>
	///     Returns the Referenced Object
	/// </summary>
	/// <returns>Referenced Object</returns>
	public abstract BadObject Resolve();

	/// <summary>
	///     Deletes the Reference from the Referenced Object
	/// </summary>
	public abstract void Delete();

	/// <summary>
	///     Sets the Referenced Object to a new Value
	/// </summary>
	/// <param name="obj">New Value</param>
	/// <param name="info">(Optional) Property Info</param>
	public abstract void Set(BadObject obj, BadPropertyInfo? info = null, bool noChangeEvent = false);

	/// <summary>
	///     Creates a new Reference Object
	/// </summary>
	/// <param name="refText">Text used for debugging</param>
	/// <param name="getter">Getter of the Property</param>
	/// <param name="setter">(optional) Setter of the Property</param>
	/// <param name="delete">The Delete Function of the Reference</param>
	/// <returns>Reference Instance</returns>
	public static BadObjectReference Make(
        string refText,
        Func<BadObject> getter,
        Action<BadObject, BadPropertyInfo?>? setter = null,
        Action? delete = null)
    {
        return new BadObjectReferenceImpl(
            refText,
            getter,
            (o, i, _) =>
            {
                if (setter == null)
                {
                    throw new BadRuntimeException("Cannot set reference " + refText + " because it is read-only");
                }
                setter(o, i);
            },
            delete
        );
    }
	/// <summary>
	///     Creates a new Reference Object
	/// </summary>
	/// <param name="refText">Text used for debugging</param>
	/// <param name="getter">Getter of the Property</param>
	/// <param name="setter">(optional) Setter of the Property</param>
	/// <param name="delete">The Delete Function of the Reference</param>
	/// <returns>Reference Instance</returns>
	public static BadObjectReference Make(
        string refText,
        Func<BadObject> getter,
        Action<BadObject, BadPropertyInfo?, bool>? setter,
        Action? delete = null)
    {
        return new BadObjectReferenceImpl(refText, getter, setter, delete);
    }

	/// <summary>
	///     Implements a Reference Object
	/// </summary>
	private class BadObjectReferenceImpl : BadObjectReference
    {
	    /// <summary>
	    ///     Deletes the Reference from the Referenced Object
	    /// </summary>
	    private readonly Action? m_Delete;

	    /// <summary>
	    ///     The Getter of the Reference
	    /// </summary>
	    private readonly Func<BadObject> m_Getter;

	    /// <summary>
	    ///     The Debug Text
	    /// </summary>
	    private readonly string m_RefText;

	    /// <summary>
	    ///     The Setter of the Reference
	    /// </summary>
	    private readonly Action<BadObject, BadPropertyInfo?, bool>? m_Setter;

	    /// <summary>
	    ///     Creates a new Reference Object
	    /// </summary>
	    /// <param name="refText">The Reference Debug Text</param>
	    /// <param name="getter">Getter of the Reference</param>
	    /// <param name="setter">Setter of the Reference</param>
	    /// <param name="delete">The Delete Function of the Reference</param>
	    public BadObjectReferenceImpl(
            string refText,
            Func<BadObject> getter,
            Action<BadObject, BadPropertyInfo?, bool>? setter,
            Action? delete)
        {
            m_Getter = getter;
            m_Setter = setter;
            m_Delete = delete;
            m_RefText = refText;
        }

        /// <inheritdoc />
        public override BadClassPrototype GetPrototype()
        {
            return m_Getter().GetPrototype();
        }

        /// <inheritdoc />
        public override BadObject Resolve()
        {
            return m_Getter();
        }

        /// <inheritdoc />
        public override void Set(BadObject obj, BadPropertyInfo? info = null, bool noChangeEvent = false)
        {
            if (m_Setter == null)
            {
                throw new BadRuntimeException("Cannot set reference " + m_RefText + " because it is read-only");
            }

            m_Setter(obj, info ?? new BadPropertyInfo(BadAnyPrototype.Instance), noChangeEvent);
        }

        /// <inheritdoc />
        public override string ToSafeString(List<BadObject> done)
        {
            return m_RefText;
        }

        /// <inheritdoc />
        public override void Delete()
        {
            if (m_Delete == null)
            {
                throw new BadRuntimeException("Cannot set delete " + m_RefText + " because it is read-only");
            }

            m_Delete.Invoke();
        }
    }
}