using Terminal.Gui.Reflect.Interfaces;

namespace Terminal.Gui.Reflect.Extensions;

public static class DisposableExtensions
{
    public static void DisposeWith<T>(this IDisposable disposable, IViewController<T> controller) where T : View, new()
    {
        controller.AddCleanupOperation(disposable);
    }
}