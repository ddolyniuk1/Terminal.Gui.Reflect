using System.ComponentModel;
using System.Runtime.CompilerServices;
using DotNext;

namespace Terminal.Gui.Reflect.Base;

public enum EObservableTaskStatus
{
    Running,
    Cancelled,
    Completed,
    Failed
}

public class ObservableTask : IObservableTask
{
    private static readonly HashSet<string> StatusPropertyNames =
        [nameof(IsRunning), nameof(IsFaulted), nameof(IsCanceled), nameof(IsCompletedSuccessfully)];

    private readonly CancellationTokenSource? _cts;

    public ObservableTask(Func<Task> taskFactory, string label, string description)
    {
        Label           = label;
        Description     = description;
        IsRunning       = true;
        IsCancellable   = false;
        IsIndeterminate = true;
        Task = Task.Run(taskFactory)
                   .ContinueWith(result =>
                                 {
                                     IsCanceled              = result.IsCanceled;
                                     IsFaulted               = result.IsFaulted;
                                     IsCompletedSuccessfully = result.IsCompletedSuccessfully;
                                     IsRunning               = !result.IsCompleted;
                                     ErrorMessage            = result.Exception?.Message;
                                     CompletionTime          = DateTimeOffset.Now;
                                 }, CancellationToken.None, TaskContinuationOptions.None,
                                 TaskScheduler.FromCurrentSynchronizationContext());
    }

    public ObservableTask(Func<CancellationToken, Task> taskFactory, string label, string description)
    {
        Label           = label;
        Description     = description;
        IsRunning       = true;
        IsCancellable   = true;
        IsIndeterminate = true;
        _cts            = new CancellationTokenSource();
        Task = Task.Run(() => taskFactory(_cts.Token))
                   .ContinueWith(result =>
                                 {
                                     IsCanceled              = result.IsCanceled;
                                     IsFaulted               = result.IsFaulted;
                                     IsCompletedSuccessfully = result.IsCompletedSuccessfully;
                                     IsRunning               = !result.IsCompleted;
                                     ErrorMessage            = result.Exception?.Message;
                                     _cts.Dispose();
                                     CompletionTime = DateTimeOffset.Now;
                                 }, CancellationToken.None, TaskContinuationOptions.None,
                                 TaskScheduler.FromCurrentSynchronizationContext());
    }

    public ObservableTask(Func<IProgress<int>, CancellationToken, Task> taskFactory, string label, string description)
    {
        Label         = label;
        Description   = description;
        IsRunning     = true;
        IsCancellable = true;
        _cts          = new CancellationTokenSource();
        var progress = new Progress<int>();
        progress.ProgressChanged += ProgressOnProgressChanged;
        Task = Task.Run(() => taskFactory(progress, _cts.Token))
                   .ContinueWith(result =>
                    {
                        Application.Invoke(() =>
                        {
                            IsCanceled              = result.IsCanceled;
                            IsFaulted               = result.IsFaulted;
                            IsCompletedSuccessfully = result.IsCompletedSuccessfully;
                            IsRunning               = !result.IsCompleted;
                            ErrorMessage            = result.Exception?.Message;
                            _cts.Dispose();
                            progress.ProgressChanged -= ProgressOnProgressChanged;
                            CompletionTime           =  DateTimeOffset.Now;
                        });
                    });
    }

    public DateTimeOffset CompletionTime
    {
        get;
        private set => SetField(ref field, value);
    }

    public string? ErrorMessage
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool IsCanceled
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool IsCancellable
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool IsCompletedSuccessfully
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool IsFaulted
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool IsIndeterminate
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool IsRunning
    {
        get;
        private set => SetField(ref field, value);
    }

    public int Progress
    {
        get;
        private set => SetField(ref field, value);
    }

    public DateTimeOffset StartTime { get; } = DateTimeOffset.Now;

    public string Description { get; }
    public string Label       { get; }

    public Task Task { get; }

    public EObservableTaskStatus Status
    {
        get;
        private set => SetField(ref field, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Result<bool> Cancel()
    {
        if (IsCanceled)
        {
            return true;
        }

        IsCanceled = true;

        if (!IsCancellable || _cts == null)
        {
            return new Result<bool>(new InvalidOperationException("Cannot cancel this task!"));
        }

        if (!_cts.Token.CanBeCanceled)
        {
            return false;
        }

        try
        {
            _cts.Cancel();
        }
        catch (Exception e)
        {
            return new Result<bool>(e);
        }

        return true;
    }

    private void ProgressOnProgressChanged(object? sender, int value)
    {
        Progress = value;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        if (propertyName == null || !StatusPropertyNames.Contains(propertyName))
        {
            return;
        }

        Status = IsRunning ? EObservableTaskStatus.Running : Status;
        Status = IsFaulted ? EObservableTaskStatus.Failed : Status;
        Status = IsCanceled ? EObservableTaskStatus.Cancelled : Status;
        Status = IsCompletedSuccessfully ? EObservableTaskStatus.Completed : Status;
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}