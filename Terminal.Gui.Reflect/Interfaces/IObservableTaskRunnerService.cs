using Terminal.Gui.Reflect.Base;

namespace Terminal.Gui.Reflect.Interfaces;

public interface IObservableTaskRunnerService
{
    IObservableTask RunTask(Func<Task>                                    taskFactory, string label, string description);
    IObservableTask RunTask(Func<CancellationToken, Task>                 taskFactory, string label, string description);
    IObservableTask RunTask(Func<IProgress<int>, CancellationToken, Task> taskFactory, string label, string description);
    ObservableItemCollection<IObservableTask> Tasks { get; }
}