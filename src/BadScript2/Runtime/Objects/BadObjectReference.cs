using BadScript2.Common;
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
	public abstract BadObject Resolve(BadSourcePosition? position);

	/// <summary>
	///     Deletes the Reference from the Referenced Object
	/// </summary>
	public abstract void Delete(BadSourcePosition? position);

	/// <summary>
	///     Sets the Referenced Object to a new Value
	/// </summary>
	/// <param name="obj">New Value</param>
	/// <param name="info">(Optional) Property Info</param>
	public abstract void Set(BadObject obj, BadSourcePosition? position, BadPropertyInfo? info = null, bool noChangeEvent = false);

	/// <summary>
	///     Creates a new Reference Object
	/// </summary>
	/// <param name="refText">Text used for debugging</param>
	/// <param name="getter">Getter of the Property</param>
	/// <param name="setter">(optional) Setter of the Property</param>
	/// <param name="delete">The Delete Function of the Reference</param>
	/// <returns>Reference Instance</returns>
	public static BadObjectReference Make(string refText,
	                                      Func<BadSourcePosition?, BadObject> getter,
	                                      Action<BadObject,BadSourcePosition?,  BadPropertyInfo?>? setter = null,
	                                      Action<BadSourcePosition?>? delete = null)
    {
        return new BadObjectReferenceImpl(refText,
                                          getter,
                                          (o,p, i, _) =>
                                          {
                                              if (setter == null)
                                              {
	                                              throw BadRuntimeException.Create(null, $"Cannot set reference {refText} because it is read-only", p);
                                              }

                                              setter(o, p, i);
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
	public static BadObjectReference Make(string refText,
	                                      Func<BadSourcePosition?, BadObject> getter,
	                                      Action<BadObject, BadSourcePosition?, BadPropertyInfo?, bool>? setter,
	                                      Action<BadSourcePosition?>? delete = null)
    {
        return new BadObjectReferenceImpl(refText, getter, setter, delete);
    }

#region Nested type: BadObjectReferenceImpl

	/// <summary>
	///     Implements a Reference Object
	/// </summary>
	private class BadObjectReferenceImpl : BadObjectReference
    {
	    /// <summary>
	    ///     Deletes the Reference from the Referenced Object
	    /// </summary>
	    private readonly Action<BadSourcePosition?>? m_Delete;

	    /// <summary>
	    ///     The Getter of the Reference
	    /// </summary>
	    private readonly Func<BadSourcePosition?, BadObject> m_Getter;

	    /// <summary>
	    ///     The Debug Text
	    /// </summary>
	    private readonly string m_RefText;

	    /// <summary>
	    ///     The Setter of the Reference
	    /// </summary>
	    private readonly Action<BadObject, BadSourcePosition?, BadPropertyInfo?, bool>? m_Setter;

	    /// <summary>
	    ///     Creates a new Reference Object
	    /// </summary>
	    /// <param name="refText">The Reference Debug Text</param>
	    /// <param name="getter">Getter of the Reference</param>
	    /// <param name="setter">Setter of the Reference</param>
	    /// <param name="delete">The Delete Function of the Reference</param>
	    public BadObjectReferenceImpl(string refText,
	                                  Func<BadSourcePosition?, BadObject> getter,
	                                  Action<BadObject, BadSourcePosition?, BadPropertyInfo?, bool>? setter,
	                                  Action<BadSourcePosition?>? delete)
        {
            m_Getter = getter;
            m_Setter = setter;
            m_Delete = delete;
            m_RefText = refText;
        }

        /// <inheritdoc />
        public override BadClassPrototype GetPrototype()
        {
            return m_Getter(null)
                .GetPrototype();
        }

        /// <inheritdoc />
        public override BadObject Resolve(BadSourcePosition? position)
        {
            return m_Getter(position);
        }

        /// <inheritdoc />
        public override void Set(BadObject obj, BadSourcePosition? position, BadPropertyInfo? info = null, bool noChangeEvent = false)
        {
            if (m_Setter == null)
            {
                throw new BadRuntimeException("Cannot set reference " + m_RefText + " because it is read-only");
            }

            m_Setter(obj, position, info ?? new BadPropertyInfo(BadAnyPrototype.Instance), noChangeEvent);
        }

        /// <inheritdoc />
        public override string ToSafeString(List<BadObject> done)
        {
            return m_RefText;
        }

        /// <inheritdoc />
        public override void Delete(BadSourcePosition? position)
        {
            if (m_Delete == null)
            {
                throw new BadRuntimeException("Cannot set delete " + m_RefText + " because it is read-only");
            }

            m_Delete.Invoke(position);
        }
    }

#endregion
}