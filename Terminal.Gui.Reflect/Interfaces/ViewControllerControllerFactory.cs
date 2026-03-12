using Microsoft.Extensions.DependencyInjection;

namespace Terminal.Gui.Reflect.Interfaces;

public class ViewControllerControllerFactory(IServiceProvider provider) : IViewControllerFactory
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
        }
        else
        { 
            view = ActivatorUtilities.CreateInstance<TViewController>(provider);
        }
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
        }
        else
        { 
            view = ActivatorUtilities.CreateInstance(provider, viewType);
        }
        return view as IViewController;
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