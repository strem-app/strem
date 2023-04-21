using Strem.Core.Services.Execution;

namespace Strem.UnitTests.Core.Execution;

public class ExclusiveExecutionHandlerTests
{
    [Fact]
    public async Task should_allocate_lock_on_group()
    {
        var groupName = "test-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();

        var succeeded = await exclusiveExecutionHandler.RequestLockFor(groupName);
        Assert.True(succeeded);
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks.ContainsKey(groupName));
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks[groupName]);
    }
    
    [Fact]
    public async Task should_stop_allocations_when_group_locked()
    {
        var groupName = "test-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();

        exclusiveExecutionHandler.ExclusivityGroupLocks.Add(groupName, true);

        using var timeoutToken = new CancellationTokenSource(100);
        var succeeded = await exclusiveExecutionHandler.RequestLockFor(groupName, timeoutToken.Token);
        Assert.False(succeeded);
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks.ContainsKey(groupName));
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks[groupName]);
    }
    
    [Fact]
    public async Task should_allow_allocations_when_group_not_locked_but_others_are()
    {
        var lockedGroupName = "locked-group";
        var unlockedGroupName = "unlocked-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();

        exclusiveExecutionHandler.ExclusivityGroupLocks.Add(lockedGroupName, true);

        using var timeoutToken = new CancellationTokenSource(100);
        var succeeded = await exclusiveExecutionHandler.RequestLockFor(unlockedGroupName, timeoutToken.Token);
        Assert.True(succeeded);
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks.ContainsKey(unlockedGroupName));
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks[unlockedGroupName]);
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks[lockedGroupName]);
    }
    
    [Fact]
    public void should_not_throw_error_freeing_lock_when_group_doesnt_exist()
    {
        var groupName = "test-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();

        exclusiveExecutionHandler.FreeLockFor(groupName);
        Assert.False(exclusiveExecutionHandler.ExclusivityGroupLocks.ContainsKey(groupName));
    }
    
    [Fact]
    public void should_not_throw_error_freeing_lock_when_group_already_unlocked()
    {
        var groupName = "test-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();
        exclusiveExecutionHandler.ExclusivityGroupLocks.Add(groupName, false);

        exclusiveExecutionHandler.FreeLockFor(groupName);
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks.ContainsKey(groupName));
        Assert.False(exclusiveExecutionHandler.ExclusivityGroupLocks[groupName]);
    }
    
    [Fact]
    public void should_free_lock_when_group_is_locked()
    {
        var groupName = "test-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();
        exclusiveExecutionHandler.ExclusivityGroupLocks.Add(groupName, true);

        exclusiveExecutionHandler.FreeLockFor(groupName);
        Assert.True(exclusiveExecutionHandler.ExclusivityGroupLocks.ContainsKey(groupName));
        Assert.False(exclusiveExecutionHandler.ExclusivityGroupLocks[groupName]);
    }

    [Fact]
    public void should_handle_multithreaded_locks_at_once()
    {
        var groupName = "test-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();
        var oneResult = false;
        var twoResult = false;

        var one = Task.Run(async () =>
        {
            using var tokenSource = new CancellationTokenSource(10);
            oneResult = await exclusiveExecutionHandler.RequestLockFor(groupName, tokenSource.Token);
            
        });
        var two = Task.Run(async () =>
        {
            using var tokenSource = new CancellationTokenSource(10);
            twoResult = await exclusiveExecutionHandler.RequestLockFor(groupName, tokenSource.Token);
        });

        Task.WaitAll(one, two);

        if (oneResult) 
        { Assert.False(twoResult); }
        else 
        { Assert.False(oneResult); }
    }
    
    [Fact]
    public void should_handle_multithreaded_lock_and_frees_at_once_within_timeout()
    {
        var groupName = "test-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();
        var oneResult = false;
        var twoResult = false;

        var one = Task.Run(async () =>
        {
            using var tokenSource = new CancellationTokenSource(100);
            oneResult = await exclusiveExecutionHandler.RequestLockFor(groupName, tokenSource.Token);
            await Task.Delay(5);
            exclusiveExecutionHandler.FreeLockFor(groupName);

        });
        
        var two = Task.Run(async () =>
        {
            using var tokenSource = new CancellationTokenSource(100);
            twoResult = await exclusiveExecutionHandler.RequestLockFor(groupName, tokenSource.Token);
            await Task.Delay(5);
            exclusiveExecutionHandler.FreeLockFor(groupName);
        });

        Task.WaitAll(one, two);

        Assert.True(oneResult);
        Assert.True(twoResult);
    }
    
    [Fact]
    public void should_handle_multithreaded_lock_and_frees_at_once_outside_timeout()
    {
        var groupName = "test-group";
        var exclusiveExecutionHandler = new ExclusiveExecutionHandler();
        var oneResult = false;
        var twoResult = false;
        var delayPeriodOverFrequencyCheck = ExclusiveExecutionHandler.FrequencyCheck + 10;
        
        var one = Task.Run(async () =>
        {
            using var tokenSource = new CancellationTokenSource(5);
            oneResult = await exclusiveExecutionHandler.RequestLockFor(groupName, tokenSource.Token);
            await Task.Delay(delayPeriodOverFrequencyCheck);
            exclusiveExecutionHandler.FreeLockFor(groupName);
        });
        
        var two = Task.Run(async () =>
        {
            using var tokenSource = new CancellationTokenSource(5);
            twoResult = await exclusiveExecutionHandler.RequestLockFor(groupName, tokenSource.Token);
            await Task.Delay(delayPeriodOverFrequencyCheck);
            exclusiveExecutionHandler.FreeLockFor(groupName);
        });

        Task.WaitAll(one, two);

        if (oneResult) 
        { Assert.False(twoResult); }
        else 
        { Assert.False(oneResult); }
    }
}