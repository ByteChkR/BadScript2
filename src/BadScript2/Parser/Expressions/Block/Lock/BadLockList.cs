using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Block;

/// <summary>
/// The Lock List that is used to store all locks
/// </summary>
public class BadLockList
{
    /// <summary>
    /// Instance of the Lock List
    /// </summary>
    public static readonly BadLockList Instance = new BadLockList();
    
    /// <summary>
    /// Inner List that is used to store the Locks
    /// </summary>
    private readonly List<BadObject> m_LockList = new List<BadObject>();

    private BadLockList() { }

    /// <summary>
    /// Tries to aquire a lock on the given object
    /// </summary>
    /// <param name="lockObj">The object to be locked</param>
    /// <returns>True if the lock has been aquired</returns>
    public bool TryAquire(BadObject lockObj)
    {
        lock (m_LockList)
        {
            if (m_LockList.Contains(lockObj))
            {
                return false;
            }

            m_LockList.Add(lockObj);

            return true;
        }
    }

    /// <summary>
    /// Releases the lock on the given object
    /// </summary>
    /// <param name="lockObj">Object whose lock is to be released</param>
    /// <exception cref="BadRuntimeException">Gets raised if there is no lock in place for the given object</exception>
    public void Release(BadObject lockObj)
    {
        lock (m_LockList)
        {
            if (!m_LockList.Contains(lockObj))
            {
                throw new BadRuntimeException("Lock was not properly aquired!");
            }

            m_LockList.Remove(lockObj);
        }
    }
}