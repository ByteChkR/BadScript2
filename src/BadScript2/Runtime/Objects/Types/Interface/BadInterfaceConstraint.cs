/// <summary>
/// Contains Runtime Interface Objects
/// </summary>

namespace BadScript2.Runtime.Objects.Types.Interface;

/// <summary>
///     Implements an Interface Constraint
/// </summary>
public abstract class BadInterfaceConstraint : IEquatable<BadInterfaceConstraint>
{
#region IEquatable<BadInterfaceConstraint> Members

    public abstract bool Equals(BadInterfaceConstraint? other);

#endregion

    /// <summary>
    ///     Validates the given Object against this Constraint
    /// </summary>
    /// <param name="obj">The Object to validate</param>
    /// <param name="errors">The Error List to add errors to</param>
    public abstract void Validate(BadClass obj, List<BadInterfaceValidatorError> errors);

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((BadInterfaceConstraint)obj);
    }

    /// <summary>
    /// Calculates the HashCode for this Constraint
    /// </summary>
    /// <returns>The HashCode</returns>
    protected abstract int GetConstraintHash();

    
    /// <inheritdoc />
    public override int GetHashCode()
    {
        return GetConstraintHash();
    }

    /// <summary>
    /// Implements the == operator for this Constraint
    /// </summary>
    /// <param name="left">Left Constraint</param>
    /// <param name="right">Right Constraint</param>
    /// <returns>True if the Constraints are equal</returns>
    public static bool operator ==(BadInterfaceConstraint? left, BadInterfaceConstraint? right)
    {
        return Equals(left, right);
    }


    /// <summary>
    /// Implements the != operator for this Constraint
    /// </summary>
    /// <param name="left">Left Constraint</param>
    /// <param name="right">Right Constraint</param>
    /// <returns>True if the Constraints are not equal</returns>
    public static bool operator !=(BadInterfaceConstraint? left, BadInterfaceConstraint? right)
    {
        return !Equals(left, right);
    }
}