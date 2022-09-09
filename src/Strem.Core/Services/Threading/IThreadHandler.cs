namespace Strem.Core.Threading;

public interface IThreadHandler
{
    void For(int start, int end, Action<int> process);
    Task Run(Action process);
}