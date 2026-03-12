using System.Reactive.Linq;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Interfaces;

namespace Terminal.Gui.Reflect.Services;

public class ObservableTaskRunnerService(TimeSpan? defaultEvictionTimeAfterCompleted = null)
    : IObservableTaskRunnerService
{
    private readonly TimeSpan? _defaultEvictionTimeAfterCompleted =
        defaultEvictionTimeAfterCompleted ?? TimeSpan.FromSeconds(15);

    public ObservableItemCollection<IObservableTask> Tasks { get; } = [];

    public IObservableTask RunTask(Func<Task> taskFactory, string label, string description)
    {
        var task = new ObservableTask(taskFactory, label, description);
        BeginTrackingTask(task);
        return task;
    }

    public IObservableTask RunTask(Func<CancellationToken, Task> taskFactory, string label, string description)
    {
        var task = new ObservableTask(taskFactory, label, description);
        BeginTrackingTask(task);
        return task;
    }

    public IObservableTask RunTask(Func<IProgress<int>, CancellationToken, Task> taskFactory, string label,
        string                                                                   description)
    {
        var task = new ObservableTask(taskFactory, label, description);
        BeginTrackingTask(task);
        return task;
    }

    private void BeginTrackingTask(ObservableTask task)
    {
        Application.Invoke(() => { Tasks.Add(task); });
        task.Task.ContinueWith(t =>
        {
            Observable.Timer(_defaultEvictionTimeAfterCompleted ?? TimeSpan.FromSeconds(15))
                      .Subscribe(_ => { Application.Invoke(() => { Tasks.Remove(task); }); });
        });
    }
}