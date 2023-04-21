namespace Strem.Core.Services.Execution;

public interface IExclusiveExecutionHandler
{
    Task<bool> RequestLockFor(string group, CancellationToken cancellationToken = default);
    void FreeLockFor(string group);
}