using System.Reactive;

namespace Strem.Core.Services.Execution;

public class ExclusiveExecutionHandler : IExclusiveExecutionHandler
{
    public const int FrequencyCheck = 250;
    
    public Dictionary<string, bool> ExclusivityGroupLocks { get; } = new();
    public object lockHandle = new();

    public async Task<bool> RequestLockFor(string group, CancellationToken cancellationToken = default)
    {
        lock (lockHandle)
        {
            if (!ExclusivityGroupLocks.ContainsKey(group))
            {
                ExclusivityGroupLocks.Add(group, true);
                return true;
            }
        }

        var isLocked = ExclusivityGroupLocks[group];
        if (isLocked)
        {
            while (ExclusivityGroupLocks[group])
            {
                if (cancellationToken.IsCancellationRequested)
                { return false; }
                
                await Task.Delay(FrequencyCheck);
            }
        }
        
        lock (lockHandle)
        { ExclusivityGroupLocks[group] = true; }

        return true;
    }

    public void FreeLockFor(string group)
    {
        if(!ExclusivityGroupLocks.ContainsKey(group))
        { return; }
        
        var isLocked = ExclusivityGroupLocks[group];
        if (!isLocked)
        { return; }

        lock (lockHandle)
        { ExclusivityGroupLocks[group] = false; }
    }
}