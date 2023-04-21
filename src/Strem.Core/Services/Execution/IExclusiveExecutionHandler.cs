namespace Strem.Core.Services.Execution;

/// <summary>
/// This wraps up the notion of allowing code to request locks on a group name so other code cannot execute
/// </summary>
public interface IExclusiveExecutionHandler
{
    /// <summary>
    /// This requests a lock and will either wait for success or timeout
    /// </summary>
    /// <param name="group">The name of the group to request a lock on</param>
    /// <param name="cancellationToken">The cancellation token for stopping request</param>
    /// <returns>A true for success on lock granted, a false for unable to grant lock</returns>
    /// <remarks>An exception is not thrown when timeout occurs, you just get a false return</remarks>
    Task<bool> RequestLockFor(string group, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// This frees up a lock on a given group
    /// </summary>
    /// <param name="group">The name of the group to request have the lock freed</param>
    /// <remarks>This should ALWAYS be called after a lock request has been granted</remarks>
    void FreeLockFor(string group);
}