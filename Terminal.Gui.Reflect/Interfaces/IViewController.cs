using Terminal.Gui.Reflect.Base;

namespace Terminal.Gui.Reflect.Interfaces;

public interface IViewController<out TView> : IViewController, IDisposable where TView : View, new()
{
    TView                  Root { get; }
    void                   AddCleanupOperation(IDisposable disposable);
    void                   AddCleanupOperation(Action      cleanupAction);
    IViewController<TView> Apply(Action<TView>             setCallback);
}

public interface IViewController
{
    void InitializeComponents();
    void SetupBindings();

    View GetRootBase();
}