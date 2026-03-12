namespace Terminal.Gui.Reflect.Base;

public interface IViewModelMapper<TViewModel>
{
    TViewModel? ViewModel { get; }
}

public interface IViewModelMapper
{
}