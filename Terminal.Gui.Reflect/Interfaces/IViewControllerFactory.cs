namespace Terminal.Gui.Reflect.Interfaces;

public interface IViewControllerFactory
{
    TView Create<TView>()
        where TView : IViewController;

    IViewController? Create(Type viewType);
}