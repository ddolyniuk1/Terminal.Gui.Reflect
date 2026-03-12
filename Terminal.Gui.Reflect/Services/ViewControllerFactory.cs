using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Interfaces;

namespace Terminal.Gui.Reflect.Services;

public class ViewControllerFactory(IServiceProvider provider) : IViewControllerFactory
{
    public TViewController Create<TViewController>()
        where TViewController : IViewController
    {
        var             viewModelForViewType = GetViewModelForViewType(typeof(TViewController));
        TViewController view;
        if (viewModelForViewType != null)
        {
            var   viewModel = ActivatorUtilities.CreateInstance(provider, viewModelForViewType);
            view = ActivatorUtilities.CreateInstance<TViewController>(provider, viewModel);

            if (view is IPrivateSetViewModelMapper privateSetViewModelMapper)
            {
                privateSetViewModelMapper.SetViewModel(viewModel);
            }
        }
        else
        { 
            view = ActivatorUtilities.CreateInstance<TViewController>(provider);
        }

        view.Initialize();
        return view;
    }

    public IViewController? Create(Type viewType)
    {
        var    viewModelForViewType = GetViewModelForViewType(viewType);
        object view;
        if (viewModelForViewType != null)
        {
            var   viewModel = ActivatorUtilities.CreateInstance(provider, viewModelForViewType);
            view = ActivatorUtilities.CreateInstance(provider, viewType, viewModel);
            if (view is IPrivateSetViewModelMapper privateSetViewModelMapper)
            {
                privateSetViewModelMapper.SetViewModel(viewModel);
            }
        }
        else
        { 
            view = ActivatorUtilities.CreateInstance(provider, viewType);
        }

        if (view is not IViewController viewController)
        {
            return null;
        }

        viewController.Initialize();
        return viewController;
    }

    private static Type? GetViewModelForViewType(Type viewType)
    {
        try
        {
            var value = viewType.GetInterfaces()
                                .FirstOrDefault(i =>
                                                    i.IsGenericType &&
                                                    i.GetGenericTypeDefinition() ==
                                                    typeof(IViewFor<>))?.GetGenericArguments().ElementAtOrDefault(0);

            return value;
        }
        catch
        {
            return null;
        }
    }
}