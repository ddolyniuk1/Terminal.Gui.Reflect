using System.ComponentModel;
using DotNext;

namespace Terminal.Gui.Reflect.Base;

public interface IObservableTask : INotifyPropertyChanged
{
    DateTimeOffset                     CompletionTime          { get; }
    string?                            ErrorMessage            { get; }
    bool                               IsCanceled              { get; }
    bool                               IsCancellable           { get; }
    bool                               IsCompletedSuccessfully { get; }
    bool                               IsFaulted               { get; }
    bool                               IsIndeterminate         { get; }
    bool                               IsRunning               { get; }
    int                                Progress                { get; }
    DateTimeOffset                     StartTime               { get; }
    string                             Description             { get; }
    string                             Label                   { get; }
    Task                               Task                    { get; }
    EObservableTaskStatus              Status                  { get; }
    Result<bool>                       Cancel();
}