namespace Strem.Core.Services.Threading;

public interface IThreadHandler
{
    void For(int start, int end, Action<int> process);
    Task Run(Action process);
}